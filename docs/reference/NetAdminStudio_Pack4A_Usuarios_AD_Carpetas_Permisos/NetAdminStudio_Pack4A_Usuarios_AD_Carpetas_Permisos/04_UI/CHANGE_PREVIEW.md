# UI — Vista previa de cambios

Debe mostrar:

- estado actual;
- cambio solicitado;
- ACL resultante;
- usuarios afectados;
- carpetas afectadas;
- riesgos;
- respaldo;
- posibilidad de rollback;
- duración estimada no obligatoria;
- confirmación explícita.

No permitir ejecutar si:

- el host no está disponible;
- la ACL cambió desde la vista previa;
- falta respaldo;
- el usuario perdió permisos;
- el path no coincide;
- el agente no posee capacidad.
