# Reglas de Dependencias

## Dependencias permitidas

```text
Console -> Application
Application -> Core
Data -> Core
Communication -> Core
Platform.Windows -> Application/Core contracts
Agent -> Application + Platform.Windows + Communication
```

## Dependencias prohibidas

- Core -> cualquier proyecto externo.
- Application -> WPF.
- Console -> SQLite directo.
- Console -> PowerShell directo.
- Data -> Console.
- Agent -> Console.

## Verificación

Crear pruebas de arquitectura cuando sea posible.
