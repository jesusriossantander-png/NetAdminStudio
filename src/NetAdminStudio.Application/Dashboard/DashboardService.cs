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
    int HealthScore,
    string HealthLabel,
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

        var online = allAssets.Count(x => x.State == OperationalState.Online);
        var degraded = allAssets.Count(x => x.State == OperationalState.Degraded);
        var offline = allAssets.Count(x => x.State == OperationalState.Offline);

        var health = HealthScoreCalculator.Calculate(
            allAssets.Count, offline, degraded, openAlerts.Count);

        return new DashboardSnapshot(
            allAssets.Count,
            online,
            degraded,
            offline,
            allPrinters.Count,
            openAlerts.Count,
            health.Score,
            health.Label,
            DateTimeOffset.UtcNow);
    }
}
