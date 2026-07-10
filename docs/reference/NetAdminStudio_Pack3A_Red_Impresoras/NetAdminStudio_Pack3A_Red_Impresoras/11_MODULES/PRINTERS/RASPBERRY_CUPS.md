# Impresoras USB mediante Raspberry Pi y CUPS

## Escenario

Una Raspberry Pi actúa como servidor de impresión para una o varias impresoras USB sin interfaz de red.

## Componentes

- Raspberry Pi OS;
- CUPS;
- USB;
- IPP;
- mDNS opcional;
- agente NetAdminStudio opcional;
- monitoreo del host;
- respaldo de configuración.

## Datos observables

- host;
- versión de CUPS;
- colas;
- URI;
- driver;
- disponibilidad;
- trabajos pendientes;
- errores;
- impresoras USB;
- uso de disco y temperatura del host.

## Reglas

- NetAdminStudio no guarda contraseña SSH en texto plano;
- el descubrimiento no habilita CUPS;
- cambios requieren acción explícita;
- documentar IP fija o reserva DHCP;
- limitar CUPS a redes autorizadas;
- preferir IPP;
- respaldar `/etc/cups` antes de cambios.
