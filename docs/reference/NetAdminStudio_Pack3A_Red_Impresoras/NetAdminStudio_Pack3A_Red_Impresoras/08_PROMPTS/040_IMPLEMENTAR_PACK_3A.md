# Prompt — Implementar Pack 3A

Implementá el Pack 3A de NetAdminStudio respetando estrictamente los Packs 1, 2A, 2B y 2C.

## Reglas obligatorias

- WPF + MVVM.
- Clean Architecture pragmática.
- SQLite para MVP.
- Result Pattern.
- Serilog.
- DPAPI para secretos.
- HTTP/JSON versionado.
- Jobs para operaciones largas.
- Auditoría permanente.
- Agente autónomo.
- Sin lógica de negocio en code-behind.
- Sin dependencias de infraestructura en Domain.
- Sin ejecución de comandos con argumentos libres.

## Módulos

### Red

Implementar inventario, interfaces, puertos, escaneo seguro, presencia, topología, métricas, alertas y diagnósticos.

### Impresoras

Implementar inventario, endpoints, colas, trabajos observados, consumibles, contadores, mantenimiento y alertas.

## Entregables

1. Domain.
2. Application.
3. Infrastructure.
4. Persistence.
5. API.
6. Console WPF.
7. Agent adapters.
8. Migraciones.
9. Tests.
10. Documentación.
11. Datos demo.

## Criterio

No simular integración real con SNMP/WMI/CUPS dentro del dominio. Crear adaptadores y fallbacks. Las funciones no disponibles deben informar `NotSupported`, no devolver éxito ficticio.
