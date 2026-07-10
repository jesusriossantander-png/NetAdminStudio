# Checklist de aceptación — Pack 3A

## Arquitectura

- [ ] El dominio no depende de WPF, SQLite, SNMP, WMI o CUPS.
- [ ] Adaptadores externos implementan interfaces de aplicación.
- [ ] Operaciones largas usan jobs.
- [ ] Errores usan el modelo común.
- [ ] Eventos usan envelope versionado.

## Red

- [ ] Inventario manual y descubierto.
- [ ] Escaneo limitado y auditable.
- [ ] Presencia con tolerancia a fallos.
- [ ] Topología distingue evidencia y confianza.
- [ ] Herramientas sin argumentos de shell.
- [ ] Métricas y alertas historizadas.

## Impresoras

- [ ] Modela red, Windows, USB y CUPS.
- [ ] Colas y trabajos protegidos por permisos.
- [ ] Consumibles distinguen cero de desconocido.
- [ ] Contadores conservan origen y fecha.
- [ ] Mantenimiento es auditable.

## UX

- [ ] Loading, empty, error y stale states.
- [ ] Acciones sensibles con confirmación.
- [ ] Última actualización visible.
- [ ] Navegación por teclado.
- [ ] Colores no son el único indicador.

## Seguridad

- [ ] Secretos fuera de SQLite plano.
- [ ] Rangos permitidos.
- [ ] Rate limiting.
- [ ] RBAC.
- [ ] Auditoría completa.
