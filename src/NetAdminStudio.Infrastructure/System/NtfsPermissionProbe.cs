using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.System;

/// <summary>
/// Lee los permisos NTFS de las carpetas compartidas (quién tiene acceso a cada recurso).
/// Solo observación. Reutiliza el sondeo de recursos compartidos para saber qué carpetas mirar.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class NtfsPermissionProbe(IShareProbe shareProbe) : IPermissionProbe
{
    public async Task<IReadOnlyList<FolderPermission>> GetSharePermissionsAsync(CancellationToken ct)
    {
        if (!OperatingSystem.IsWindows())
            return Array.Empty<FolderPermission>();

        var shares = await shareProbe.GetLocalSharesAsync(ct);

        return await Task.Run<IReadOnlyList<FolderPermission>>(() =>
        {
            var result = new List<FolderPermission>();

            foreach (var share in shares)
            {
                ct.ThrowIfCancellationRequested();

                // Solo carpetas reales (no impresoras, IPC ni recursos administrativos).
                if (string.IsNullOrWhiteSpace(share.Path)
                    || share.ShareType.Contains("administrativo")
                    || !share.ShareType.StartsWith("Carpeta")
                    || !Directory.Exists(share.Path))
                    continue;

                try
                {
                    var info = new DirectoryInfo(share.Path);
                    var security = info.GetAccessControl();
                    var rules = security.GetAccessRules(true, true, typeof(NTAccount));

                    foreach (FileSystemAccessRule rule in rules)
                    {
                        result.Add(new FolderPermission(
                            share.Name,
                            share.Path!,
                            rule.IdentityReference.Value,
                            DescribeRights(rule.FileSystemRights),
                            rule.AccessControlType == AccessControlType.Allow ? "Permitir" : "Denegar"));
                    }
                }
                catch
                {
                    // Sin acceso a la ACL de esa carpeta; se omite.
                }
            }

            return result;
        }, ct);
    }

    private static string DescribeRights(FileSystemRights rights)
    {
        if (rights.HasFlag(FileSystemRights.FullControl)) return "Control total";
        if (rights.HasFlag(FileSystemRights.Modify)) return "Modificar";
        if (rights.HasFlag(FileSystemRights.ReadAndExecute)) return "Lectura y ejecución";
        if (rights.HasFlag(FileSystemRights.Read)) return "Lectura";
        if (rights.HasFlag(FileSystemRights.Write)) return "Escritura";
        return rights.ToString();
    }
}
