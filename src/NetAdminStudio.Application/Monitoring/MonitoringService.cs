using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Domain.Alerts;
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Application.Monitoring;

public sealed class MonitoringService(
    IAssetRepository assets,
    IAlertRepository alerts,
    INetworkProbe probe)
{
    public async Task MonitorOnceAsync(CancellationToken ct)
    {
        foreach (var asset in await assets.GetAllAsync(ct))
        {
            if (string.IsNullOrWhiteSpace(asset.IpAddress))
                continue;

            var wasOnline = asset.State == OperationalState.Online;
            var result = await probe.PingAsync(asset.IpAddress, ct);

            asset.RecordPresence(result.Success, result.DurationMs, DateTimeOffset.UtcNow);
            await assets.UpsertAsync(asset, ct);

            if (!result.Success && wasOnline)
            {
                await alerts.AddAsync(new Alert
                {
                    Title = $"{asset.Name} dejó de responder",
                    SourceType = "NetworkAsset",
                    SourceId = asset.Id,
                    Severity = AlertSeverity.Critical
                }, ct);
            }
        }
    }
}
