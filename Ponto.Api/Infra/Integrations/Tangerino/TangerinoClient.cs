using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Ponto.Api.Infra.Integrations.Discord;

namespace Ponto.Api.Infra.Integrations.Tangerino;

public class TangerinoClient : ITangerinoClient
{
    private readonly HttpClient _httpClient;
    private readonly TangerinoOptions _options;
    private readonly ILogger<TangerinoClient> _logger;
    private readonly IDiscordClient _discordClient;

    public TangerinoClient(HttpClient httpClient, IOptions<TangerinoOptions> options, ILogger<TangerinoClient> logger, IDiscordClient discordClient)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        _discordClient = discordClient;
    }

    public async Task<bool> ClockIn(string employeeId, string pin, CancellationToken cancellationToken)
    {
        var emptyDevicePayload = JsonSerializer.Serialize(new { deviceId = (string?)null });

        try
        {
            var clockInPath = _options.ClockInPath
                .Replace("{employeeId}", Uri.EscapeDataString(employeeId), StringComparison.Ordinal)
                .Replace("{pin}", Uri.EscapeDataString(pin), StringComparison.Ordinal);
            var clockInUrl = new Uri(new Uri(_options.BaseAddress), clockInPath);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, clockInUrl);
            httpRequest.Headers.TryAddWithoutValidation("empregador", employeeId);
            httpRequest.Headers.TryAddWithoutValidation("pin", pin);
            httpRequest.Headers.TryAddWithoutValidation("origin", _options.Origin);
            httpRequest.Content = new StringContent(
                emptyDevicePayload,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                await _discordClient.SendMessageAsync($"User {employeeId} clocked in successfully");
                return true;
            }

            _logger.LogError("Failed to clock in user {employeeId}: {StatusCode}", employeeId, response.StatusCode);
            await _discordClient.SendMessageAsync($"Failed to clock in user {employeeId}: {response.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clock in user {employeeId}", employeeId);
            return false;
        }
    }

    public async Task<bool> SicronizeClockIn(string employeeId, string pin, string interpriseId, CancellationToken cancellationToken)
    {
        try
        {
            var synchronizeUrl = new Uri(new Uri(_options.BaseAddress), _options.SynchronizePath);

            var synchronizePayload = JsonSerializer.Serialize(new
            {
                horaInicio = "",
                deviceId = (string?)null,
                offline = "false",
                horaFim = "",
                tipo = "WEB",
                intervalo = "",
                validFingerprint = false,
                versao = "registra-ponto-fingerprint",
                plataforma = "WEB",
                funcionarioId = employeeId,
                recaptcha = (string?)null,
                idAtividade = 6,
                latitude = (string?)null,
                longitude = (string?)null,
                pin,
                foto = "",
                codigoEmpregador = interpriseId,
            });

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, synchronizeUrl);
            httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_options.Authorization}");
            httpRequest.Headers.TryAddWithoutValidation("empregador", employeeId);
            httpRequest.Headers.TryAddWithoutValidation("pin", pin);
            httpRequest.Headers.TryAddWithoutValidation("funcionarioid", interpriseId);
            httpRequest.Headers.TryAddWithoutValidation("origin", _options.Origin);
            httpRequest.Content = new StringContent(
                synchronizePayload,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                await _discordClient.SendMessageAsync($"User {employeeId} synchronized clock in successfully");
                return true;
            }

            _logger.LogError("Failed to synchronize clock in user {employeeId}: {StatusCode}", employeeId, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to synchronize clock in user {employeeId}", employeeId);
            return false;
        }
    }
}
