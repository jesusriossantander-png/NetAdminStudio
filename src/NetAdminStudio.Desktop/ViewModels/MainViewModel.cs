using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
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
    private void ExportReport()
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"informe_{DateTime.Now:yyyyMMdd_HHmm}.html",
            Filter = "HTML (*.html)|*.html"
        };
        if (dialog.ShowDialog() != true)
            return;

        File.WriteAllText(dialog.FileName, BuildReportHtml(), Encoding.UTF8);
        Status = "Informe generado.";

        try
        {
            Process.Start(new ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
        }
        catch
        {
            // Si no se puede abrir el navegador, el archivo igual quedó guardado.
        }
    }

    private string BuildReportHtml()
    {
        static string E(string? v) => WebUtility.HtmlEncode(v ?? "");
        var sb = new StringBuilder();

        sb.Append("""
        <!doctype html><html lang="es"><head><meta charset="utf-8">
        <title>Informe NetAdmin Studio</title><style>
        body{font-family:Segoe UI,Arial,sans-serif;margin:32px;color:#111827;background:#f8fafc}
        h1{margin:0 0 4px}h2{margin:28px 0 8px;color:#0f172a;border-bottom:2px solid #38BDF8;padding-bottom:4px}
        .muted{color:#64748b}.cards{display:flex;gap:12px;flex-wrap:wrap;margin:16px 0}
        .card{background:#fff;border:1px solid #e2e8f0;border-radius:10px;padding:14px 18px;min-width:120px}
        .card b{font-size:26px;display:block}
        table{border-collapse:collapse;width:100%;background:#fff;font-size:13px}
        th,td{border:1px solid #e2e8f0;padding:6px 10px;text-align:left}
        th{background:#0f172a;color:#fff}tr:nth-child(even){background:#f1f5f9}
        </style></head><body>
        """);

        sb.Append($"<h1>NetAdmin Studio — Informe de infraestructura</h1>");
        sb.Append($"<p class='muted'>Generado el {E(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))}</p>");

        if (Dashboard is { } d)
        {
            sb.Append("<div class='cards'>");
            sb.Append($"<div class='card'>Activos<b>{d.Assets}</b></div>");
            sb.Append($"<div class='card'>Online<b>{d.Online}</b></div>");
            sb.Append($"<div class='card'>Offline<b>{d.Offline}</b></div>");
            sb.Append($"<div class='card'>Impresoras<b>{d.Printers}</b></div>");
            sb.Append($"<div class='card'>Alertas<b>{d.OpenAlerts}</b></div>");
            sb.Append($"<div class='card'>Health Score<b>{d.HealthScore}% {E(d.HealthLabel)}</b></div>");
            sb.Append("</div>");
        }

        sb.Append("<h2>Activos de red</h2><table><tr><th>IP</th><th>Nombre</th><th>Tipo</th><th>MAC</th><th>Fabricante</th><th>Puertos</th><th>Estado</th></tr>");
        foreach (var a in Assets)
            sb.Append($"<tr><td>{E(a.IpAddress)}</td><td>{E(a.Name)}</td><td>{E(a.TypeText)}</td><td>{E(a.MacAddress)}</td><td>{E(a.Vendor)}</td><td>{E(a.PortsText)}</td><td>{E(a.StateText)}</td></tr>");
        sb.Append("</table>");

        sb.Append("<h2>Impresoras</h2><table><tr><th>Nombre</th><th>Conexión</th><th>Modelo</th><th>Cola</th></tr>");
        foreach (var p in Printers)
            sb.Append($"<tr><td>{E(p.Name)}</td><td>{E(p.ConnectionType)}</td><td>{E(p.Model)}</td><td>{p.PendingJobs}</td></tr>");
        sb.Append("</table>");

        sb.Append("<h2>Permisos de carpetas compartidas</h2><table><tr><th>Recurso</th><th>Identidad</th><th>Permiso</th><th>Acceso</th></tr>");
        foreach (var perm in Permissions)
            sb.Append($"<tr><td>{E(perm.Share)}</td><td>{E(perm.Identity)}</td><td>{E(perm.Rights)}</td><td>{E(perm.Access)}</td></tr>");
        sb.Append("</table>");

        sb.Append("<h2>Usuarios locales</h2><table><tr><th>Usuario</th><th>Nombre completo</th><th>Estado</th></tr>");
        foreach (var u in Users)
            sb.Append($"<tr><td>{E(u.Name)}</td><td>{E(u.FullName)}</td><td>{E(u.StateText)}</td></tr>");
        sb.Append("</table>");

        sb.Append("</body></html>");
        return sb.ToString();
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
    public ObservableCollection<SharedFolderDto> Shares { get; } = [];
    public ObservableCollection<LocalUserDto> Users { get; } = [];
    public ObservableCollection<LocalGroupDto> Groups { get; } = [];
    public ObservableCollection<FolderPermissionDto> Permissions { get; } = [];
    public ObservableCollection<AuditEntryDto> Audit { get; } = [];

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

            Shares.Clear();
            foreach (var item in await apiClient.GetSharesAsync(cts.Token))
                Shares.Add(item);

            Users.Clear();
            foreach (var item in await apiClient.GetUsersAsync(cts.Token))
                Users.Add(item);

            Groups.Clear();
            foreach (var item in await apiClient.GetGroupsAsync(cts.Token))
                Groups.Add(item);

            Permissions.Clear();
            foreach (var item in await apiClient.GetSharePermissionsAsync(cts.Token))
                Permissions.Add(item);

            Audit.Clear();
            foreach (var item in await apiClient.GetAuditAsync(cts.Token))
                Audit.Add(item);

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
