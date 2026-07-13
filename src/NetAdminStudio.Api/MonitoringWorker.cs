using NetAdminStudio.Application.Monitoring;

namespace NetAdminStudio.Api;

/// <summary>
/// Servicio en segundo plano que re-sondea periódicamente los activos de red
/// (ping) y genera alertas cuando un equipo deja de responder.
/// </summary>
public sealed class MonitoringWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<MonitoringWorker> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(60);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Espera inicial para dejar que la API termine de arrancar.
        try { await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken); }
        catch (OperationCanceledException) { return; }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var monitor = scope.ServiceProvider.GetRequiredService<MonitoringService>();
                await monitor.MonitorOnceAsync(stoppingToken);
                logger.LogInformation("Ciclo de monitoreo completado.");
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Falló un ciclo de monitoreo.");
            }

            try { await Task.Delay(Interval, stoppingToken); }
            catch (OperationCanceledException) { break; }
        }
    }
}
