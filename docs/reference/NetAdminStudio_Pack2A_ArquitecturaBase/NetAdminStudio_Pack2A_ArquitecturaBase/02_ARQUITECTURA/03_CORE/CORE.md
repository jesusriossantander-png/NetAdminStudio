# Arquitectura del Core

## Objetivo

Contener el modelo de dominio sin dependencias externas.

## Entidades iniciales

- Asset
- AgentRegistration
- NetworkInterface
- Printer
- Share
- LocalUser
- LocalGroup
- Event
- Incident
- Policy
- HealthSnapshot

## Value Objects

- AssetId
- IpAddress
- MacAddress
- HostName
- HealthScore
- AgentVersion

## Interfaces

- IClock
- IIdGenerator
- IAssetRepository
- IEventRepository
- IAuditRepository

## Regla

Core no referencia WPF, EF Core, SQLite, PowerShell ni Windows APIs.
