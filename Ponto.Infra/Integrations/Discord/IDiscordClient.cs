namespace Ponto.Infra.Integrations.Discord;

public interface IDiscordClient
{
    Task SendMessageAsync(string message);
}