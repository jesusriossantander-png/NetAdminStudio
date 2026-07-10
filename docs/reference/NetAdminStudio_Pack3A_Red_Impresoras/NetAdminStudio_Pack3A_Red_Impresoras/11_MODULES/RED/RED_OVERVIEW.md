# Módulo Red — Visión general

## Objetivo

Centralizar la observación, inventario, diagnóstico y trazabilidad de la red sin convertir NetAdminStudio en un controlador invasivo.

## Entidades principales

- NetworkAsset
- NetworkInterface
- NetworkPort
- NetworkObservation
- NetworkMetric
- NetworkLink
- NetworkScan
- NetworkAlert
- VlanObservation
- DhcpObservation
- DnsObservation
- SnmpProfileReference

## Casos de uso

- listar dispositivos;
- registrar dispositivo manual;
- descubrir dispositivos en un rango autorizado;
- consultar disponibilidad;
- ejecutar ping y traceroute;
- resolver DNS;
- consultar ARP;
- consultar puertos observados;
- visualizar topología;
- inspeccionar historial;
- reconocer alertas;
- asociar ubicación, sector y responsable;
- adjuntar documentación.

## Estados operativos

- Unknown
- Online
- Degraded
- Offline
- Maintenance
- Unmanaged
- Retired

El estado se calcula usando observaciones recientes, reglas de presencia y datos manuales. Nunca debe inferirse un estado crítico desde una sola medición aislada.
