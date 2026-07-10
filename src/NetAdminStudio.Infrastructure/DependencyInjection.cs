using Microsoft.Extensions.DependencyInjection;
using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Infrastructure.Demo;
using NetAdminStudio.Infrastructure.Networking;
using NetAdminStudio.Infrastructure.Persistence;

namespace NetAdminStudio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton(new Database(connectionString));
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IPrinterRepository, PrinterRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IAutomationRepository, AutomationRepository>();
        services.AddSingleton<INetworkProbe, SafeNetworkProbe>();
        services.AddScoped<DemoDataSeeder>();
        return services;
    }
}
