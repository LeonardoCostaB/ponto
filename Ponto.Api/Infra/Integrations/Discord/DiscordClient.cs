using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Ponto.Api.Infra.Integrations.Discord;

public class DiscordClient : IDiscordClient
{
    private readonly DiscordOptions _options;
    private readonly ILogger<DiscordClient> _logger;
    private readonly HttpClient _httpClient;

    public DiscordClient(IOptions<DiscordOptions> options, ILogger<DiscordClient> logger, HttpClient httpClient)
    {
        _options = options.Value;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task SendMessageAsync(string message)
    {
        if (string.IsNullOrEmpty(_options.ChannelId) || string.IsNullOrEmpty(_options.BotToken))
        {
            _logger.LogError("Discord channel ID or bot token is not set");
            return;
        }

        var payload = new
        {
            content = $"Date: **{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}**\n\nMessage: **{message}**"
        };

        try
        {
            var discordUrl = new Uri(new Uri(_options.BaseAddress), $"channels/{_options.ChannelId}/messages");
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, discordUrl);
            httpRequest.Headers.TryAddWithoutValidation("Content-Type", "application/json");
            httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Bot {_options.BotToken}");
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to send Discord notification: {Response}", await response.Content.ReadAsStringAsync());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send Discord notification");
        }
    }
}
