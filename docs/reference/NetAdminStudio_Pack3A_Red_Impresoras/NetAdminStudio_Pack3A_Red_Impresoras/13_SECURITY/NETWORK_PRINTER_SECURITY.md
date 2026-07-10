# Seguridad — Red e Impresoras

## Amenazas

- escaneo no autorizado;
- abuso de herramientas diagnósticas;
- exposición de comunidades SNMP;
- modificación de colas;
- fuga de nombres de documentos;
- acceso a interfaces web;
- inyección de argumentos;
- falsificación de observaciones;
- descubrimiento de infraestructura sensible.

## Controles

- RBAC;
- scopes por sede/sector;
- allowlists de rangos;
- rate limiting;
- secretos protegidos con DPAPI;
- referencias a credenciales, no secretos en entidades;
- auditoría;
- validación de objetivos;
- salida de comandos sanitizada;
- mínimo privilegio;
- TLS cuando corresponda;
- rotación de credenciales;
- retención limitada de trabajos.

## Permisos sugeridos

- Network.View
- Network.EditMetadata
- Network.Scan
- Network.Diagnostics
- Network.WakeOnLan
- Network.ManageCredentials
- Printers.View
- Printers.EditMetadata
- Printers.ViewJobs
- Printers.ManageQueues
- Printers.ManageMaintenance
- Printers.ManageCredentials
