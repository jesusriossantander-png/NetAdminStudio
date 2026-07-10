# Modelo de Datos

## Modelo conceptual

Asset es la entidad central.

```text
Asset
 ├── Agent
 ├── NetworkInterfaces
 ├── Printers
 ├── Shares
 ├── Users
 ├── Events
 ├── Incidents
 └── HealthSnapshots
```

## Identidad

Cada asset tiene:
- AssetId interno.
- Nombre.
- Tipo.
- IP actual.
- MAC principal.
- Último contacto.
- Estado.
