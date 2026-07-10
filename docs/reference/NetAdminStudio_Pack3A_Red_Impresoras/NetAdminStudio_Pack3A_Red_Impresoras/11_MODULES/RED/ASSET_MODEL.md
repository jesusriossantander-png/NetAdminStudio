# Modelo de activos de red

## NetworkAsset

Campos mínimos:

- Id
- AssetCode
- DisplayName
- AssetType
- Vendor
- Model
- SerialNumber
- FirmwareVersion
- PrimaryIpAddress
- PrimaryMacAddress
- Hostname
- LocationId
- SectorId
- ResponsiblePersonId
- ManagementMode
- OperationalState
- Criticality
- Notes
- Source
- Confidence
- FirstSeenAt
- LastSeenAt
- CreatedAt
- UpdatedAt
- RetiredAt

## Tipos iniciales

- Router
- Switch
- AccessPoint
- Firewall
- Server
- Workstation
- Laptop
- RaspberryPi
- Plc
- Camera
- DvrNvr
- Printer
- Ups
- IoT
- Unknown

## Reglas

- IP y MAC no son identificadores permanentes.
- Un activo puede tener múltiples interfaces.
- Un activo descubierto no se fusiona automáticamente con uno manual si la coincidencia no es fuerte.
- Las fusiones deben quedar auditadas.
- `Source` identifica Manual, Agent, SNMP, ARP, WMI, IPP, CUPS o Import.
- `Confidence` admite Low, Medium, High y Verified.
