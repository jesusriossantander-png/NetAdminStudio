using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.Networking;

/// <summary>
/// Sondeo real de red: ping ICMP (con DNS inverso best-effort) y escaneo de puertos TCP
/// con timeouts cortos. Solo observación, nunca modifica el dispositivo.
/// </summary>
public sealed class TcpPortScanner : INetworkScanner
{
    private static readonly TimeSpan PingTimeout = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan TcpTimeout = TimeSpan.FromMilliseconds(500);

    public async Task<HostProbe> ProbeHostAsync(IPAddress ip, CancellationToken ct)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(ip, PingTimeout);
            if (reply.Status != IPStatus.Success)
                return new HostProbe(false, null, null);

            string? hostname = null;
            try
            {
                var entry = await Dns.GetHostEntryAsync(ip);
                hostname = entry.HostName;
            }
            catch
            {
                // Sin resolución DNS inversa; no es un error.
            }

            return new HostProbe(true, hostname, reply.RoundtripTime);
        }
        catch
        {
            return new HostProbe(false, null, null);
        }
    }

    public async Task<IReadOnlyList<int>> ScanPortsAsync(
        IPAddress ip, IReadOnlyList<int> ports, CancellationToken ct)
    {
        var open = new List<int>();
        foreach (var port in ports)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                using var client = new TcpClient();
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
                timeout.CancelAfter(TcpTimeout);
                await client.ConnectAsync(ip, port, timeout.Token);
                if (client.Connected)
                    open.Add(port);
            }
            catch
            {
                // Puerto cerrado, filtrado o timeout.
            }
        }
        return open;
    }
}
