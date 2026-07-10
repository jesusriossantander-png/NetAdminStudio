# Funcionamiento del Agent

## Resumen

El Agent es un Windows Service.

## Responsabilidades

- Escuchar comandos.
- Validar autenticación.
- Ejecutar acciones locales.
- Reportar inventario.
- Enviar heartbeat.
- Registrar logs.

## Comandos iniciales

- Inventory.Get
- Printer.List
- Printer.Share
- Printer.ClearQueue
- Service.Restart
- Share.List
- User.List
- Network.GetConfig
