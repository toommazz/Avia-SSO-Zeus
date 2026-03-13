using Avia.SSO.Zeus.Application.Identity.Commands.Login;
using Avia.SSO.Zeus.Application.Identity.Commands.RefreshToken;
using Avia.SSO.Zeus.Application.Identity.Commands.Register;
using Avia.SSO.Zeus.Application.Identity.Commands.VerifyTwoFactor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Avia.SSO.Zeus.Api.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
public sealed class AuthController(IMediator mediator) : ApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Handle(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var tenantHeader = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        var tenantId = Guid.TryParse(tenantHeader, out var parsed) ? parsed : command.TenantId;
        var commandWithIp = command with { IpAddress = ipAddress, TenantId = tenantId };
        var result = await mediator.Send(commandWithIp, ct);
        return Handle(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Handle(result);
    }

    [HttpPost("verify-two-factor")]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Handle(result);
    }
}
