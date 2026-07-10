# Cálculo de acceso efectivo

## Objetivo

Responder: “¿Qué puede hacer realmente este usuario o grupo sobre esta carpeta?”

## Entradas

- usuario o grupo;
- SID;
- membresías directas;
- membresías anidadas;
- ACL NTFS;
- ACL SMB;
- propietario;
- ruta;
- herencia;
- contexto del host.

## Salida

- nivel efectivo;
- permisos incluidos;
- permisos denegados;
- reglas determinantes;
- grupos que aportan acceso;
- origen NTFS/SMB;
- nivel de confianza;
- advertencias.

## Limitaciones

El cálculo offline puede diferir del token real de Windows. Debe informarse como:

- Verified: validado por el host.
- Calculated: calculado con datos completos.
- Estimated: datos parciales.
- Unknown: no calculable.
