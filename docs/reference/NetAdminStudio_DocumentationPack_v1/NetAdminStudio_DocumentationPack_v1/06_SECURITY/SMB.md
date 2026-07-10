# Permisos SMB

## SMB

Define acceso al recurso compartido desde la red.

## Reglas recomendadas

- SMB puede ser más general.
- NTFS define granularidad fina.
- Evitar Everyone/Todos con Control Total.
- Usar grupos.

## Acciones

- Get-SmbShare
- Get-SmbShareAccess
- Grant-SmbShareAccess
- Revoke-SmbShareAccess
