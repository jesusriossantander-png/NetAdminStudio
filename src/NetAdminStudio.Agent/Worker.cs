namespace NetAdminStudio.Agent;

public sealed class Worker(
    ILogger<Worker> logger,
    IHttpClientFactory clientFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = clientFactory.CreateClient("api");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await client.PostAsync(
                    "/api/v1/monitor/run-once",
                    null,
                    stoppingToken);

                logger.LogInformation(
                    "Monitoreo ejecutado. HTTP {StatusCode}",
                    response.StatusCode);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "La API no está disponible.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
