# Diseño — NetAdmin Studio: Escaneo de Red Real (v1)

**Fecha:** 2026-07-10
**Autor:** Jesús (con Claude)
**Estado:** Aprobado el diseño; pendiente plan de implementación

---

## 1. Objetivo

Convertir la primera rama de la visión de NetAdmin Studio —
**"Monitorea toda la red" + "Detecta equipos"**— en funcionalidad real,
reemplazando los datos demo sembrados por infraestructura descubierta de verdad.

Al terminar esta versión, el usuario podrá:

1. Abrir la app de escritorio.
2. Indicar un rango de red (ej. `192.168.0.0/24`) y confirmar el escaneo.
3. Ver en vivo los dispositivos reales descubiertos: IP, hostname, MAC,
   fabricante, puertos abiertos, tipo estimado, estado y latencia.
4. Que esos dispositivos alimenten el dashboard y las alertas existentes.

## 2. Contexto y punto de partida

El repositorio tiene 8 "packs" de documentación y un esqueleto de código real
en `Pack5A_Implementacion_Real`. El esqueleto está **bien construido**:
Clean Architecture en .NET 8 (Domain → Application → Infrastructure →
Api/Agent/Desktop), SQLite con WAL, Minimal API + Swagger, WPF MVVM
(CommunityToolkit), y un Agent (Windows Service) que hoy dispara el monitoreo
cada 30 s vía la API.

**Limitación actual:** no existe descubrimiento. `MonitoringService` solo hace
ping a activos que ya existen en la base, y esos activos provienen únicamente
del `DemoDataSeeder`. No hay forma de encontrar dispositivos nuevos.

## 3. Decisiones tomadas

| # | Decisión | Elección |
|---|----------|----------|
| D1 | Punto de partida | Consolidar Pack 5A como base (no empezar de cero) |
| D2 | Dónde corre el escaneo | **Opción A**: en Infrastructure, disparado por la API. El Agent u otros clientes lo invocan vía endpoint. Reutiliza el patrón existente. |
| D3 | Ubicación de la documentación | Mover los 8 packs a `docs/` (se conservan como referencia) |
| D4 | Alcance del escaneo v1 | Descubrimiento + identificación (MAC/fabricante) + puertos/servicios |

## 4. Consolidación del repositorio

Estructura destino (solución única en la raíz):

```
f:\NetAdminStudio\
├─ NetAdminStudio.sln
├─ src\
│  ├─ NetAdminStudio.Domain\
│  ├─ NetAdminStudio.Application\
│  ├─ NetAdminStudio.Infrastructure\
│  ├─ NetAdminStudio.Api\
│  ├─ NetAdminStudio.Agent\
│  └─ NetAdminStudio.Desktop\
├─ tests\
│  └─ NetAdminStudio.Tests\
└─ docs\
   ├─ DocumentationPack_v1\
   ├─ Pack1_Foundation\
   ├─ ... (los demás packs de documentación)
```

- Se mueve el contenido de `Pack5A_Implementacion_Real/.../` (código) a la raíz.
- Se mueven los packs de documentación a `docs/`.
- No se pierde nada; solo se reorganiza. Los `.csproj` y el `.sln` conservan
  sus rutas relativas `src\...` y `tests\...`, por lo que la solución sigue
  compilando sin cambios de referencias.

## 5. Motor de descubrimiento

### 5.1 Componente principal

Nuevo servicio `NetworkDiscoveryEngine` en
`Infrastructure/Networking/`, que ejecuta un escaneo sobre un rango CIDR en
cuatro fases:

| Fase | Nombre | Qué hace | Mecanismo |
|------|--------|----------|-----------|
| 1 | Barrido (sweep) | Detecta qué IPs responden | Ping ICMP en paralelo, con límite de concurrencia |
| 2 | Identidad | Resuelve hostname, MAC y fabricante | DNS inverso + tabla ARP local + base OUI local |
| 3 | Puertos | Detecta servicios abiertos | Conexión TCP con timeout corto a una lista fija de puertos |
| 4 | Clasificación | Estima el tipo de dispositivo | Reglas combinando puertos abiertos + fabricante (OUI) |

### 5.2 Abstracciones (puertos nuevos en Application)

Para poder testear sin tocar la red real, la lógica de red se expone tras
interfaces en `Application/Abstractions`:

```csharp
public interface INetworkScanner        // barrido + puertos
{
    Task<HostProbe> ProbeHostAsync(IPAddress ip, CancellationToken ct);
    Task<IReadOnlyList<int>> ScanPortsAsync(IPAddress ip, IReadOnlyList<int> ports, CancellationToken ct);
}

public interface IArpTable               // MAC por IP
{
    Task<string?> GetMacAsync(IPAddress ip, CancellationToken ct);
}

public interface IVendorLookup           // fabricante por MAC (OUI)
{
    string? ResolveVendor(string macAddress);
}
```

- `NetworkDiscoveryEngine` (Application) orquesta las fases usando estas
  interfaces y `INetworkProbe` (ya existente).
- Las implementaciones concretas (que sí tocan la red / el SO) viven en
  Infrastructure.
- La **clasificación** y el **cálculo de IPs de un CIDR** son lógica pura,
  testeable sin red.

### 5.3 Puertos escaneados (lista fija y conservadora)

`22 (SSH), 23 (Telnet), 53 (DNS), 80 (HTTP), 443 (HTTPS), 445 (SMB),
515 (LPD), 631 (IPP), 9100 (RAW/JetDirect), 161 (SNMP), 3389 (RDP), 8080`.

Reglas de clasificación (ejemplos):
- `9100` o `515` o `631` abiertos → **Printer**.
- `161` (SNMP) + fabricante de red conocido → **Switch/Router/AccessPoint**.
- `445` + `3389` → **Workstation/Server** Windows.
- Sin puertos y sin más señales → **Unknown**.

## 6. Cambios de datos

Ampliar la tabla `assets` (solo columnas nuevas, no destructivo):

| Columna | Tipo | Uso |
|---------|------|-----|
| `hostname` | TEXT | Nombre resuelto por DNS inverso |
| `open_ports` | TEXT (JSON) | Lista de puertos abiertos, ej. `[80,443,9100]` |
| `first_seen_at` | TEXT | Primera vez que se descubrió |
| `origin` | TEXT | `discovery` o `demo` |

El `NetworkAsset` del dominio gana estas propiedades y un método
`RecordDiscovery(...)` que fija hostname, MAC, vendor, puertos, tipo y origen.

El `DemoDataSeeder` se conserva pero solo siembra si la base está vacía
(comportamiento actual), marcando su origen como `demo`. En cuanto haya un
escaneo real, los datos reales conviven o reemplazan a los demo por IP.

## 7. API y UI

### 7.1 Endpoints nuevos (Api)

- `POST /api/v1/network/scan` — cuerpo: `{ "cidr": "192.168.0.0/24", "options": {...} }`.
  Lanza un escaneo (asíncrono) y devuelve un `scanId`.
- `GET  /api/v1/network/scan/{scanId}` — estado y progreso del escaneo
  (total, completadas, encontradas, terminado sí/no).
- Los dispositivos descubiertos se persisten vía `IAssetRepository` y quedan
  visibles en el ya existente `GET /api/v1/assets`.

### 7.2 Escritorio (Desktop, WPF MVVM)

Nueva vista **"Red"** con:
- Campo de rango (CIDR) con valor por defecto detectado de la subred local.
- Botón **Escanear** (pide confirmación antes de empezar).
- Barra de progreso (X de N direcciones).
- Grilla en vivo: IP · Hostname · MAC · Fabricante · Puertos · Tipo · Estado · Latencia.

Sigue el patrón `MainViewModel` + `NetAdminApiClient` ya existente
(polling del estado del escaneo mientras corre).

## 8. Seguridad y límites (reglas globales del proyecto)

- **Confirmar antes de escanear**: el rango se muestra y se confirma
  explícitamente (regla "confirmar cambios críticos").
- **Solo observación**: el escaneo nunca modifica dispositivos
  (ADR-0018 "observar antes de administrar").
- **Límites duros**: máximo de IPs por escaneo (ej. /24 = 254; rechazar rangos
  mayores a un umbral configurable), concurrencia acotada (ej. 32 pings /
  16 conexiones TCP simultáneas), timeouts cortos (ping 1–2 s, TCP 500 ms).
- **Lista de puertos fija** y conservadora (sin escaneo agresivo de rangos).
- **Auditoría**: cada escaneo se registra (quién, cuándo, qué rango, resultado).
- **Sin dependencia de Internet**: la base OUI de fabricantes se embebe/almacena
  localmente (regla "no depender de Internet para funciones críticas").

## 9. Pruebas

Tests unitarios (sin tocar la red real, usando las interfaces del §5.2):

- **CIDR → lista de IPs**: `192.168.0.0/30` produce las hosts correctas;
  validación de rangos inválidos y demasiado grandes.
- **Clasificación**: combinaciones de puertos + fabricante producen el
  `AssetType` esperado.
- **Parseo de ARP**: una salida de tabla ARP de ejemplo produce el mapa
  IP→MAC correcto.
- **OUI lookup**: una MAC conocida resuelve el fabricante esperado.
- **DiscoveryEngine** con scanner/arp/vendor simulados: un rango pequeño
  produce los `NetworkAsset` esperados y se persisten vía repositorio en memoria.

## 10. Fuera de alcance (v1)

- SNMP / WMI reales (Fase 2).
- Topología visual de red.
- Escaneo de máquinas remotas vía Agent (la lógica queda lista para ello, pero
  v1 escanea desde la máquina donde corre la API).
- Impresoras, usuarios/permisos, reportes, IA (fases posteriores).

## 11. Riesgos

- **Permisos ICMP/ARP en Windows**: el ping administrado funciona sin admin; la
  lectura de la tabla ARP puede requerir invocar `arp -a` o P/Invoke. Se abstrae
  tras `IArpTable` para aislar el riesgo.
- **Falsos negativos**: dispositivos que no responden a ping pero sí tienen
  puertos abiertos. Mitigación: la fase de puertos puede correr sobre el rango
  aunque el ping falle (configurable), a costa de más tiempo.
- **Target net8.0 con SDK 9/10 instalado**: compila, pero se verifica en el
  primer build del plan de implementación.
