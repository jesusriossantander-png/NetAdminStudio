# Política de Inyección de Dependencias

## Contenedor

Usar Microsoft.Extensions.DependencyInjection.

## Lifetimes

- Singleton: configuración, reloj, caches controladas.
- Scoped: casos de uso y unidades de trabajo.
- Transient: ViewModels livianos y validadores.

## Regla

No usar Service Locator.
No resolver servicios manualmente desde ViewModels.
