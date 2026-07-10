using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Domain.Alerts;
using NetAdminStudio.Domain.Assets;
using NetAdminStudio.Domain.Automation;
using NetAdminStudio.Domain.Printers;

namespace NetAdminStudio.Infrastructure.Demo;

public sealed class DemoDataSeeder(
    IAssetRepository assets,
    IPrinterRepository printers,
    IAlertRepository alerts,
    IAutomationRepository automations)
{
    public async Task SeedAsync(CancellationToken ct)
    {
        if ((await assets.GetAllAsync(ct)).Count > 0)
            return;

        var router = new NetworkAsset
        {
            Name = "Router principal",
            Type = AssetType.Router,
            IpAddress = "192.168.0.1",
            Location = "Oficina"
        };
        router.RecordPresence(true, 2, DateTimeOffset.UtcNow);

        var switchDevice = new NetworkAsset
        {
            Name = "Switch SG3428",
            Type = AssetType.Switch,
            IpAddress = "192.168.0.10",
            Vendor = "TP-Link",
            Model = "TL-SG3428",
            Location = "Oficina"
        };
        switchDevice.RecordPresence(true, 4, DateTimeOffset.UtcNow);

        var cpe = new NetworkAsset
        {
            Name = "CPE Taller",
            Type = AssetType.AccessPoint,
            IpAddress = "192.168.0.3",
            Vendor = "TP-Link",
            Model = "CPE710",
            Location = "Taller"
        };
        cpe.MarkDegraded();

        await assets.UpsertAsync(router, ct);
        await assets.UpsertAsync(switchDevice, ct);
        await assets.UpsertAsync(cpe, ct);

        await printers.UpsertAsync(new PrinterDevice
        {
            Name = "Epson Oficina",
            ConnectionType = "RaspberryPiCups",
            IpAddress = "192.168.0.50",
            QueueName = "epson_oficina",
            Model = "Epson L3250",
            State = OperationalState.Online,
            BlackLevelPercent = 62,
            LastSeenAt = DateTimeOffset.UtcNow
        }, ct);

        await printers.UpsertAsync(new PrinterDevice
        {
            Name = "Brother Taller",
            ConnectionType = "WindowsShared",
            HostName = "PC-TALLER",
            QueueName = "Brother_Taller",
            Model = "Brother HL",
            State = OperationalState.Degraded,
            BlackLevelPercent = 14,
            PendingJobs = 2,
            LastSeenAt = DateTimeOffset.UtcNow
        }, ct);

        await alerts.AddAsync(new Alert
        {
            Title = "CPE Taller presenta degradación",
            SourceType = "NetworkAsset",
            SourceId = cpe.Id,
            Severity = AlertSeverity.Warning
        }, ct);

        await automations.AddAsync(new AutomationRule
        {
            Name = "Alertar dispositivo offline",
            TriggerType = "AssetStateChanged",
            ConditionJson = "{\"newState\":\"Offline\",\"durationSeconds\":120}",
            ActionType = "CreateAlert",
            ActionJson = "{\"severity\":\"Critical\"}"
        }, ct);
    }
}
