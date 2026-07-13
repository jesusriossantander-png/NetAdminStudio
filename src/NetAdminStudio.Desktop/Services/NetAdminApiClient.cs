using System.Net.Http;
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
    string? MacAddress,
    string? Hostname,
    string? Vendor,
    string? Model,
    string? Location,
    IReadOnlyList<int>? OpenPorts,
    string? Origin,
    DateTimeOffset? LastSeenAt,
    double? LatencyMs)
{
    public string PortsText => OpenPorts is { Count: > 0 } ? string.Join(", ", OpenPorts) : "";
    public string TypeText => Type switch
    {
        1 => "Router",
        2 => "Switch",
        3 => "Access Point",
        4 => "Firewall",
        5 => "Servidor",
        6 => "PC",
        7 => "Raspberry Pi",
        8 => "Cámara",
        9 => "Impresora",
        _ => "Desconocido"
    };
    public string StateText => State switch
    {
        1 => "Online",
        2 => "Degradado",
        3 => "Offline",
        4 => "Mantenimiento",
        _ => "Desconocido"
    };
}

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

public sealed record ScanStartedDto(Guid ScanId);

public sealed record PrinterScanResultDto(int Discovered);

public sealed record DiskDto(string Drive, double TotalGb, double FreeGb, int UsagePercent)
{
    public string Display => $"{Drive}  {UsagePercent}%  ({FreeGb:0.#} GB libres de {TotalGb:0.#} GB)";
}

public sealed record SystemInfoDto(
    string MachineName,
    string OperatingSystem,
    string? CpuModel,
    int CpuCores,
    int LogicalProcessors,
    int? CpuUsagePercent,
    double TotalMemoryGb,
    double UsedMemoryGb,
    int MemoryUsagePercent,
    int ServicesRunning,
    int ServicesTotal,
    IReadOnlyList<DiskDto> Disks)
{
    public string CpuText => CpuModel is null
        ? "CPU no disponible"
        : $"{CpuModel}  ·  {CpuCores} núcleos / {LogicalProcessors} hilos";
    public string CpuUsageText => CpuUsagePercent is null ? "—" : $"{CpuUsagePercent}%";
    public string MemoryText => $"{UsedMemoryGb:0.#} GB / {TotalMemoryGb:0.#} GB  ({MemoryUsagePercent}%)";
    public string ServicesText => $"{ServicesRunning} activos de {ServicesTotal}";
}

public sealed record ScanStatusDto(
    Guid Id,
    string Cidr,
    bool Finished,
    int Total,
    int Completed,
    int Found,
    string? Error);

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

    public async Task<Guid> StartScanAsync(string cidr, CancellationToken ct)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/network/scan", new { cidr }, ct);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<ScanStartedDto>(
            cancellationToken: ct)
            ?? throw new InvalidOperationException("La API no devolvió un identificador de escaneo.");
        return dto.ScanId;
    }

    public async Task<ScanStatusDto> GetScanStatusAsync(Guid id, CancellationToken ct) =>
        await httpClient.GetFromJsonAsync<ScanStatusDto>(
            $"/api/v1/network/scan/{id}", ct)
            ?? throw new InvalidOperationException("La API devolvió un estado vacío.");

    public async Task<int> ScanPrintersAsync(CancellationToken ct)
    {
        var response = await httpClient.PostAsync("/api/v1/printers/scan", null, ct);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<PrinterScanResultDto>(
            cancellationToken: ct);
        return dto?.Discovered ?? 0;
    }

    public async Task<SystemInfoDto> GetSystemInfoAsync(CancellationToken ct) =>
        await httpClient.GetFromJsonAsync<SystemInfoDto>("/api/v1/system/local", ct)
            ?? throw new InvalidOperationException("La API devolvió información vacía del sistema.");
}
