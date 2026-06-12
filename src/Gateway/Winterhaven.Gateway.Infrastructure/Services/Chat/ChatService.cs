using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Winterhaven.Common.Events;
using Winterhaven.Common.Events.Chat;
using Winterhaven.Common.Exceptions;
using Winterhaven.Gateway.Core.Application.Services.Chat;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.Exceptions;

namespace Winterhaven.Gateway.Infrastructure.Services.Chat;

//// TODO: This should likely be a singleton and take in UserSession or userAccountId as a parameter.
internal sealed class ChatService : IChatService
{
    private static readonly Dictionary<string, ChatEmoteType> CommandToChatEmoteTypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["/love"] = ChatEmoteType.Heart,
        ["/?"] = ChatEmoteType.Question,
        ["/!"] = ChatEmoteType.Exclaim,
        ["/..."] = ChatEmoteType.Ellipsis,
    };

    private static readonly string[] WhiteListEffects =
    [
        "/shake",
        "/wobble",
        "/blink",
        "/rainbow"
    ];

    private readonly ILogger<ChatService> logger;

    private readonly IMessageBus messageBus;

    private readonly IUserSessionContext userSessionContext;

    public ChatService(ILogger<ChatService> logger, IUserSessionContext userSessionContext, IMessageBus messageBus)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userSessionContext = userSessionContext ?? throw new ArgumentNullException(nameof(userSessionContext));
        this.messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        const string failureMessage = "Failed to send message, please try again in a few moments.";

        if (!this.userSessionContext.IsAuthenticated || this.userSessionContext.UserSession == null)
        {
            this.logger.LogError("Failed to send message, user session is not authenticated with ID: '{PlayerId}'", this.userSessionContext.UserSession?.UserAccountId);
            throw new AuthorizationException(failureMessage);
        }

        var userSession = this.userSessionContext.UserSession;

        this.logger.LogDebug("Player with ID: '{PlayerId}' says: '{Message}'", userSession.UserAccountId, message);

        try
        {
            await this.messageBus.PublishAsync(
                data: this.ParseChatMessage(message),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (MessageBusException ex)
        {
            this.logger.LogError(ex, "Failed to broadcast chat notification for player with ID: '{UserAccountId}'", userSession.UserAccountId);
            throw new ChatMessageException(failureMessage);
        }
    }

    private ChatEvent ParseChatMessage(string message)
    {
        const int maxMessageLength = 80;

        string trimmed = message.Trim();

        // Limit message length to the maximum length instead of just not sending the message.
        if (trimmed.Length > maxMessageLength)
        {
            trimmed = trimmed[..maxMessageLength];
        }

        //// Remove unintended Scribble effects
        //// Scribble only requires '[' to be escaped, not ']': https://www.jujuadams.com/Scribble/#/latest/text-formatting
        trimmed = trimmed.Replace("[", "[[", StringComparison.InvariantCulture);

        string[] tokens = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var emoteType = ChatEmoteType.None;

        string body = string.Join(' ', tokens.Select(token =>
        {
            if (token.StartsWith('/'))
            {
                //// Remove all occurances of emote commands from the body.
                //// Only use the last emote that is found.
                if (CommandToChatEmoteTypeMap.TryGetValue(token, out var chatEmoteType))
                {
                    emoteType = chatEmoteType;
                    return null;
                }

                // Convert effects to Scribble effects.
                if (WhiteListEffects.Contains(token))
                {
                    return $"[{token[1..]}]";
                }
            }

            return token;
        })
        .Where(token => token != null))
        .Trim();

        this.logger.LogTrace("Parsed Incoming Message: '{Message}'", body);

        return new ChatEvent()
        {
            EmoteType = emoteType,
            SenderId = this.userSessionContext.UserSession!.UserAccountId,
            Message = body,
        };
    }
}
