using System.Collections.Concurrent;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ponto.Api.Infra.Database;
using Ponto.Api.Infra.Integrations.Discord;
using Ponto.Api.Infra.Integrations.Tangerino;

namespace Ponto.Api.Features.Scheduler.ClockIn;

public class ClockInHandler : IRequestHandler<ClockInCommand, List<ClockInResponse>>
{
    private readonly AppDbContext _dbContext;
    private readonly ITangerinoClient _tangerinoClient;
    private readonly IDiscordClient _discordClient;

    public ClockInHandler(AppDbContext dbContext, ITangerinoClient tangerinoClient, IDiscordClient discordClient)
    {
        _dbContext = dbContext;
        _tangerinoClient = tangerinoClient;
        _discordClient = discordClient;
    }

    public async Task<List<ClockInResponse>> Handle(ClockInCommand request, CancellationToken cancellationToken)
    {
        var users = await _dbContext.Users.ToListAsync(cancellationToken);

        if (users.Count == 0)
        {
            return new List<ClockInResponse>();
        }

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 5,
            CancellationToken = cancellationToken
        };

        var result = new ConcurrentBag<(Guid UserId, string Message, bool Success)>();

        await Parallel.ForEachAsync(users, parallelOptions, async (user, token) =>
        {
            var employeeId = user.EmployeeId;
            var pin = user.pin;
            var interpriseId = user.companyId;

            var clockInResponse = await _tangerinoClient.ClockIn(employeeId, pin, token);

            if (clockInResponse)
            {
                var synchronizeResponse = await _tangerinoClient.SicronizeClockIn(employeeId, pin, interpriseId, token);
                if (synchronizeResponse)
                {
                    result.Add((user.Id, "Synchronize successful", true));
                }
                else
                {
                    result.Add((user.Id, "Synchronize failed", false));
                }
            }
            else
            {
                result.Add((user.Id, "Clock in failed", false));
            }
        });

        if (result.Count > 0)
        {
            var message = string.Join("\n", result.Select(r => $"{r.UserId}: {r.Message}"));
            await _discordClient.SendMessageAsync(message);
        }

        return result.Select(r => new ClockInResponse { UserId = r.UserId, Message = r.Message, Success = r.Success }).ToList();
    }
}