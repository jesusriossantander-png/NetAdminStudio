using NetAdminStudio.Domain.Alerts;
using NetAdminStudio.Domain.Assets;
using NetAdminStudio.Domain.Automation;
using NetAdminStudio.Domain.Printers;

namespace NetAdminStudio.Application.Abstractions;

public interface IAssetRepository
{
    Task<IReadOnlyList<NetworkAsset>> GetAllAsync(CancellationToken ct);
    Task UpsertAsync(NetworkAsset asset, CancellationToken ct);
}

public interface IPrinterRepository
{
    Task<IReadOnlyList<PrinterDevice>> GetAllAsync(CancellationToken ct);
    Task UpsertAsync(PrinterDevice printer, CancellationToken ct);
}

public interface IAlertRepository
{
    Task<IReadOnlyList<Alert>> GetOpenAsync(CancellationToken ct);
    Task<Alert?> GetAsync(Guid id, CancellationToken ct);
    Task AddAsync(Alert alert, CancellationToken ct);
    Task SaveAsync(Alert alert, CancellationToken ct);
}

public interface IAutomationRepository
{
    Task<IReadOnlyList<AutomationRule>> GetAllAsync(CancellationToken ct);
    Task AddAsync(AutomationRule rule, CancellationToken ct);
}

public interface INetworkProbe
{
    Task<ProbeResult> PingAsync(string host, CancellationToken ct);
}

public sealed record ProbeResult(bool Success, double? DurationMs, string? Error);
