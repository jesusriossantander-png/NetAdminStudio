namespace NetAdminStudio.Application.Dashboard;

/// <summary>Resultado del cálculo de salud: puntaje 0–100 y etiqueta descriptiva.</summary>
public sealed record HealthScoreResult(int Score, string Label);

/// <summary>
/// Calcula un puntaje de salud de la infraestructura (0–100) penalizando equipos
/// caídos/degradados y alertas abiertas. Lógica pura y testeable.
/// </summary>
public static class HealthScoreCalculator
{
    public static HealthScoreResult Calculate(
        int totalAssets, int offline, int degraded, int openAlerts)
    {
        if (totalAssets == 0)
            return new HealthScoreResult(100, "Sin datos");

        var score = 100
                    - offline * 8
                    - degraded * 4
                    - openAlerts * 5;

        score = Math.Clamp(score, 0, 100);

        var label = score switch
        {
            >= 90 => "Excelente",
            >= 75 => "Bueno",
            >= 50 => "Regular",
            _ => "Crítico"
        };

        return new HealthScoreResult(score, label);
    }
}
