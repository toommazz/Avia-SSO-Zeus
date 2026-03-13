using Avia.SSO.Zeus.Application.Multitenancy.Commands.CreateTenant;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Avia.SSO.Zeus.Api.Controllers;

[Route("api/[controller]")]
public sealed class TenantsController(IMediator mediator) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return Handle(result);
    }
}
