using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Application.Printers;
using NetAdminStudio.Domain.Assets;
using NetAdminStudio.Domain.Printers;

namespace NetAdminStudio.Tests;

public sealed class PrinterDiscoveryServiceTests
{
    private sealed class FakeProbe(IReadOnlyList<DiscoveredPrinter> printers) : IPrinterProbe
    {
        public Task<IReadOnlyList<DiscoveredPrinter>> GetLocalPrintersAsync(CancellationToken ct) =>
            Task.FromResult(printers);
    }

    private sealed class InMemoryPrinters : IPrinterRepository
    {
        public readonly List<PrinterDevice> Saved = new();

        public Task<IReadOnlyList<PrinterDevice>> GetAllAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<PrinterDevice>>(Saved);

        public Task UpsertAsync(PrinterDevice printer, CancellationToken ct)
        {
            Saved.RemoveAll(x => x.Id == printer.Id);
            Saved.Add(printer);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Discover_SharedPrinter_MapsToWindowsShared()
    {
        var repo = new InMemoryPrinters();
        var service = new PrinterDiscoveryService(
            new FakeProbe(new[]
            {
                new DiscoveredPrinter("HP Recepción", "HP LaserJet", "USB001",
                    Shared: true, ShareName: "HP_Recepcion", OperationalState.Online, PendingJobs: 3)
            }),
            repo);

        var count = await service.DiscoverAsync(CancellationToken.None);

        Assert.Equal(1, count);
        var saved = Assert.Single(repo.Saved);
        Assert.Equal("WindowsShared", saved.ConnectionType);
        Assert.Equal("HP LaserJet", saved.Model);
        Assert.Equal("HP_Recepcion", saved.QueueName);
        Assert.Equal(3, saved.PendingJobs);
        Assert.Equal(OperationalState.Online, saved.State);
    }

    [Fact]
    public async Task Discover_ExistingPrinter_UpdatesInPlaceByName()
    {
        var repo = new InMemoryPrinters();
        var original = new PrinterDevice
        {
            Name = "Epson Oficina",
            ConnectionType = "WindowsLocal",
            State = OperationalState.Offline
        };
        repo.Saved.Add(original);

        var service = new PrinterDiscoveryService(
            new FakeProbe(new[]
            {
                new DiscoveredPrinter("Epson Oficina", "Epson L3250", "IP_192.168.0.50",
                    Shared: false, ShareName: null, OperationalState.Online, PendingJobs: 0)
            }),
            repo);

        await service.DiscoverAsync(CancellationToken.None);

        var saved = Assert.Single(repo.Saved);
        Assert.Equal(original.Id, saved.Id);                 // conserva identidad
        Assert.Equal(OperationalState.Online, saved.State);   // actualizado
    }
}
