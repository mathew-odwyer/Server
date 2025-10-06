namespace Web.Presentation.Middleware.Users;

using System.Security.Claims;
using Web.Application.Contexts.Users;

internal sealed class UserSessionValidationMiddleware
{
    private readonly ILogger<UserSessionValidationMiddleware> logger;

    private readonly RequestDelegate next;

    public UserSessionValidationMiddleware(ILogger<UserSessionValidationMiddleware> logger, RequestDelegate next)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context, IUserSessionTokenRepository userSessionTokenRepository)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(userSessionTokenRepository);

        // If the user is not authenticated, we can skip the validation.
        // This is because the user session validation is only necessary for authenticated users.
        // When the user is not authenticated, we do not have any user information to validate against.
        if (context.User.Identity?.IsAuthenticated == true)
        {
            string? identifier = context.User.FindFirstValue("identifier");
            string? session = context.User.FindFirstValue("session");

            // If the claims are null, empty or whitespace we can't proceed with validation.
            // However, because the user has authenticated, something has gone wrong so we should return a 401 Unauthorized.
            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(session) || !Guid.TryParse(session, out var sessionId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            this.logger.LogDebug("Validating active session for user {UserId} on path {Path}", identifier, context.Request.Path);

            // Finally, check whether the user has an active session.
            var activeSession = await userSessionTokenRepository.GetBySessionIdAsync(sessionId, context.RequestAborted).ConfigureAwait(false);

            // If they don't, we can assume they're not authorized to make the requset because either the JWT has expired or they've logged out.
            if (activeSession == null)
            {
                this.logger.LogWarning("No active session found for user {UserId}. Returning 401. Path: {Path}", identifier, context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            this.logger.LogDebug("Active session confirmed for user {UserId}. Continuing request pipeline.", identifier);
        }

        await this.next(context).ConfigureAwait(false);
    }
}
