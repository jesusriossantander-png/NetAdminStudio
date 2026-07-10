# Capacidades del Agent

## Capability Model

El Agent declara capacidades disponibles:

- Inventory.Basic
- Inventory.Software
- Printer.Read
- Printer.Manage
- Network.Read
- Network.Manage
- Share.Read
- Share.Manage
- User.Read
- User.Manage
- Service.Read
- Service.Manage
- FilePermission.Read
- FilePermission.Manage

## Motivo

Permite:
- Compatibilidad entre versiones.
- Agents con permisos limitados.
- Plugins futuros.
- UI adaptativa.

## Respuesta

La Console debe ocultar o deshabilitar acciones no soportadas.
