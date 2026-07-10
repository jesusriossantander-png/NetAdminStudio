using Microsoft.Extensions.DependencyInjection;
using NetAdminStudio.Application.Assistant;
using NetAdminStudio.Application.Dashboard;
using NetAdminStudio.Application.Monitoring;
using NetAdminStudio.Application.Networking;

namespace NetAdminStudio.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<DashboardService>();
        services.AddScoped<MonitoringService>();
        services.AddScoped<OperationsAssistant>();
        services.AddScoped<NetworkDiscoveryEngine>();
        services.AddSingleton<ScanJobManager>();
        return services;
    }
}
