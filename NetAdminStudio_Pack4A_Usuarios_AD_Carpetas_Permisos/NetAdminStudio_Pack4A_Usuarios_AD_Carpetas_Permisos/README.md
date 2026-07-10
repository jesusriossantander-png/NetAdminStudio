# NetAdminStudio — Pack 4A
## Usuarios, Active Directory, Carpetas Compartidas y Permisos NTFS/SMB

Este pack define la arquitectura funcional, técnica, visual y de seguridad para administrar y auditar:

- usuarios locales;
- usuarios de dominio;
- grupos locales y de Active Directory;
- membresías;
- recursos compartidos SMB;
- carpetas;
- permisos NTFS;
- permisos de recurso compartido;
- herencia;
- propietarios;
- accesos efectivos;
- auditoría y remediación controlada.

## Principios

1. Leer antes de modificar.
2. Separar permisos NTFS de permisos SMB.
3. Mostrar permisos efectivos, no solo ACL crudas.
4. Preservar herencia y propietario salvo acción explícita.
5. Toda modificación debe ser previa, validada, reversible y auditada.
6. Nunca almacenar contraseñas en texto plano.
7. No ejecutar comandos arbitrarios.
8. El agente actúa con mínimo privilegio.
9. Las operaciones masivas se ejecutan como jobs.
10. Los cambios críticos requieren vista previa y confirmación.

## Alcance

### Usuarios y grupos
- inventario local;
- inventario de dominio;
- estado;
- membresías;
- bloqueo;
- expiración;
- última autenticación;
- cuentas inactivas;
- cuentas privilegiadas.

### Carpetas y recursos compartidos
- shares SMB;
- rutas locales;
- rutas UNC;
- host;
- disponibilidad;
- sesiones;
- archivos abiertos;
- cuota y espacio;
- propietario.

### Permisos
- ACL NTFS;
- ACL SMB;
- acceso efectivo;
- herencia;
- reglas explícitas;
- reglas heredadas;
- Allow y Deny;
- remediación;
- comparación entre carpetas.

## Fuera de alcance
- GPO completa;
- administración de Microsoft Entra ID;
- migración de perfiles;
- edición avanzada de esquema AD;
- PAM/JIT;
- DFS-R;
- administración de archivos en la nube.
