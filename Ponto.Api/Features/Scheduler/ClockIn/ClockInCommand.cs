using MediatR;

namespace Ponto.Api.Features.Scheduler.ClockIn;

public class ClockInResponse
{
    public Guid UserId { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
}

public record ClockInCommand : IRequest<List<ClockInResponse>>;
