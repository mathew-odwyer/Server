using System.Threading;
using System.Threading.Tasks;

namespace Winterhaven.Gateway.Core.Application.Services.Chat;

/// <summary>
///   Defines an interface that provides methods to handle chat between players.
/// </summary>
public interface IChatService
{
    /// <summary>
    ///   Sends a message to all players connected to the gateway.
    /// </summary>
    /// <param name="message">
    ///   The message to be sent to all players.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token used to cancel the operation.
    /// </param>
    /// <returns>
    ///   Returns a <see cref="Task"/> that is completed once the message has been published.
    /// </returns>
    public Task SendMessageAsync(string message, CancellationToken cancellationToken = default);
}
