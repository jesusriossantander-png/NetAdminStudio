# Casos de Uso

## UC-001 Preparar PC con impresora USB

Actor: Administrador  
Flujo:
1. Descubre PC.
2. Instala agente.
3. Verifica impresoras.
4. Cambia red a privada si hace falta.
5. Comparte impresora.
6. Genera ruta `\\PC\IMPRESORA`.
7. Registra evento.

## UC-002 Instalar impresora compartida en otras PCs

1. Selecciona varias PCs.
2. Elige impresora compartida.
3. Ejecuta acción masiva.
4. Cada agente instala la impresora.
5. Reporta éxito/error por PC.

## UC-003 Revisar carpetas compartidas inseguras

1. Ejecuta auditoría.
2. Detecta Everyone/Todos con Control Total.
3. Muestra riesgo.
4. Propone corrección.
5. Crea backup de ACL.
6. Aplica cambios con confirmación.

## UC-004 Alta de empleado

1. Ingresa nombre, apellido, sector.
2. Selecciona perfil.
3. Crea usuario local.
4. Agrega a grupos.
5. Asigna carpetas e impresoras.
6. Genera informe de alta.

## UC-005 Baja de empleado

1. Selecciona usuario.
2. Muestra accesos actuales.
3. Deshabilita usuario.
4. Quita accesos.
5. Genera informe.
