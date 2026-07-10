using System.Net.Http.Json;

namespace NetAdminStudio.Desktop.Services;

public sealed record DashboardDto(
    int Assets,
    int Online,
    int Degraded,
    int Offline,
    int Printers,
    int OpenAlerts,
    DateTimeOffset GeneratedAt);

public sealed record AssetDto(
    Guid Id,
    string Name,
    int Type,
    int State,
    string? IpAddress,
    string? Vendor,
    string? Model,
    string? Location,
    DateTimeOffset? LastSeenAt,
    double? LatencyMs);

public sealed record PrinterDto(
    Guid Id,
    string Name,
    string ConnectionType,
    string? IpAddress,
    string? HostName,
    string? QueueName,
    string? Model,
    int State,
    int? BlackLevelPercent,
    int PendingJobs,
    DateTimeOffset? LastSeenAt);

public sealed record AlertDto(
    Guid Id,
    string Title,
    string SourceType,
    Guid SourceId,
    int Severity,
    int State,
    DateTimeOffset OpenedAt);

public sealed record AssistantAnswerDto(
    string Text,
    IReadOnlyList<string> SuggestedActions);

public sealed class NetAdminApiClient(HttpClient httpClient)
{
    public async Task<DashboardDto> GetDashboardAsync(CancellationToken ct) =>
        await httpClient.GetFromJsonAsync<DashboardDto>("/api/v1/dashboard", ct)
        ?? throw new InvalidOperationException("La API devolvió una respuesta vacía.");

    public async Task<List<AssetDto>> GetAssetsAsync(CancellationToken ct) =>
        await httpClient.GetFromJsonAsync<List<AssetDto>>("/api/v1/assets", ct) ?? [];

    public async Task<List<PrinterDto>> GetPrintersAsync(CancellationToken ct) =>
        await httpClient.GetFromJsonAsync<List<PrinterDto>>("/api/v1/printers", ct) ?? [];

    public async Task<List<AlertDto>> GetAlertsAsync(CancellationToken ct) =>
        await httpClient.GetFromJsonAsync<List<AlertDto>>("/api/v1/alerts", ct) ?? [];

    public async Task<AssistantAnswerDto> AskAsync(string question, CancellationToken ct)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/assistant/ask",
            new { question },
            ct);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AssistantAnswerDto>(
            cancellationToken: ct)
            ?? throw new InvalidOperationException("La API devolvió una respuesta vacía.");
    }
}
