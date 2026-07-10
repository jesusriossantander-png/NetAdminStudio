# Arquitectura

```text
Desktop WPF
    |
    v
ASP.NET Core API
    |
    v
Application
    |
    v
Domain
    ^
    |
Infrastructure (SQLite, ICMP)
    ^
    |
Agent Worker
```

## Reglas

- Domain no depende de infraestructura.
- Application declara puertos.
- Infrastructure implementa repositorios y probes.
- API compone dependencias.
- Desktop consume la API.
- Agent ejecuta monitoreo periódico.
