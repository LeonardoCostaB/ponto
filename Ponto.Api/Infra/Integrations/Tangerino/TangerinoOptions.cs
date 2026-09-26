namespace Ponto.Api.Infra.Integrations.Tangerino;

public class TangerinoOptions
{
    public const string SectionName = "Tangerino";

    public required string BaseAddress { get; set; }
    public required string Origin { get; set; }
    public required string ClockInPath { get; set; }
    public required string SynchronizePath { get; set; }
}
