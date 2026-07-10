using System.Net.NetworkInformation;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.Networking;

public sealed class SafeNetworkProbe : INetworkProbe
{
    public async Task<ProbeResult> PingAsync(string host, CancellationToken ct)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(
                host, TimeSpan.FromSeconds(2), cancellationToken: ct);
            return new(
                reply.Status == IPStatus.Success,
                reply.Status == IPStatus.Success ? reply.RoundtripTime : null,
                reply.Status.ToString());
        }
        catch (Exception ex)
        {
            return new(false, null, ex.Message);
        }
    }
}
