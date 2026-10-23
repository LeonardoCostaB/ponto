using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ponto.Api.Features.Users.ListUsers;

[ApiController]
[Route("api/users")]
public class ListUsersEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public ListUsersEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> ListUsers(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListUsersQuery(), cancellationToken);
        return Ok(result);
    }
}
