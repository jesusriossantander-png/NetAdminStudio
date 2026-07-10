# Arquitectura de Navegación

## Menú principal

- Operations Center
- Red
- Equipos
- Impresoras
- Carpetas
- Usuarios
- Auditoría
- Informes
- Configuración

## Modelo

Navegación basada en rutas internas:

```text
/dashboard
/network
/assets
/assets/{id}
/printers
/shares
/users
/audit
/settings
```

## Reglas

- Mantener historial.
- Permitir volver.
- Preservar filtros al regresar.
- No usar ventanas nuevas salvo tareas especiales.
- Preferir panel lateral contextual.
