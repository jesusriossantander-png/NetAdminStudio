# Motor de Descubrimiento

## Objetivo

Detectar activos presentes en la LAN.

## Fuentes

- Interfaces locales.
- ARP.
- Ping.
- DNS reverso.
- NetBIOS.
- Puertos TCP.
- SMB.
- Respuesta específica del Agent.

## Fases

1. Detectar subred.
2. Generar rango.
3. Ejecutar probes concurrentes limitados.
4. Resolver identidad.
5. Clasificar tipo.
6. Guardar AssetCandidate.
7. Correlacionar con assets existentes.

## Regla

Descubrir no significa administrar.
