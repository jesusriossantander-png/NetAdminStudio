using NetAdminStudio.Application;
using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Application.Assistant;
using NetAdminStudio.Application.Dashboard;
using NetAdminStudio.Application.Monitoring;
using NetAdminStudio.Application.Networking;
using NetAdminStudio.Domain.Automation;
using NetAdminStudio.Infrastructure;
using NetAdminStudio.Infrastructure.Demo;
using NetAdminStudio.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");
Directory.CreateDirectory(dataDirectory);
var connectionString = $"Data Source={Path.Combine(dataDirectory, "netadminstudio.db")}";

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<Database>().Initialize();
    await scope.ServiceProvider
        .GetRequiredService<DemoDataSeeder>()
        .SeedAsync(CancellationToken.None);
}

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/v1/health",
    () => Results.Ok(new { status = "ok", utc = DateTimeOffset.UtcNow }));

app.MapGet("/api/v1/dashboard",
    async (DashboardService service, CancellationToken ct) =>
        Results.Ok(await service.GetAsync(ct)));

app.MapGet("/api/v1/assets",
    async (IAssetRepository repository, CancellationToken ct) =>
        Results.Ok(await repository.GetAllAsync(ct)));

app.MapGet("/api/v1/printers",
    async (IPrinterRepository repository, CancellationToken ct) =>
        Results.Ok(await repository.GetAllAsync(ct)));

app.MapGet("/api/v1/alerts",
    async (IAlertRepository repository, CancellationToken ct) =>
        Results.Ok(await repository.GetOpenAsync(ct)));

app.MapPost("/api/v1/alerts/{id:guid}/acknowledge",
    async (Guid id, IAlertRepository repository, CancellationToken ct) =>
    {
        var alert = await repository.GetAsync(id, ct);
        if (alert is null)
            return Results.NotFound();

        alert.Acknowledge();
        await repository.SaveAsync(alert, ct);
        return Results.NoContent();
    });

app.MapGet("/api/v1/automations",
    async (IAutomationRepository repository, CancellationToken ct) =>
        Results.Ok(await repository.GetAllAsync(ct)));

app.MapPost("/api/v1/automations",
    async (AutomationRule rule, IAutomationRepository repository, CancellationToken ct) =>
    {
        await repository.AddAsync(rule, ct);
        return Results.Created($"/api/v1/automations/{rule.Id}", rule);
    });

app.MapPost("/api/v1/monitor/run-once",
    async (MonitoringService service, CancellationToken ct) =>
    {
        await service.MonitorOnceAsync(ct);
        return Results.Accepted();
    });

app.MapPost("/api/v1/assistant/ask",
    async (AssistantRequest request, OperationsAssistant assistant, CancellationToken ct) =>
        Results.Ok(await assistant.AskAsync(request.Question, ct)));

app.MapPost("/api/v1/network/scan",
    (NetworkScanRequest request, ScanJobManager jobs, NetworkDiscoveryEngine engine) =>
    {
        try
        {
            // Valida el CIDR temprano (lanza si es inválido o demasiado grande).
            _ = Cidr.Hosts(request.Cidr);
            var id = jobs.Start(request.Cidr, engine);
            return Results.Accepted($"/api/v1/network/scan/{id}", new { scanId = id });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    });

app.MapGet("/api/v1/network/scan/{id:guid}",
    (Guid id, ScanJobManager jobs) =>
    {
        var status = jobs.Get(id);
        return status is null ? Results.NotFound() : Results.Ok(status);
    });

app.Run();

public sealed record AssistantRequest(string Question);
public sealed record NetworkScanRequest(string Cidr);
