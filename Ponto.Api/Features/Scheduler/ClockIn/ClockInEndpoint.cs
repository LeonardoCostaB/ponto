using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ponto.Api.Features.Scheduler.ClockIn;

[ApiController]
[Route("api/scheduler/clock-in")]
public class ClockInEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public ClockInEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> ClockIn(ClockInCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}