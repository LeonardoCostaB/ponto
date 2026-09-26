namespace Ponto.Api.Infra.Integrations.Discord;

public class DiscordOptions
{
    public const string SectionName = "Discord";

    public required string BaseAddress { get; set; }
    public required string ChannelId { get; set; }
    public required string BotToken { get; set; }
}
