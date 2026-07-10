using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetAdminStudio.Desktop.Services;

namespace NetAdminStudio.Desktop.ViewModels;

public partial class MainViewModel(NetAdminApiClient apiClient) : ObservableObject
{
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

    public bool CanScan => !IsScanning;

    public ObservableCollection<AssetDto> Assets { get; } = [];
    public ObservableCollection<PrinterDto> Printers { get; } = [];
    public ObservableCollection<AlertDto> Alerts { get; } = [];

    [RelayCommand]
    private async Task RefreshAsync()
    {
        try
        {
            Status = "Actualizando…";
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            Dashboard = await apiClient.GetDashboardAsync(cts.Token);

            Assets.Clear();
            foreach (var item in await apiClient.GetAssetsAsync(cts.Token))
                Assets.Add(item);

            Printers.Clear();
            foreach (var item in await apiClient.GetPrintersAsync(cts.Token))
                Printers.Add(item);

            Alerts.Clear();
            foreach (var item in await apiClient.GetAlertsAsync(cts.Token))
                Alerts.Add(item);

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
