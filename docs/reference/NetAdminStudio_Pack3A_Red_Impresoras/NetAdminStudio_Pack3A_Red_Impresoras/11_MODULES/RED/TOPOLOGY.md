# Topología de red

## Objetivo

Representar relaciones conocidas o inferidas entre activos.

## NetworkLink

- Id
- SourceAssetId
- TargetAssetId
- SourceInterfaceId
- TargetInterfaceId
- LinkType
- SpeedMbps
- Duplex
- VlanId
- Status
- EvidenceType
- Confidence
- FirstSeenAt
- LastSeenAt
- IsManual

## Tipos de vínculo

- Ethernet
- Wireless
- PointToPoint
- Logical
- Uplink
- Trunk
- Access
- Virtual
- Unknown

## Evidencias

- LLDP/CDP;
- tabla MAC;
- ARP;
- gateway;
- agente;
- SNMP;
- asociación manual.

## Reglas visuales

- vínculo manual: línea sólida;
- vínculo inferido: línea segmentada;
- enlace degradado: advertencia;
- enlace caído: rojo semántico;
- confianza baja: opacidad reducida;
- no mostrar inferencias como hechos confirmados.
