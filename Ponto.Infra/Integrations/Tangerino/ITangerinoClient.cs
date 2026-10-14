namespace Ponto.Infra.Integrations.Tangerino;

public interface ITangerinoClient
{
    Task<bool> ClockIn(string employeeId, string pin, CancellationToken cancellationToken);
    Task<bool> SicronizeClockIn(string employeeId, string pin, string interpriseId, string bearerToken, CancellationToken cancellationToken);
}
