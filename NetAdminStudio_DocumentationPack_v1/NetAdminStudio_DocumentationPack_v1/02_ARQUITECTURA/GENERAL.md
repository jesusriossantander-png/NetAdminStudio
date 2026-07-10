# Arquitectura General

## Arquitectura general

La solución se divide en proyectos:

```text
/src
  /NetAdmin.Console
  /NetAdmin.Agent
  /NetAdmin.Core
  /NetAdmin.Data
  /NetAdmin.Communication
  /NetAdmin.Installer
  /NetAdmin.Updater
/tests
/docs
```

## Reglas

- Console no ejecuta comandos remotos directamente salvo funciones sin agente.
- Agent ejecuta acciones locales en su PC.
- Core contiene modelos y contratos.
- Data contiene persistencia SQLite.
- Communication contiene protocolo, DTOs y autenticación.

## Conceptos principales

- Asset: cualquier activo de infraestructura.
- Agent: servicio instalado en un Asset administrable.
- Command: solicitud de acción remota.
- Event: registro de algo ocurrido.
- Incident: problema detectado.
- Policy: regla deseada.
- Health Score: indicador de salud.

## Flujo

1. Console descubre asset.
2. Verifica agente.
3. Agent reporta inventario.
4. Console guarda en SQLite.
5. Usuario ejecuta acción.
6. Console envía Command.
7. Agent ejecuta.
8. Agent responde.
9. Console registra Event.
