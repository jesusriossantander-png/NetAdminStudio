# Gestión de Estado en la Console

## Estado global permitido

- Usuario actual.
- Tema visual.
- Equipo seleccionado.
- Filtros globales.
- Estado de conectividad.
- Notificaciones.

## Estado local

Cada ViewModel administra:
- Loading.
- Error.
- Items.
- Selección.
- Paginación.
- Filtros.

## Regla

Evitar singletons con estado mutable arbitrario.
Usar servicios explícitos y observables.
