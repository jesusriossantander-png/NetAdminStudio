using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Application.Assistant;

public sealed record AssistantAnswer(string Text, IReadOnlyList<string> SuggestedActions);

public sealed class OperationsAssistant(
    IAssetRepository assets,
    IPrinterRepository printers,
    IAlertRepository alerts)
{
    public async Task<AssistantAnswer> AskAsync(string question, CancellationToken ct)
    {
        var q = question.Trim().ToLowerInvariant();
        var allAssets = await assets.GetAllAsync(ct);
        var allPrinters = await printers.GetAllAsync(ct);
        var openAlerts = await alerts.GetOpenAsync(ct);

        if (q.Contains("offline") || q.Contains("caído"))
        {
            var offline = allAssets.Where(x => x.State == OperationalState.Offline).ToList();
            var text = offline.Count == 0
                ? "No hay activos marcados como offline."
                : $"Hay {offline.Count} activos offline: {string.Join(", ", offline.Select(x => x.Name))}.";
            return new(text, ["Abrir activos offline", "Ejecutar monitoreo"]);
        }

        if (q.Contains("impres"))
        {
            var problems = allPrinters
                .Where(x => x.State is OperationalState.Offline or OperationalState.Degraded)
                .ToList();

            var text = problems.Count == 0
                ? "Las impresoras registradas no presentan problemas."
                : $"Hay {problems.Count} impresoras con problemas: {string.Join(", ", problems.Select(x => x.Name))}.";
            return new(text, ["Abrir impresoras", "Revisar colas"]);
        }

        if (q.Contains("alert"))
            return new($"Hay {openAlerts.Count} alertas abiertas.", ["Abrir alertas"]);

        return new(
            $"Resumen: {allAssets.Count} activos, {allPrinters.Count} impresoras y {openAlerts.Count} alertas abiertas.",
            ["Abrir dashboard", "Ver alertas"]);
    }
}
