# Seguridad del Escaneo

## Reglas

- Limitar concurrencia.
- Configurar timeout.
- No escanear fuera de la subred autorizada.
- No ejecutar probes agresivos.
- Permitir cancelación.
- Registrar inicio y fin.
- No intentar autenticación automática contra servicios.

## Configuración sugerida

- Concurrencia: 32.
- Timeout ping: 750 ms.
- Timeout TCP: 1000 ms.
- Máximo /24 en MVP.
