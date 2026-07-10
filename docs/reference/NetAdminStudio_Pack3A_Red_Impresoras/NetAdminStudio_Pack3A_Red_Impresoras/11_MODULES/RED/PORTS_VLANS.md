# Puertos, interfaces y VLAN

## NetworkPort

- AssetId
- PortIndex
- Name
- Description
- MacAddress
- AdminStatus
- OperationalStatus
- SpeedMbps
- Duplex
- PoeState
- PoePowerWatts
- AccessVlanId
- NativeVlanId
- IsTrunk
- ErrorCount
- DiscardCount
- LastChangeAt
- Source
- ObservedAt

## VLAN observada

- VlanId
- Name
- Description
- Scope
- SourceAssetId
- TaggedPorts
- UntaggedPorts
- FirstSeenAt
- LastSeenAt

## Alcance MVP

El sistema visualiza e historiza. No modifica configuración del switch.

## Detección de anomalías

- puerto operativo abajo con estado administrativo arriba;
- errores crecientes;
- velocidad negociada inferior a la esperada;
- consumo PoE cercano al límite;
- puerto flapping;
- posible loop basado en eventos disponibles;
- VLAN desconocida o inconsistente.
