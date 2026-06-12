using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using Winterhaven.Common.Exceptions;
using Winterhaven.Gateway.Core.Application.Services.Chat;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Presentation.Attributes;

namespace Winterhaven.Gateway.Presentation.Targets.Chat;

internal sealed record ChatSendMessageRpcParameters(
    string Message);

internal sealed class ChatRpcTarget : IRpcTarget
{
    private readonly IChatService chatService;

    private readonly ILogger<ChatRpcTarget> logger;

    private readonly IUserSessionContext userSessionContext;

    public ChatRpcTarget(ILogger<ChatRpcTarget> logger, IUserSessionContext userSessionContext, IChatService chatService)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userSessionContext = userSessionContext ?? throw new ArgumentNullException(nameof(userSessionContext));
        this.chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
    }

    [JsonRpcAuthorize]
    [JsonRpcMethod("chat.send_message", UseSingleObjectParameterDeserialization = true)]
    public async Task SendMessageAsync(ChatSendMessageRpcParameters parameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var userSession = this.userSessionContext.UserSession;

        if (!this.userSessionContext.IsAuthenticated || userSession == null)
        {
            this.logger.LogError("Failed to send message, user session is not authenticated!");
            throw new AuthorizationException("Failed to send message, please try again in a few moments.");
        }

        var userAccountId = userSession.UserAccountId;
        string message = parameters.Message;

        this.logger.LogTrace("Sending chat message for with player with ID: '{PlayerId}': {Message}", userAccountId, message);

        await this.chatService.SendMessageAsync(message, cancellationToken).ConfigureAwait(false);
    }
}
