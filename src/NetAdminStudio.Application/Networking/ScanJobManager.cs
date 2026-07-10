using System.Collections.Concurrent;

namespace NetAdminStudio.Application.Networking;

/// <summary>Estado de un trabajo de escaneo, consultable mientras corre.</summary>
public sealed record ScanStatus(
    Guid Id, string Cidr, bool Finished,
    int Total, int Completed, int Found, string? Error);

/// <summary>
/// Lanza escaneos en segundo plano y expone su progreso. Singleton en memoria.
/// El motor se pasa por parámetro (resuelto del scope de la request) para evitar
/// problemas de scope de inyección de dependencias.
/// </summary>
public sealed class ScanJobManager
{
    private readonly ConcurrentDictionary<Guid, ScanStatus> _jobs = new();

    public Guid Start(string cidr, NetworkDiscoveryEngine engine)
    {
        var id = Guid.NewGuid();
        _jobs[id] = new ScanStatus(id, cidr, false, 0, 0, 0, null);

        _ = Task.Run(async () =>
        {
            try
            {
                var progress = new Progress<ScanProgress>(p =>
                    _jobs[id] = _jobs[id] with
                    {
                        Total = p.Total,
                        Completed = p.Completed,
                        Found = p.Found
                    });

                await engine.ScanAsync(cidr, progress, CancellationToken.None);
                _jobs[id] = _jobs[id] with { Finished = true };
            }
            catch (Exception ex)
            {
                _jobs[id] = _jobs[id] with { Finished = true, Error = ex.Message };
            }
        });

        return id;
    }

    public ScanStatus? Get(Guid id) => _jobs.TryGetValue(id, out var s) ? s : null;
}
