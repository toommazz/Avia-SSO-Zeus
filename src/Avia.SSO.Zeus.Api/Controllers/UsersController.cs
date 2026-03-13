using Avia.SSO.Zeus.Application.Identity.Commands.ChangePassword;
using Avia.SSO.Zeus.Application.Identity.Commands.EnableTwoFactor;
using Avia.SSO.Zeus.Application.Identity.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Avia.SSO.Zeus.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public sealed class UsersController(IMediator mediator) : ApiController
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserQuery(id), ct);
        return Handle(result);
    }

    [HttpPut("{id:guid}/password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var command = new ChangePasswordCommand(id, request.CurrentPassword, request.NewPassword);
        var result = await mediator.Send(command, ct);
        return Handle(result);
    }

    [HttpPost("{id:guid}/two-factor")]
    public async Task<IActionResult> EnableTwoFactor(Guid id, [FromBody] EnableTwoFactorRequest request, CancellationToken ct)
    {
        var command = new EnableTwoFactorCommand(id, request.Method);
        var result = await mediator.Send(command, ct);
        return Handle(result);
    }
}

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public sealed record EnableTwoFactorRequest(Avia.SSO.Zeus.Domain.Identity.Enums.TwoFactorMethod Method);
