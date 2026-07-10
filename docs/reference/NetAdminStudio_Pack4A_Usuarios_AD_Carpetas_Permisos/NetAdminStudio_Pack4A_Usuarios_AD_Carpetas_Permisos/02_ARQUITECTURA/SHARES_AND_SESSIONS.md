# Recursos compartidos SMB, sesiones y archivos abiertos

## Share

- host;
- nombre;
- ruta local;
- ruta UNC;
- descripción;
- tipo;
- estado;
- disponibilidad;
- permisos SMB;
- permisos NTFS de la raíz;
- espacio total y libre;
- sesiones activas;
- archivos abiertos.

## Fuentes

- Win32_Share
- SMB Management APIs
- Get-SmbShare
- Get-SmbSession
- Get-SmbOpenFile

## Precauciones

- el acceso a sesiones y archivos abiertos requiere permisos elevados;
- los nombres de archivos pueden ser sensibles;
- retención limitada;
- visibilidad por permiso;
- no cerrar sesiones o archivos sin confirmación explícita.
