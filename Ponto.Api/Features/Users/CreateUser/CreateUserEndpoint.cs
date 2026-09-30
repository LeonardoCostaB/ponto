using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public class CreateUserEndpoint : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateUserEndpoint> _logger;

    public CreateUserEndpoint(ILogger<CreateUserEndpoint> logger, IMediator mediator)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(CreateUser), new { id = result }, result);
    }
}