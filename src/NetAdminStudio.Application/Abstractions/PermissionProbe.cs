namespace NetAdminStudio.Application.Abstractions;

/// <summary>Una entrada de permiso NTFS sobre una carpeta compartida.</summary>
public sealed record FolderPermission(
    string Share,
    string Path,
    string Identity,
    string Rights,
    string Access);

/// <summary>Lee los permisos NTFS de las carpetas compartidas. Solo observación.</summary>
public interface IPermissionProbe
{
    Task<IReadOnlyList<FolderPermission>> GetSharePermissionsAsync(CancellationToken ct);
}
