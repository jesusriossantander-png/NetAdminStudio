# Herramientas de diagnóstico

## Herramientas MVP

- Ping
- Traceroute
- DNS lookup
- Reverse DNS
- ARP lookup
- TCP connect sobre puertos autorizados
- Wake-on-LAN
- información de interfaz local
- prueba de gateway
- prueba de resolución
- prueba de conectividad externa

## Modelo

Cada diagnóstico es un `DiagnosticJob` con:

- tipo;
- objetivo;
- agente ejecutor;
- parámetros;
- estado;
- salida estructurada;
- salida textual limitada;
- timestamps;
- usuario solicitante;
- correlación;
- auditoría.

## Seguridad

- validar objetivos;
- bloquear rangos no autorizados;
- aplicar rate limiting;
- no permitir argumentos de shell libres;
- ejecutar mediante adaptadores tipados;
- truncar y sanitizar salidas;
- registrar quién inició la acción.
