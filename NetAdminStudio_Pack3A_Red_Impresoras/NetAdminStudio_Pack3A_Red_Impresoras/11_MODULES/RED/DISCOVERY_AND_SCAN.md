# Descubrimiento y escaneo

## Filosofía

El descubrimiento debe ser explícito, limitado, auditable y seguro.

## Modos

1. Passive: información existente del agente, ARP y cachés.
2. Safe ICMP: ping con concurrencia limitada.
3. Standard: ICMP + resolución DNS + puertos permitidos.
4. SNMP Assisted: consulta a equipos con perfil autorizado.
5. Import: CSV, JSON u otro origen administrado.

## Parámetros

- rango CIDR;
- exclusiones;
- timeout;
- concurrencia máxima;
- puertos permitidos;
- horario autorizado;
- agente ejecutor;
- perfil de credencial referenciado;
- política de reintentos.

## Límites recomendados MVP

- concurrencia predeterminada: 16;
- timeout ICMP: 1.5 s;
- máximo de 2 intentos;
- pausa configurable;
- no escanear internet;
- no usar UDP genérico;
- no realizar fuerza bruta;
- no almacenar comunidades SNMP en texto plano.

## Resultado

Cada ejecución produce:

- ScanJob;
- objetivos;
- observaciones;
- errores;
- nuevos candidatos;
- activos actualizados;
- duración;
- métricas de seguridad;
- auditoría.
