namespace Web.Presentation.Controllers.Players;

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.DTOs.Players;
using Web.Application.Requests.Players;

public sealed class PlayerController : ApiControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreatePlayerRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestDto);

        var request = new CreatePlayerRequest()
        {
            Name = requestDto.Name,
            UserAccountId = this.User.FindFirstValue("identifier")!,
        };

        var response = await this.Sender.Send(request, cancellationToken).ConfigureAwait(false);

        if (response.IsFailed)
        {
            return this.BadRequest(response.Value);
        }

        return this.Ok(response.Value);
    }
}
