# Configuración del Agent

## Configuración

```json
{
  "Agent": {
    "Port": 47820,
    "ConsoleAddresses": ["192.168.1.10"],
    "HeartbeatSeconds": 30,
    "InventoryMinutes": 15,
    "AllowRemoteActions": true
  },
  "Security": {
    "TokenReference": "dpapi://agent-token",
    "RequireHttps": false
  }
}
```

## Fuentes

- appsettings.json.
- appsettings.Production.json.
- Variables de entorno.
- Configuración protegida en ProgramData.

## Validación

El Agent no debe iniciar acciones remotas si la configuración de seguridad es inválida.
