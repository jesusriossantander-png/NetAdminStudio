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
}
