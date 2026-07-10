# Operación, despliegue y soporte

## Dependencias

- servidor NetAdminStudio;
- base SQLite;
- uno o más agentes;
- acceso de red autorizado;
- SNMP opcional;
- WMI/Windows Print Management opcional;
- CUPS/IPP opcional.

## Checklist operativo

- configurar rangos permitidos;
- definir exclusiones;
- validar DNS;
- registrar gateways;
- configurar retención;
- probar un escaneo pequeño;
- verificar jobs;
- validar permisos;
- probar auditoría;
- probar recuperación del agente;
- respaldar base y configuración.

## Diagnóstico

Registrar:

- correlationId;
- agentId;
- jobId;
- target;
- timeout;
- adapter;
- elapsedMs;
- errorCode.

Nunca registrar contraseñas, comunidades o tokens.
