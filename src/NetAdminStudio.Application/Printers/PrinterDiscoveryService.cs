using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Domain.Printers;

namespace NetAdminStudio.Application.Printers;

/// <summary>
/// Descubre las impresoras reales del equipo y las persiste como <see cref="PrinterDevice"/>,
/// reutilizando el registro existente (por nombre) para conservar su identidad.
/// </summary>
public sealed class PrinterDiscoveryService(
    IPrinterProbe probe,
    IPrinterRepository printers)
{
    public async Task<int> DiscoverAsync(CancellationToken ct)
    {
        var found = await probe.GetLocalPrintersAsync(ct);
        var existing = await printers.GetAllAsync(ct);

        foreach (var p in found)
        {
            var device = existing.FirstOrDefault(x =>
                             string.Equals(x.Name, p.Name, StringComparison.OrdinalIgnoreCase))
                         ?? new PrinterDevice { Name = p.Name, ConnectionType = "WindowsLocal" };

            device.ConnectionType = p.Shared ? "WindowsShared" : "WindowsLocal";
            device.Model = p.Model;
            device.QueueName = p.ShareName ?? p.Name;
            device.State = p.State;
            device.PendingJobs = p.PendingJobs;
            device.LastSeenAt = DateTimeOffset.UtcNow;

            await printers.UpsertAsync(device, ct);
        }

        return found.Count;
    }
}
