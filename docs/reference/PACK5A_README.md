# NetAdminStudio — Pack 5A: Implementación real

Primera base ejecutable de NetAdminStudio.

## Incluye

- .NET 8.
- ASP.NET Core Minimal API.
- Aplicación WPF con MVVM.
- Worker Agent.
- SQLite.
- Arquitectura Domain / Application / Infrastructure.
- Dashboard NOC.
- Inventario básico de red.
- Impresoras.
- Alertas.
- Automatizaciones.
- Asistente operativo local basado en reglas.
- Datos de demostración.
- Pruebas unitarias.
- Scripts PowerShell.

## Requisitos

- Windows 10 u 11.
- .NET 8 SDK.
- Visual Studio 2022 17.8+ o VS Code con C# Dev Kit.
- PowerShell 7 recomendado.

## Ejecución

```powershell
.\scripts\restore.ps1
.\scripts\build.ps1
.\scripts\run-all.ps1
```

Servicios:

- API: http://localhost:5188
- Swagger: http://localhost:5188/swagger
- Desktop WPF: consume la API local.
- Agent: ejecuta monitoreo cada 30 segundos.

## Estado de esta entrega

Es una base funcional y ampliable. El entorno de generación no posee el SDK de .NET, por lo que no fue posible compilarla aquí. Se incluye un script de validación para ejecutarlo en tu PC.

## Seguridad

Este MVP no tiene autenticación real. No publiques el puerto 5188 directamente en Internet.
