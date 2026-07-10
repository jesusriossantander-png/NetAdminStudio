# Módulo Impresoras — Visión general

## Objetivo

Inventariar, observar y mantener impresoras conectadas por red, Windows, USB compartido, CUPS o Raspberry Pi.

## Entidades

- PrinterAsset
- PrinterEndpoint
- PrinterQueue
- PrinterObservation
- PrintJobObservation
- PrinterConsumable
- PrinterCounter
- PrinterMaintenanceRecord
- PrinterAlert

## Tipos de conexión

- NetworkDirect
- WindowsShared
- WindowsLocal
- UsbHost
- Cups
- RaspberryPiCups
- Ipp
- Lpd
- Unknown

## Estados

- Unknown
- Ready
- Printing
- Paused
- Offline
- Error
- PaperOut
- Jammed
- DoorOpen
- TonerLow
- Maintenance
- Retired
