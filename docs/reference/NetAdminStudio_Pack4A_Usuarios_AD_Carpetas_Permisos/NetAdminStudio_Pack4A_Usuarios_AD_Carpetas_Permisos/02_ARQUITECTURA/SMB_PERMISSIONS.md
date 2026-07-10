# Permisos SMB

## Niveles

- Full
- Change
- Read
- Custom
- Deny

## Permiso efectivo combinado

El acceso real depende de:

1. permiso de recurso compartido;
2. permiso NTFS;
3. pertenencia a grupos;
4. reglas Allow y Deny;
5. token de acceso;
6. herencia;
7. privilegios del sistema.

La interfaz debe explicar el resultado, por ejemplo:

> El usuario posee Modify por NTFS, pero solo Read en el recurso compartido. Acceso efectivo: Read.
