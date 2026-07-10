# Guía operativa

## Puesta en marcha

1. Registrar dominios y hosts.
2. Definir credenciales referenciadas.
3. Verificar conectividad LDAP/SMB.
4. Ejecutar inventario de prueba.
5. Validar resolución de SID.
6. Escanear un share pequeño.
7. Revisar snapshots.
8. Probar acceso efectivo.
9. Ejecutar dry-run.
10. Probar rollback en entorno controlado.

## Diagnóstico

Registrar:

- agentId;
- hostId;
- jobId;
- path;
- principalSid;
- operation;
- result;
- correlationId.

No registrar:

- contraseñas;
- tokens;
- contenido de archivos;
- nombres sensibles sin protección.
