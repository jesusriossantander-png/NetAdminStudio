# Remediación de permisos

## Operaciones admitidas

- agregar regla;
- quitar regla;
- reemplazar regla;
- cambiar herencia;
- copiar permisos;
- restaurar snapshot;
- cambiar propietario;
- modificar permiso SMB.

## Reglas de seguridad

- vista previa obligatoria;
- respaldo de ACL;
- dry-run;
- validación de SID;
- no aceptar comandos libres;
- no seguir junctions por defecto;
- límite de profundidad;
- cancelación;
- rollback;
- auditoría;
- verificación posterior.

## Operaciones masivas

Siempre se ejecutan como job con:

- lote;
- progreso;
- errores parciales;
- reintentos;
- idempotencia;
- exportación de resultado.
