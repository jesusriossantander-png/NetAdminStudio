# Modelo de impresora

## PrinterAsset

- Id
- AssetId
- DisplayName
- Vendor
- Model
- SerialNumber
- FirmwareVersion
- ColorCapability
- DuplexCapability
- MaxPaperSize
- ConnectionType
- HostAssetId
- PrimaryEndpointId
- DriverName
- LocationId
- SectorId
- ResponsiblePersonId
- OperationalState
- Notes
- FirstSeenAt
- LastSeenAt

## PrinterEndpoint

- Protocol
- Address
- Port
- ShareName
- QueueName
- DeviceUri
- IsPrimary
- IsSecure
- Source
- LastVerifiedAt

## Identidad

Se prioriza:

1. serial reportado;
2. UUID de dispositivo;
3. MAC;
4. host + cola;
5. IP + modelo;
6. coincidencia manual verificada.
