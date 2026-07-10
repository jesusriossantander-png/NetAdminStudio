using System.Diagnostics;
using System.Text.RegularExpressions;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.Networking;

/// <summary>
/// Lee la tabla ARP del sistema ejecutando <c>arp -a</c>. El parseo es puro y testeable.
/// </summary>
public sealed partial class ArpTable : IArpTable
{
    [GeneratedRegex(@"(\d{1,3}(?:\.\d{1,3}){3})\s+([0-9a-fA-F]{2}(?:[-:][0-9a-fA-F]{2}){5})")]
    private static partial Regex LineRegex();

    public static IReadOnlyDictionary<string, string> Parse(string arpOutput)
    {
        var map = new Dictionary<string, string>();
        foreach (Match m in LineRegex().Matches(arpOutput))
        {
            var ip = m.Groups[1].Value;
            var mac = m.Groups[2].Value.Replace(':', '-').ToUpperInvariant();
            if (mac is "FF-FF-FF-FF-FF-FF" or "00-00-00-00-00-00")
                continue;
            if (ip.EndsWith(".255"))
                continue;
            map[ip] = mac;
        }
        return map;
    }

    public async Task<IReadOnlyDictionary<string, string>> GetAllAsync(CancellationToken ct)
    {
        try
        {
            var psi = new ProcessStartInfo("arp", "-a")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);
            if (process is null)
                return new Dictionary<string, string>();

            var output = await process.StandardOutput.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);
            return Parse(output);
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}
