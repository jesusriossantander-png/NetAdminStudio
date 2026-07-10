using System.Management;
using System.Runtime.Versioning;
using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Infrastructure.Printers;

/// <summary>
/// Lee las impresoras instaladas en Windows vía WMI (<c>Win32_Printer</c>) y cuenta los
/// trabajos pendientes por impresora (<c>Win32_PrintJob</c>). Solo observación.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class WmiPrinterProbe : IPrinterProbe
{
    public Task<IReadOnlyList<DiscoveredPrinter>> GetLocalPrintersAsync(CancellationToken ct)
    {
        if (!OperatingSystem.IsWindows())
            return Task.FromResult<IReadOnlyList<DiscoveredPrinter>>(Array.Empty<DiscoveredPrinter>());

        return Task.Run<IReadOnlyList<DiscoveredPrinter>>(() =>
        {
            var pendingByPrinter = CountPendingJobs();
            var result = new List<DiscoveredPrinter>();

            using var searcher = new ManagementObjectSearcher(
                "SELECT Name, DriverName, PortName, Shared, ShareName, PrinterStatus, WorkOffline FROM Win32_Printer");

            foreach (ManagementObject printer in searcher.Get())
            {
                ct.ThrowIfCancellationRequested();

                var name = printer["Name"]?.ToString() ?? "(sin nombre)";
                var model = printer["DriverName"]?.ToString();
                var portName = printer["PortName"]?.ToString();
                var shared = printer["Shared"] as bool? ?? false;
                var shareName = printer["ShareName"]?.ToString();
                var workOffline = printer["WorkOffline"] as bool? ?? false;
                var status = Convert.ToInt32(printer["PrinterStatus"] ?? 0);

                pendingByPrinter.TryGetValue(name, out var pending);

                result.Add(new DiscoveredPrinter(
                    name, model, portName, shared,
                    string.IsNullOrWhiteSpace(shareName) ? null : shareName,
                    MapState(status, workOffline),
                    pending));
            }

            return result;
        }, ct);
    }

    // Win32_Printer.PrinterStatus: 3=Idle, 4=Printing, 5=Warmup → operativa.
    private static OperationalState MapState(int printerStatus, bool workOffline)
    {
        if (workOffline) return OperationalState.Offline;
        return printerStatus is 3 or 4 or 5
            ? OperationalState.Online
            : OperationalState.Degraded;
    }

    // Win32_PrintJob.Name tiene el formato "NombreImpresora, JobId".
    private static Dictionary<string, int> CountPendingJobs()
    {
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_PrintJob");
            foreach (ManagementObject job in searcher.Get())
            {
                var raw = job["Name"]?.ToString();
                if (string.IsNullOrWhiteSpace(raw)) continue;

                var printerName = raw.Split(',')[0].Trim();
                counts[printerName] = counts.GetValueOrDefault(printerName) + 1;
            }
        }
        catch
        {
            // Sin acceso a la cola; se reporta 0 pendientes.
        }
        return counts;
    }
}
