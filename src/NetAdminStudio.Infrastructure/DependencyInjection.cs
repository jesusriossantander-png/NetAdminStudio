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
        services.AddScoped<IAuditLog, SqliteAuditLog>();
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IPrinterRepository, PrinterRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IAutomationRepository, AutomationRepository>();
        services.AddSingleton<INetworkProbe, SafeNetworkProbe>();
        services.AddSingleton<INetworkScanner, TcpPortScanner>();
        services.AddSingleton<IArpTable, ArpTable>();
        services.AddSingleton<IVendorLookup, OuiVendorLookup>();
        services.AddSingleton<IPrinterProbe, Printers.WmiPrinterProbe>();
        services.AddSingleton<ISystemInfoProbe, System.WmiSystemInfoProbe>();
        services.AddSingleton<IShareProbe, System.WmiShareProbe>();
        services.AddSingleton<IUserProbe, System.WmiUserProbe>();
        services.AddSingleton<IGroupProbe, System.WmiGroupProbe>();
        services.AddSingleton<IPermissionProbe, System.NtfsPermissionProbe>();
        services.AddScoped<DemoDataSeeder>();
        return services;
    }
}
