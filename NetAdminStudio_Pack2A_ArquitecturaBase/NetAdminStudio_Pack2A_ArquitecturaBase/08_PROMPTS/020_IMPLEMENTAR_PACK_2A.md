# Prompt para Implementar Pack 2A

Leé toda la documentación del Pack 2A.

Tu tarea es crear la arquitectura base de la solución, sin implementar aún funciones avanzadas de red, impresoras, usuarios ni permisos.

## Entrega requerida

1. Crear `NetAdminStudio.sln`.
2. Crear proyectos:
   - NetAdmin.Console
   - NetAdmin.Application
   - NetAdmin.Core
   - NetAdmin.Data
   - NetAdmin.Communication
   - NetAdmin.Platform.Windows
   - NetAdmin.Reporting
   - NetAdmin.Agent
3. Configurar referencias según `DEPENDENCY_RULES.md`.
4. Activar nullable y analyzers.
5. Configurar DI.
6. Configurar Serilog.
7. Crear Result<T> y modelo de error.
8. Crear entidades base Asset y Event.
9. Crear navegación mínima en WPF.
10. Crear README técnico con comandos build/test.

## Restricciones

- No ejecutar PowerShell desde la UI.
- No acceder a SQLite desde ViewModels.
- No implementar todavía protocolos reales.
- No agregar librerías innecesarias.
- La solución debe compilar.
- Agregar pruebas básicas de Core y Application.

Antes de escribir código, devolvé:
- resumen de arquitectura,
- lista de archivos a crear,
- riesgos detectados,
- orden de implementación.
