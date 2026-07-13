using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetAdminStudio.Desktop.Services;

namespace NetAdminStudio.Desktop.ViewModels;

public partial class MainViewModel(NetAdminApiClient apiClient) : ObservableObject
{
    private DispatcherTimer? _autoRefreshTimer;

    /// <summary>Inicia el refresco automático del dashboard (cada 30 s).</summary>
    public void StartAutoRefresh()
    {
        if (_autoRefreshTimer is not null) return;
        _autoRefreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        _autoRefreshTimer.Tick += async (_, _) => await RefreshAsync();
        _autoRefreshTimer.Start();
    }

    [ObservableProperty]
    private DashboardDto? dashboard;

    [ObservableProperty]
    private string status = "Conectando con la API…";

    [ObservableProperty]
    private string assistantQuestion = "";

    [ObservableProperty]
    private string assistantAnswer =
        "Preguntá por activos offline, impresoras o alertas.";

    [ObservableProperty]
    private string scanCidr = "192.168.0.0/24";

    [ObservableProperty]
    private string scanStatusText = "Listo para escanear la red.";

    [ObservableProperty]
    private string printerStatusText = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanScan))]
    private bool isScanning;

    [ObservableProperty]
    private SystemInfoDto? localSystem;

    [ObservableProperty]
    private string selectedSection = "dashboard";

    public bool CanScan => !IsScanning;

    [RelayCommand]
    private void Navigate(string section) => SelectedSection = section;

    [RelayCommand]
    private void ExportInventory()
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"inventario_{DateTime.Now:yyyyMMdd_HHmm}.csv",
            Filter = "CSV (*.csv)|*.csv"
        };
        if (dialog.ShowDialog() != true)
            return;

        var sb = new StringBuilder();
        sb.AppendLine("IP,Nombre,Tipo,MAC,Fabricante,Puertos,Estado,Origen");
        foreach (var a in Assets)
            sb.AppendLine(string.Join(",", new[]
            {
                Csv(a.IpAddress), Csv(a.Name), Csv(a.TypeText), Csv(a.MacAddress),
                Csv(a.Vendor), Csv(a.PortsText), Csv(a.StateText), Csv(a.Origin)
            }));

        File.WriteAllText(dialog.FileName, sb.ToString(), Encoding.UTF8);
        Status = $"Inventario exportado ({Assets.Count} activos).";
    }

    private static string Csv(string? value)
    {
        value ??= "";
        return value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
    }

    [RelayCommand]
    private async Task AcknowledgeAlertAsync(Guid id)
    {
        try
        {
            await apiClient.AcknowledgeAlertAsync(id, CancellationToken.None);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            Status = $"No se pudo reconocer la alerta: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ResolveAlertAsync(Guid id)
    {
        try
        {
            await apiClient.ResolveAlertAsync(id, CancellationToken.None);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            Status = $"No se pudo resolver la alerta: {ex.Message}";
        }
    }

    public ObservableCollection<AssetDto> Assets { get; } = [];
    public ObservableCollection<AssetDto> Computers { get; } = [];
    public ObservableCollection<PrinterDto> Printers { get; } = [];
    public ObservableCollection<AlertDto> Alerts { get; } = [];
    public ObservableCollection<AutomationDto> Automations { get; } = [];

    [RelayCommand]
    private async Task RefreshAsync()
    {
        try
        {
            Status = "Actualizando…";
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            Dashboard = await apiClient.GetDashboardAsync(cts.Token);

            Assets.Clear();
            Computers.Clear();
            foreach (var item in await apiClient.GetAssetsAsync(cts.Token))
            {
                Assets.Add(item);
                // Equipos = servidores, workstations, Raspberry Pi.
                if (item.Type is 5 or 6 or 7)
                    Computers.Add(item);
            }

            Printers.Clear();
            foreach (var item in await apiClient.GetPrintersAsync(cts.Token))
                Printers.Add(item);

            Alerts.Clear();
            foreach (var item in await apiClient.GetAlertsAsync(cts.Token))
                Alerts.Add(item);

            LocalSystem = await apiClient.GetSystemInfoAsync(cts.Token);

            Automations.Clear();
            foreach (var item in await apiClient.GetAutomationsAsync(cts.Token))
                Automations.Add(item);

            Status = $"Actualizado {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            Status = $"No se pudo conectar: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task AskAssistantAsync()
    {
        if (string.IsNullOrWhiteSpace(AssistantQuestion))
            return;

        try
        {
            var answer = await apiClient.AskAsync(
                AssistantQuestion,
                CancellationToken.None);

            AssistantAnswer = answer.Text;
        }
        catch (Exception ex)
        {
            AssistantAnswer = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ScanAsync()
    {
        if (IsScanning || string.IsNullOrWhiteSpace(ScanCidr))
            return;

        try
        {
            IsScanning = true;
            ScanStatusText = $"Iniciando escaneo de {ScanCidr}…";
            var id = await apiClient.StartScanAsync(ScanCidr, CancellationToken.None);

            while (true)
            {
                await Task.Delay(1000);
                var st = await apiClient.GetScanStatusAsync(id, CancellationToken.None);
                ScanStatusText =
                    $"Escaneando {st.Cidr}: {st.Completed}/{st.Total} " +
                    $"({st.Found} encontrados)";

                if (st.Finished)
                {
                    ScanStatusText = st.Error is null
                        ? $"Escaneo completo: {st.Found} dispositivos encontrados."
                        : $"Error en el escaneo: {st.Error}";
                    break;
                }
            }

            await RefreshAsync();   // recarga la grilla con lo descubierto
        }
        catch (Exception ex)
        {
            ScanStatusText = $"No se pudo escanear: {ex.Message}";
        }
        finally
        {
            IsScanning = false;
        }
    }

    [RelayCommand]
    private async Task ScanPrintersAsync()
    {
        try
        {
            PrinterStatusText = "Detectando impresoras…";
            var count = await apiClient.ScanPrintersAsync(CancellationToken.None);
            PrinterStatusText = $"{count} impresoras detectadas.";
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            PrinterStatusText = $"Error: {ex.Message}";
        }
    }
}
