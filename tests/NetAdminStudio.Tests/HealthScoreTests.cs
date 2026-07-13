using NetAdminStudio.Application.Dashboard;

namespace NetAdminStudio.Tests;

public sealed class HealthScoreTests
{
    [Fact]
    public void AllHealthy_ScoresExcellent()
    {
        var r = HealthScoreCalculator.Calculate(totalAssets: 10, offline: 0, degraded: 0, openAlerts: 0);
        Assert.Equal(100, r.Score);
        Assert.Equal("Excelente", r.Label);
    }

    [Fact]
    public void NoAssets_ReturnsNeutral()
    {
        var r = HealthScoreCalculator.Calculate(0, 0, 0, 0);
        Assert.Equal(100, r.Score);
        Assert.Equal("Sin datos", r.Label);
    }

    [Fact]
    public void SomeProblems_LowersScore()
    {
        // 100 - 2*8 - 1*4 - 3*5 = 65 → Regular
        var r = HealthScoreCalculator.Calculate(20, offline: 2, degraded: 1, openAlerts: 3);
        Assert.Equal(65, r.Score);
        Assert.Equal("Regular", r.Label);
    }

    [Fact]
    public void ManyProblems_ClampsToZeroCritical()
    {
        var r = HealthScoreCalculator.Calculate(30, offline: 20, degraded: 5, openAlerts: 10);
        Assert.Equal(0, r.Score);
        Assert.Equal("Crítico", r.Label);
    }
}
