# Cómo ejecutar NetAdmin Studio

NetAdmin Studio tiene dos ejecutables:

- **`NetAdminStudio.Api.exe`** — el motor/servidor (escaneo de red, base de datos). Corre por detrás.
- **`NetAdminStudio.Desktop.exe`** — la aplicación de escritorio que abrís y usás.

Ambos se comunican por `http://localhost:5188`.

## Opción rápida (desarrollo)

1. Abrir dos terminales en la carpeta del proyecto.
2. Terminal 1: `dotnet run --project src/NetAdminStudio.Api`
3. Terminal 2: `dotnet run --project src/NetAdminStudio.Desktop`
4. En la app, escribir el rango de red (por ejemplo `192.168.0.0/24`) y pulsar **Escanear red**.

## Opción .exe (para usar o distribuir)

1. Generar los ejecutables autónomos (incluyen el runtime de .NET; no hace falta instalar nada):

   ```powershell
   dotnet publish src/NetAdminStudio.Api -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/api
   dotnet publish src/NetAdminStudio.Desktop -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/desktop
   ```

2. Copiar las carpetas `publish/api` y `publish/desktop` al equipo destino (Windows 10/11 de 64 bits).
3. Ejecutar primero `publish/api/NetAdminStudio.Api.exe`, luego `publish/desktop/NetAdminStudio.Desktop.exe`.

## Uso del escaneo de red

- El rango se indica en formato CIDR (por ejemplo `192.168.0.0/24` para una red doméstica típica).
- Por seguridad, el rango máximo permitido es `/22` (1024 direcciones). Rangos más grandes se rechazan.
- El escaneo es **solo de observación**: descubre dispositivos (IP, nombre, MAC, fabricante, puertos, estado) pero nunca los modifica.
- Los dispositivos descubiertos se guardan en la base de datos local (`data/netadminstudio.db`, junto al `.exe` de la API).

## Notas

- El escaneo de red y la lectura de la tabla ARP funcionan mejor si la app se ejecuta en la misma red que se quiere inspeccionar.
- El puerto por defecto es `5188`. Se puede cambiar con la variable de entorno `ASPNETCORE_URLS` (por ejemplo `http://localhost:6000`); en ese caso hay que ajustar también la URL en la app de escritorio.
