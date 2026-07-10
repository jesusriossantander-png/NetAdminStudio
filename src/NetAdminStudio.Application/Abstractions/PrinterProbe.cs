using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Application.Abstractions;

/// <summary>Una impresora detectada en el sistema operativo local.</summary>
public sealed record DiscoveredPrinter(
    string Name,
    string? Model,
    string? PortName,
    bool Shared,
    string? ShareName,
    OperationalState State,
    int PendingJobs);

/// <summary>Sondeo de las impresoras instaladas en el equipo (spooler de Windows).</summary>
public interface IPrinterProbe
{
    Task<IReadOnlyList<DiscoveredPrinter>> GetLocalPrintersAsync(CancellationToken ct);
}
