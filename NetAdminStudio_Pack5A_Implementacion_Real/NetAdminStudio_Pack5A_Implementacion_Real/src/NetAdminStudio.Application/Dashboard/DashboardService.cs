using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Application.Dashboard;

public sealed record DashboardSnapshot(
    int Assets,
    int Online,
    int Degraded,
    int Offline,
    int Printers,
    int OpenAlerts,
    DateTimeOffset GeneratedAt);

public sealed class DashboardService(
    IAssetRepository assets,
    IPrinterRepository printers,
    IAlertRepository alerts)
{
    public async Task<DashboardSnapshot> GetAsync(CancellationToken ct)
    {
        var allAssets = await assets.GetAllAsync(ct);
        var allPrinters = await printers.GetAllAsync(ct);
        var openAlerts = await alerts.GetOpenAsync(ct);

        return new DashboardSnapshot(
            allAssets.Count,
            allAssets.Count(x => x.State == OperationalState.Online),
            allAssets.Count(x => x.State == OperationalState.Degraded),
            allAssets.Count(x => x.State == OperationalState.Offline),
            allPrinters.Count,
            openAlerts.Count,
            DateTimeOffset.UtcNow);
    }
}
