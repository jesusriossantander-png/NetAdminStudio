# Escaneo de Red Real (v1) — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Añadir descubrimiento de red real (barrido + MAC/fabricante + puertos + clasificación) a NetAdmin Studio, reemplazando los datos demo por dispositivos reales visibles en la app de escritorio.

**Architecture:** Clean Architecture .NET 8 ya existente. La lógica de orquestación del escaneo vive en `Application` detrás de interfaces (`INetworkScanner`, `IArpTable`, `IVendorLookup`); las implementaciones que tocan la red/SO viven en `Infrastructure`. La API dispara escaneos asíncronos rastreados por un `ScanJobManager` en memoria; la app WPF muestra el progreso por polling. La lógica pura (CIDR, clasificación, parseo ARP) se testea sin red.

**Tech Stack:** .NET 8, C#, xUnit, Microsoft.Data.Sqlite, ASP.NET Minimal API, WPF + CommunityToolkit.Mvvm.

## Global Constraints

- Target frameworks: `net8.0` (librerías/API/Agent), `net8.0-windows` (Desktop). Compilan con SDK 10 instalado vía `global.json` con `rollForward: latestMajor`.
- Todo escaneo es **solo observación**: nunca modifica dispositivos (ADR-0018).
- **No depender de Internet**: la base OUI de fabricantes se almacena localmente.
- Límites duros de escaneo: rechazar prefijos CIDR más grandes que `/22` (máx. 1024 hosts); concurrencia de ping ≤ 32; concurrencia TCP ≤ 16; timeout ping ≤ 2 s; timeout TCP ≤ 500 ms.
- Lista de puertos fija: `22, 23, 53, 80, 161, 443, 445, 515, 631, 3389, 8080, 9100`.
- Nada de secretos ni credenciales en código.
- Los tests corren con `dotnet test`. Cada tarea termina con commit.

---

## Estructura de archivos

**Consolidación (Task 0):** el código pasa de `NetAdminStudio_Pack5A_Implementacion_Real/NetAdminStudio_Pack5A_Implementacion_Real/{src,tests,*.sln,global.json,Directory.Build.props}` a la raíz `f:\NetAdminStudio\{src,tests,...}`. La documentación (los 8 packs) pasa a `docs/reference/`.

**Archivos nuevos:**
- `src/NetAdminStudio.Application/Networking/Cidr.cs` — cálculo de hosts de un CIDR (puro).
- `src/NetAdminStudio.Application/Networking/AssetClassifier.cs` — puertos+fabricante → AssetType (puro).
- `src/NetAdminStudio.Application/Abstractions/NetworkPorts.cs` — `INetworkScanner`, `IArpTable`, `IVendorLookup` + DTOs.
- `src/NetAdminStudio.Application/Networking/NetworkDiscoveryEngine.cs` — orquesta las 4 fases.
- `src/NetAdminStudio.Application/Networking/ScanJobManager.cs` — estado/progreso de escaneos en memoria.
- `src/NetAdminStudio.Infrastructure/Networking/TcpPortScanner.cs` — `INetworkScanner`.
- `src/NetAdminStudio.Infrastructure/Networking/ArpTable.cs` — `IArpTable` (parsea `arp -a`).
- `src/NetAdminStudio.Infrastructure/Networking/OuiVendorLookup.cs` — `IVendorLookup` (datos OUI locales).
- `src/NetAdminStudio.Infrastructure/Networking/OuiData.cs` — diccionario OUI embebido.
- `src/NetAdminStudio.Desktop/Views/NetworkView.xaml(.cs)` — vista "Red".
- Tests: `tests/NetAdminStudio.Tests/{CidrTests,AssetClassifierTests,ArpTableParserTests,OuiVendorLookupTests,NetworkDiscoveryEngineTests}.cs`.

**Archivos modificados:**
- `global.json` — rollForward.
- `src/NetAdminStudio.Domain/Assets/NetworkAsset.cs` — campos de descubrimiento.
- `src/NetAdminStudio.Infrastructure/Persistence/Database.cs` — columnas nuevas.
- `src/NetAdminStudio.Infrastructure/Persistence/Repositories.cs` — leer/escribir campos nuevos.
- `src/NetAdminStudio.Application/DependencyInjection.cs` y `src/NetAdminStudio.Infrastructure/DependencyInjection.cs` — registrar servicios nuevos.
- `src/NetAdminStudio.Api/Program.cs` — endpoints de escaneo.
- `src/NetAdminStudio.Desktop/Services/NetAdminApiClient.cs` — métodos de escaneo.
- `src/NetAdminStudio.Desktop/ViewModels/MainViewModel.cs` y `MainWindow.xaml` — integrar vista Red.

---

### Task 0: Consolidar el repositorio y arreglar el build

**Files:**
- Move: `NetAdminStudio_Pack5A_Implementacion_Real/NetAdminStudio_Pack5A_Implementacion_Real/*` → raíz.
- Move: `NetAdminStudio_DocumentationPack_v1`, `NetAdminStudio_Pack1_Foundation` … `NetAdminStudio_Pack4B_*` → `docs/reference/`.
- Modify: `global.json`

- [ ] **Step 1: Mover el código a la raíz**

```bash
cd f:/NetAdminStudio
INNER="NetAdminStudio_Pack5A_Implementacion_Real/NetAdminStudio_Pack5A_Implementacion_Real"
git mv "$INNER/NetAdminStudio.sln" .
git mv "$INNER/global.json" .
git mv "$INNER/Directory.Build.props" . 2>/dev/null || true
git mv "$INNER/.gitignore" .gitignore 2>/dev/null || true
git mv "$INNER/src" src
git mv "$INNER/tests" tests
# archivos sueltos restantes (README, etc.) del pack de código
git mv "$INNER"/*.md docs/reference/ 2>/dev/null || true
```

- [ ] **Step 2: Mover la documentación a docs/reference/**

```bash
cd f:/NetAdminStudio
mkdir -p docs/reference
for d in NetAdminStudio_DocumentationPack_v1 NetAdminStudio_Pack1_Foundation \
         NetAdminStudio_Pack2A_ArquitecturaBase NetAdminStudio_Pack2B_Agent_Comunicacion \
         NetAdminStudio_Pack3A_Red_Impresoras NetAdminStudio_Pack3B_Monitoreo \
         NetAdminStudio_Pack4A_Usuarios_AD_Carpetas_Permisos \
         NetAdminStudio_Pack4B_Auditoria_Reportes_Cumplimiento; do
  git mv "$d" "docs/reference/$d"
done
# borrar la carpeta de código ya vaciada
rmdir "NetAdminStudio_Pack5A_Implementacion_Real/NetAdminStudio_Pack5A_Implementacion_Real" 2>/dev/null || true
rmdir "NetAdminStudio_Pack5A_Implementacion_Real" 2>/dev/null || true
```

- [ ] **Step 3: Arreglar global.json**

Reemplazar el contenido de `global.json` por:

```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestMajor"
  }
}
```

- [ ] **Step 4: Verificar que compila y los tests existentes pasan**

Run: `cd f:/NetAdminStudio && dotnet build NetAdminStudio.sln --nologo`
Expected: `Build succeeded`. 0 Error(s).

Run: `dotnet test --nologo`
Expected: los 2 tests existentes (`NetworkAssetTests`) pasan.

> Nota: `NetAdminStudio.Desktop` (`net8.0-windows`, WPF) solo compila en Windows; en este entorno ya estamos en Windows, así que compila.

- [ ] **Step 5: Commit**

```bash
cd f:/NetAdminStudio
git add -A
git commit -m "Consolida el código en la raíz y mueve documentación a docs/reference"
```

---

### Task 1: Cálculo de hosts de un CIDR (lógica pura)

**Files:**
- Create: `src/NetAdminStudio.Application/Networking/Cidr.cs`
- Test: `tests/NetAdminStudio.Tests/CidrTests.cs`

**Interfaces:**
- Produces: `static class Cidr` con `static IReadOnlyList<IPAddress> Hosts(string cidr)` y `static int MaxPrefixHosts` const. Lanza `ArgumentException` si el CIDR es inválido o el prefijo es menor a `/22` (más de 1024 hosts).

- [ ] **Step 1: Escribir el test que falla**

```csharp
using System.Net;
using NetAdminStudio.Application.Networking;

namespace NetAdminStudio.Tests;

public sealed class CidrTests
{
    [Fact]
    public void Hosts_Slash30_ReturnsTwoUsableHosts()
    {
        var hosts = Cidr.Hosts("192.168.0.0/30");
        Assert.Equal(2, hosts.Count);
        Assert.Equal(IPAddress.Parse("192.168.0.1"), hosts[0]);
        Assert.Equal(IPAddress.Parse("192.168.0.2"), hosts[1]);
    }

    [Fact]
    public void Hosts_Slash24_Returns254Hosts()
    {
        var hosts = Cidr.Hosts("10.0.0.0/24");
        Assert.Equal(254, hosts.Count);
        Assert.DoesNotContain(IPAddress.Parse("10.0.0.0"), hosts);   // red
        Assert.DoesNotContain(IPAddress.Parse("10.0.0.255"), hosts); // broadcast
    }

    [Theory]
    [InlineData("no-es-cidr")]
    [InlineData("192.168.0.0/21")]   // demasiado grande
    [InlineData("999.0.0.0/24")]
    public void Hosts_InvalidOrTooLarge_Throws(string cidr) =>
        Assert.Throws<ArgumentException>(() => Cidr.Hosts(cidr));
}
```

- [ ] **Step 2: Ejecutar el test y verificar que falla**

Run: `dotnet test --filter FullyQualifiedName~CidrTests --nologo`
Expected: FAIL (no compila: `Cidr` no existe).

- [ ] **Step 3: Implementación mínima**

```csharp
using System.Net;

namespace NetAdminStudio.Application.Networking;

public static class Cidr
{
    public const int MinPrefix = 22; // /22 = 1024 direcciones (límite duro)

    public static IReadOnlyList<IPAddress> Hosts(string cidr)
    {
        if (string.IsNullOrWhiteSpace(cidr))
            throw new ArgumentException("CIDR vacío.", nameof(cidr));

        var parts = cidr.Split('/');
        if (parts.Length != 2
            || !IPAddress.TryParse(parts[0], out var baseIp)
            || baseIp.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork
            || !int.TryParse(parts[1], out var prefix)
            || prefix is < 0 or > 32)
            throw new ArgumentException($"CIDR inválido: {cidr}", nameof(cidr));

        if (prefix < MinPrefix)
            throw new ArgumentException(
                $"Rango demasiado grande (prefijo /{prefix}); mínimo permitido /{MinPrefix}.",
                nameof(cidr));

        var baseBytes = baseIp.GetAddressBytes();
        var baseValue = (uint)((baseBytes[0] << 24) | (baseBytes[1] << 16)
                             | (baseBytes[2] << 8) | baseBytes[3]);
        var mask = prefix == 0 ? 0u : uint.MaxValue << (32 - prefix);
        var network = baseValue & mask;
        var broadcast = network | ~mask;

        var hosts = new List<IPAddress>();
        // /31 y /32 no tienen hosts "usables" en este modelo; devolvemos vacío.
        for (var value = network + 1; value < broadcast; value++)
        {
            var bytes = new[]
            {
                (byte)(value >> 24), (byte)(value >> 16),
                (byte)(value >> 8), (byte)value
            };
            hosts.Add(new IPAddress(bytes));
        }
        return hosts;
    }
}
```

- [ ] **Step 4: Ejecutar y verificar que pasa**

Run: `dotnet test --filter FullyQualifiedName~CidrTests --nologo`
Expected: PASS (3 casos).

- [ ] **Step 5: Commit**

```bash
git add src/NetAdminStudio.Application/Networking/Cidr.cs tests/NetAdminStudio.Tests/CidrTests.cs
git commit -m "Agrega cálculo de hosts de un CIDR con límite de tamaño"
```

---

### Task 2: Extender el dominio NetworkAsset con datos de descubrimiento

**Files:**
- Modify: `src/NetAdminStudio.Domain/Assets/NetworkAsset.cs`
- Test: `tests/NetAdminStudio.Tests/NetworkAssetTests.cs` (añadir test)

**Interfaces:**
- Produces: en `NetworkAsset`, propiedades nuevas `string? Hostname`, `IReadOnlyList<int> OpenPorts`, `DateTimeOffset? FirstSeenAt`, `string Origin` (default `"demo"`); y método `void RecordDiscovery(string? hostname, string? mac, string? vendor, IReadOnlyList<int> openPorts, AssetType type, double? latencyMs, DateTimeOffset observedAt)` que además marca `Online`, fija `Origin="discovery"` y setea `FirstSeenAt` si estaba nulo.

- [ ] **Step 1: Escribir el test que falla** (añadir a `NetworkAssetTests.cs`)

```csharp
[Fact]
public void RecordDiscovery_SetsDiscoveryFieldsAndOnline()
{
    var asset = new NetworkAsset { Name = "PC-01", Type = AssetType.Unknown };
    var now = DateTimeOffset.UtcNow;

    asset.RecordDiscovery("PC-01.local", "AA-BB-CC-11-22-33", "Dell",
        new[] { 445, 3389 }, AssetType.Workstation, 3.0, now);

    Assert.Equal(OperationalState.Online, asset.State);
    Assert.Equal("PC-01.local", asset.Hostname);
    Assert.Equal("Dell", asset.Vendor);
    Assert.Equal(AssetType.Workstation, asset.Type);
    Assert.Equal(new[] { 445, 3389 }, asset.OpenPorts);
    Assert.Equal("discovery", asset.Origin);
    Assert.Equal(now, asset.FirstSeenAt);
}
```

- [ ] **Step 2: Ejecutar y verificar que falla**

Run: `dotnet test --filter FullyQualifiedName~RecordDiscovery_SetsDiscoveryFields --nologo`
Expected: FAIL (no compila).

- [ ] **Step 3: Implementación** (añadir a `NetworkAsset`, después de `LatencyMs`)

```csharp
    public string? Hostname { get; set; }
    public IReadOnlyList<int> OpenPorts { get; set; } = Array.Empty<int>();
    public DateTimeOffset? FirstSeenAt { get; private set; }
    public string Origin { get; set; } = "demo";

    public void RecordDiscovery(
        string? hostname, string? mac, string? vendor,
        IReadOnlyList<int> openPorts, AssetType type,
        double? latencyMs, DateTimeOffset observedAt)
    {
        Hostname = hostname;
        if (!string.IsNullOrWhiteSpace(mac)) MacAddress = mac;
        if (!string.IsNullOrWhiteSpace(vendor)) Vendor = vendor;
        OpenPorts = openPorts;
        if (type != AssetType.Unknown) Type = type;
        Origin = "discovery";
        FirstSeenAt ??= observedAt;
        RecordPresence(true, latencyMs, observedAt);
    }
```

- [ ] **Step 4: Ejecutar y verificar que pasa**

Run: `dotnet test --filter FullyQualifiedName~NetworkAssetTests --nologo`
Expected: PASS (3 tests).

- [ ] **Step 5: Commit**

```bash
git add src/NetAdminStudio.Domain/Assets/NetworkAsset.cs tests/NetAdminStudio.Tests/NetworkAssetTests.cs
git commit -m "Agrega campos y método de descubrimiento a NetworkAsset"
```

---

### Task 3: Persistir los campos de descubrimiento

**Files:**
- Modify: `src/NetAdminStudio.Infrastructure/Persistence/Database.cs`
- Modify: `src/NetAdminStudio.Infrastructure/Persistence/Repositories.cs`

**Interfaces:**
- Consumes: `NetworkAsset.Hostname/OpenPorts/FirstSeenAt/Origin` (Task 2).
- Produces: `AssetRepository` lee/escribe las columnas nuevas; `OpenPorts` se serializa como CSV (ej. `"445,3389"`).

- [ ] **Step 1: Añadir columnas al esquema** (en `Database.Initialize`, dentro del `CREATE TABLE assets`, añadir columnas antes del cierre `);`)

```sql
 hostname TEXT,
 open_ports TEXT,
 first_seen_at TEXT,
 origin TEXT NOT NULL DEFAULT 'demo'
```

Y justo después del bloque de `CREATE TABLE ... automation_rules(...);` añadir migración idempotente para bases existentes:

```sql
-- Migración: columnas de descubrimiento (ignorar error si ya existen)
```

En C#, tras `command.ExecuteNonQuery();`, añadir un helper que intente los `ALTER TABLE` y trague el error si la columna ya existe:

```csharp
foreach (var col in new[]
{
    "ALTER TABLE assets ADD COLUMN hostname TEXT",
    "ALTER TABLE assets ADD COLUMN open_ports TEXT",
    "ALTER TABLE assets ADD COLUMN first_seen_at TEXT",
    "ALTER TABLE assets ADD COLUMN origin TEXT NOT NULL DEFAULT 'demo'"
})
{
    try
    {
        using var alter = db.CreateCommand();
        alter.CommandText = col;
        alter.ExecuteNonQuery();
    }
    catch (SqliteException) { /* columna ya existe */ }
}
```

- [ ] **Step 2: Escribir/leer en `AssetRepository`**

En `GetAllAsync`, dentro del `new NetworkAsset { ... }`, añadir tras `Location`:

```csharp
                Hostname = reader.NullableString("hostname"),
                Origin = reader.NullableString("origin") ?? "demo",
                OpenPorts = ParsePorts(reader.NullableString("open_ports"))
```

Y un helper privado en la clase:

```csharp
    private static IReadOnlyList<int> ParsePorts(string? csv) =>
        string.IsNullOrWhiteSpace(csv)
            ? Array.Empty<int>()
            : csv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                 .Select(int.Parse).ToArray();
```

En `UpsertAsync`: añadir columnas al INSERT/UPDATE (`hostname,open_ports,first_seen_at,origin` en la lista, `$hostname,$ports,$firstSeen,$origin` en VALUES, y las tres líneas `xxx=excluded.xxx` en el ON CONFLICT — **excepto** `first_seen_at`, que NO se pisa en update). Parámetros:

```csharp
        cmd.Parameters.AddWithValue("$hostname", (object?)asset.Hostname ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$ports",
            asset.OpenPorts.Count == 0 ? (object)DBNull.Value : string.Join(",", asset.OpenPorts));
        cmd.Parameters.AddWithValue("$firstSeen",
            asset.FirstSeenAt?.ToString("O") ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$origin", asset.Origin);
```

- [ ] **Step 3: Verificar compilación y tests**

Run: `dotnet build --nologo && dotnet test --nologo`
Expected: Build succeeded; todos los tests pasan.

- [ ] **Step 4: Commit**

```bash
git add src/NetAdminStudio.Infrastructure/Persistence/
git commit -m "Persiste campos de descubrimiento en assets (con migración)"
```

---

### Task 4: Abstracciones de red (interfaces + DTOs)

**Files:**
- Create: `src/NetAdminStudio.Application/Abstractions/NetworkPorts.cs`

**Interfaces:**
- Produces:
  - `record HostProbe(bool Reachable, string? Hostname, double? LatencyMs)`
  - `interface INetworkScanner { Task<HostProbe> ProbeHostAsync(IPAddress ip, CancellationToken ct); Task<IReadOnlyList<int>> ScanPortsAsync(IPAddress ip, IReadOnlyList<int> ports, CancellationToken ct); }`
  - `interface IArpTable { Task<IReadOnlyDictionary<string,string>> GetAllAsync(CancellationToken ct); }` (mapa IP→MAC)
  - `interface IVendorLookup { string? ResolveVendor(string? macAddress); }`
  - `static class ScanPorts { public static readonly IReadOnlyList<int> Default = new[]{22,23,53,80,161,443,445,515,631,3389,8080,9100}; }`

- [ ] **Step 1: Crear el archivo**

```csharp
using System.Net;

namespace NetAdminStudio.Application.Abstractions;

public sealed record HostProbe(bool Reachable, string? Hostname, double? LatencyMs);

public interface INetworkScanner
{
    Task<HostProbe> ProbeHostAsync(IPAddress ip, CancellationToken ct);
    Task<IReadOnlyList<int>> ScanPortsAsync(
        IPAddress ip, IReadOnlyList<int> ports, CancellationToken ct);
}

public interface IArpTable
{
    // Mapa dirección IP (string) -> MAC normalizada "AA-BB-CC-11-22-33".
    Task<IReadOnlyDictionary<string, string>> GetAllAsync(CancellationToken ct);
}

public interface IVendorLookup
{
    string? ResolveVendor(string? macAddress);
}

public static class ScanPorts
{
    public static readonly IReadOnlyList<int> Default =
        new[] { 22, 23, 53, 80, 161, 443, 445, 515, 631, 3389, 8080, 9100 };
}
```

- [ ] **Step 2: Verificar compilación**

Run: `dotnet build src/NetAdminStudio.Application --nologo`
Expected: Build succeeded.

- [ ] **Step 3: Commit**

```bash
git add src/NetAdminStudio.Application/Abstractions/NetworkPorts.cs
git commit -m "Agrega abstracciones de escaneo de red (scanner, ARP, OUI)"
```

---

### Task 5: Clasificador de dispositivos (lógica pura)

**Files:**
- Create: `src/NetAdminStudio.Application/Networking/AssetClassifier.cs`
- Test: `tests/NetAdminStudio.Tests/AssetClassifierTests.cs`

**Interfaces:**
- Consumes: `AssetType` (Domain).
- Produces: `static AssetType Classify(IReadOnlyList<int> openPorts, string? vendor)`.

- [ ] **Step 1: Escribir el test que falla**

```csharp
using NetAdminStudio.Application.Networking;
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Tests;

public sealed class AssetClassifierTests
{
    [Fact]
    public void PrinterPorts_ClassifiedAsPrinter() =>
        Assert.Equal(AssetType.Printer,
            AssetClassifier.Classify(new[] { 9100, 631 }, null));

    [Fact]
    public void SnmpWithNetworkVendor_ClassifiedAsSwitch() =>
        Assert.Equal(AssetType.Switch,
            AssetClassifier.Classify(new[] { 161, 80 }, "TP-Link"));

    [Fact]
    public void SmbAndRdp_ClassifiedAsWorkstation() =>
        Assert.Equal(AssetType.Workstation,
            AssetClassifier.Classify(new[] { 445, 3389 }, "Dell"));

    [Fact]
    public void NoSignals_ClassifiedAsUnknown() =>
        Assert.Equal(AssetType.Unknown,
            AssetClassifier.Classify(Array.Empty<int>(), null));
}
```

- [ ] **Step 2: Ejecutar y verificar que falla**

Run: `dotnet test --filter FullyQualifiedName~AssetClassifierTests --nologo`
Expected: FAIL (no compila).

- [ ] **Step 3: Implementación**

```csharp
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Application.Networking;

public static class AssetClassifier
{
    private static readonly string[] NetworkVendors =
        { "tp-link", "cisco", "mikrotik", "ubiquiti", "netgear", "d-link", "huawei", "aruba" };

    public static AssetType Classify(IReadOnlyList<int> openPorts, string? vendor)
    {
        bool Has(int p) => openPorts.Contains(p);
        var v = vendor?.ToLowerInvariant() ?? "";
        var networkVendor = NetworkVendors.Any(nv => v.Contains(nv));

        if (Has(9100) || Has(515) || Has(631))
            return AssetType.Printer;

        if (Has(161) && networkVendor)
            return AssetType.Switch;

        if (Has(445) && Has(3389))
            return AssetType.Workstation;

        if (Has(445))
            return AssetType.Server;

        return AssetType.Unknown;
    }
}
```

- [ ] **Step 4: Ejecutar y verificar que pasa**

Run: `dotnet test --filter FullyQualifiedName~AssetClassifierTests --nologo`
Expected: PASS (4 tests).

- [ ] **Step 5: Commit**

```bash
git add src/NetAdminStudio.Application/Networking/AssetClassifier.cs tests/NetAdminStudio.Tests/AssetClassifierTests.cs
git commit -m "Agrega clasificador de dispositivos por puertos y fabricante"
```

---

### Task 6: Lookup de fabricante por OUI (datos locales)

**Files:**
- Create: `src/NetAdminStudio.Infrastructure/Networking/OuiData.cs`
- Create: `src/NetAdminStudio.Infrastructure/Networking/OuiVendorLookup.cs`
- Test: `tests/NetAdminStudio.Tests/OuiVendorLookupTests.cs`

**Interfaces:**
- Consumes: `IVendorLookup` (Task 4).
- Produces: `OuiVendorLookup : IVendorLookup`. Normaliza la MAC a los primeros 3 octetos y busca en `OuiData.Map` (`Dictionary<string,string>` con clave `"AABBCC"`).

- [ ] **Step 1: Escribir el test que falla**

```csharp
using NetAdminStudio.Infrastructure.Networking;

namespace NetAdminStudio.Tests;

public sealed class OuiVendorLookupTests
{
    private readonly OuiVendorLookup _lookup = new();

    [Theory]
    [InlineData("50-C7-BF-11-22-33", "TP-Link")]   // 50C7BF es un OUI real de TP-Link
    [InlineData("50c7bf112233", "TP-Link")]
    [InlineData("50:C7:BF:11:22:33", "TP-Link")]
    public void ResolvesKnownVendor(string mac, string expected) =>
        Assert.Equal(expected, _lookup.ResolveVendor(mac));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("FF-FF-FF-00-00-00")]
    public void ReturnsNullForUnknown(string? mac) =>
        Assert.Null(_lookup.ResolveVendor(mac));
}
```

- [ ] **Step 2: Ejecutar y verificar que falla**

Run: `dotnet test --filter FullyQualifiedName~OuiVendorLookupTests --nologo`
Expected: FAIL (no compila).

- [ ] **Step 3: Implementación**

`OuiData.cs` (semilla pequeña de OUIs comunes; se puede ampliar luego):

```csharp
namespace NetAdminStudio.Infrastructure.Networking;

internal static class OuiData
{
    // Prefijo OUI (6 hex mayúsculas, sin separadores) -> fabricante.
    public static readonly IReadOnlyDictionary<string, string> Map =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["50C7BF"] = "TP-Link",
            ["1C61B4"] = "TP-Link",
            ["001B63"] = "Apple",
            ["3C0754"] = "Apple",
            ["F0DEF1"] = "Wistron",
            ["001A2B"] = "Cisco",
            ["00D0BC"] = "Cisco",
            ["DCA632"] = "Raspberry Pi",
            ["B827EB"] = "Raspberry Pi",
            ["E45F01"] = "Raspberry Pi",
            ["24A43C"] = "Ubiquiti",
            ["744401"] = "Netgear",
            ["001E2A"] = "Netgear",
            ["4CCC6A"] = "Murata",
            ["001321"] = "Dell",
            ["B083FE"] = "Dell",
            ["3417EB"] = "Dell",
            ["001560"] = "HP",
            ["A0481C"] = "HP",
            ["00219B"] = "Dell",
            ["0000AA"] = "Xerox",
            ["001279"] = "HP",
            ["0004E2"] = "SMC",
            ["001CC0"] = "Intel",
            ["8C1645"] = "Brother",
            ["008077"] = "Brother",
            ["44D9E7"] = "Ubiquiti",
        };
}
```

`OuiVendorLookup.cs`:

```csharp
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.Networking;

public sealed class OuiVendorLookup : IVendorLookup
{
    public string? ResolveVendor(string? macAddress)
    {
        if (string.IsNullOrWhiteSpace(macAddress))
            return null;

        var hex = new string(macAddress
            .Where(Uri.IsHexDigit)
            .ToArray());

        if (hex.Length < 6)
            return null;

        var prefix = hex[..6].ToUpperInvariant();
        return OuiData.Map.TryGetValue(prefix, out var vendor) ? vendor : null;
    }
}
```

- [ ] **Step 4: Ejecutar y verificar que pasa**

Run: `dotnet test --filter FullyQualifiedName~OuiVendorLookupTests --nologo`
Expected: PASS (6 casos).

- [ ] **Step 5: Commit**

```bash
git add src/NetAdminStudio.Infrastructure/Networking/Oui*.cs tests/NetAdminStudio.Tests/OuiVendorLookupTests.cs
git commit -m "Agrega lookup de fabricante por OUI con datos locales"
```

---

### Task 7: Tabla ARP (parser puro + implementación de SO)

**Files:**
- Create: `src/NetAdminStudio.Infrastructure/Networking/ArpTable.cs`
- Test: `tests/NetAdminStudio.Tests/ArpTableParserTests.cs`

**Interfaces:**
- Consumes: `IArpTable` (Task 4).
- Produces: `ArpTable : IArpTable`. `GetAllAsync` ejecuta `arp -a` y usa `static IReadOnlyDictionary<string,string> Parse(string arpOutput)` (público y estático para testear). MAC normalizada con guiones y mayúsculas.

- [ ] **Step 1: Escribir el test que falla** (parser puro, sin ejecutar `arp`)

```csharp
using NetAdminStudio.Infrastructure.Networking;

namespace NetAdminStudio.Tests;

public sealed class ArpTableParserTests
{
    private const string Sample = @"
Interfaz: 192.168.0.14 --- 0xb
  Dirección de Internet     Dirección física      Tipo
  192.168.0.1           50-c7-bf-11-22-33     dinámico
  192.168.0.50          8c-16-45-aa-bb-cc     dinámico
  192.168.0.255         ff-ff-ff-ff-ff-ff     estático
";

    [Fact]
    public void Parse_ExtractsIpToMacPairs()
    {
        var map = ArpTable.Parse(Sample);
        Assert.Equal("50-C7-BF-11-22-33", map["192.168.0.1"]);
        Assert.Equal("8C-16-45-AA-BB-CC", map["192.168.0.50"]);
    }

    [Fact]
    public void Parse_IgnoresBroadcastAndMulticast()
    {
        var map = ArpTable.Parse(Sample);
        Assert.False(map.ContainsKey("192.168.0.255"));
    }
}
```

- [ ] **Step 2: Ejecutar y verificar que falla**

Run: `dotnet test --filter FullyQualifiedName~ArpTableParserTests --nologo`
Expected: FAIL (no compila).

- [ ] **Step 3: Implementación**

```csharp
using System.Diagnostics;
using System.Text.RegularExpressions;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.Networking;

public sealed partial class ArpTable : IArpTable
{
    [GeneratedRegex(@"(\d{1,3}(?:\.\d{1,3}){3})\s+([0-9a-fA-F]{2}(?:[-:][0-9a-fA-F]{2}){5})")]
    private static partial Regex LineRegex();

    public static IReadOnlyDictionary<string, string> Parse(string arpOutput)
    {
        var map = new Dictionary<string, string>();
        foreach (Match m in LineRegex().Matches(arpOutput))
        {
            var ip = m.Groups[1].Value;
            var mac = m.Groups[2].Value.Replace(':', '-').ToUpperInvariant();
            if (mac is "FF-FF-FF-FF-FF-FF" or "00-00-00-00-00-00")
                continue;
            if (ip.EndsWith(".255"))
                continue;
            map[ip] = mac;
        }
        return map;
    }

    public async Task<IReadOnlyDictionary<string, string>> GetAllAsync(CancellationToken ct)
    {
        try
        {
            var psi = new ProcessStartInfo("arp", "-a")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = Process.Start(psi);
            if (process is null)
                return new Dictionary<string, string>();

            var output = await process.StandardOutput.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);
            return Parse(output);
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}
```

- [ ] **Step 4: Ejecutar y verificar que pasa**

Run: `dotnet test --filter FullyQualifiedName~ArpTableParserTests --nologo`
Expected: PASS (2 tests).

- [ ] **Step 5: Commit**

```bash
git add src/NetAdminStudio.Infrastructure/Networking/ArpTable.cs tests/NetAdminStudio.Tests/ArpTableParserTests.cs
git commit -m "Agrega tabla ARP con parser testeable"
```

---

### Task 8: Scanner TCP/ping (implementación de Infrastructure)

**Files:**
- Create: `src/NetAdminStudio.Infrastructure/Networking/TcpPortScanner.cs`

**Interfaces:**
- Consumes: `INetworkScanner`, `HostProbe` (Task 4).
- Produces: `TcpPortScanner : INetworkScanner`. Ping con `System.Net.NetworkInformation.Ping` (timeout 2 s) + DNS inverso best-effort; puertos con `TcpClient.ConnectAsync` (timeout 500 ms).

- [ ] **Step 1: Implementar** (sin test unitario directo: toca la red; se valida vía el motor con fakes en Task 9 y en el smoke test manual)

```csharp
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.Networking;

public sealed class TcpPortScanner : INetworkScanner
{
    public async Task<HostProbe> ProbeHostAsync(IPAddress ip, CancellationToken ct)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(ip, TimeSpan.FromSeconds(2));
            if (reply.Status != IPStatus.Success)
                return new HostProbe(false, null, null);

            string? hostname = null;
            try
            {
                var entry = await Dns.GetHostEntryAsync(ip, ct);
                hostname = entry.HostName;
            }
            catch { /* sin DNS inverso */ }

            return new HostProbe(true, hostname, reply.RoundtripTime);
        }
        catch
        {
            return new HostProbe(false, null, null);
        }
    }

    public async Task<IReadOnlyList<int>> ScanPortsAsync(
        IPAddress ip, IReadOnlyList<int> ports, CancellationToken ct)
    {
        var open = new List<int>();
        foreach (var port in ports)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                using var client = new TcpClient();
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
                timeout.CancelAfter(TimeSpan.FromMilliseconds(500));
                await client.ConnectAsync(ip, port, timeout.Token);
                if (client.Connected)
                    open.Add(port);
            }
            catch { /* cerrado / filtrado / timeout */ }
        }
        return open;
    }
}
```

- [ ] **Step 2: Verificar compilación**

Run: `dotnet build src/NetAdminStudio.Infrastructure --nologo`
Expected: Build succeeded.

- [ ] **Step 3: Commit**

```bash
git add src/NetAdminStudio.Infrastructure/Networking/TcpPortScanner.cs
git commit -m "Agrega scanner TCP/ping con DNS inverso"
```

---

### Task 9: Motor de descubrimiento (orquestación de las 4 fases)

**Files:**
- Create: `src/NetAdminStudio.Application/Networking/NetworkDiscoveryEngine.cs`
- Test: `tests/NetAdminStudio.Tests/NetworkDiscoveryEngineTests.cs`

**Interfaces:**
- Consumes: `Cidr` (T1), `AssetClassifier` (T5), `INetworkScanner`/`IArpTable`/`IVendorLookup`/`ScanPorts` (T4), `IAssetRepository` (existente), `NetworkAsset.RecordDiscovery` (T2).
- Produces:
  - `record DiscoveredHost(string Ip, string? Hostname, string? Mac, string? Vendor, IReadOnlyList<int> OpenPorts, AssetType Type, double? LatencyMs)`
  - `NetworkDiscoveryEngine(INetworkScanner, IArpTable, IVendorLookup, IAssetRepository)` con `Task<IReadOnlyList<DiscoveredHost>> ScanAsync(string cidr, IProgress<ScanProgress>? progress, CancellationToken ct)`.
  - `record ScanProgress(int Total, int Completed, int Found)`.

- [ ] **Step 1: Escribir el test que falla** (con fakes en memoria)

```csharp
using System.Net;
using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Application.Networking;
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Tests;

public sealed class NetworkDiscoveryEngineTests
{
    private sealed class FakeScanner : INetworkScanner
    {
        public Task<HostProbe> ProbeHostAsync(IPAddress ip, CancellationToken ct)
        {
            // Solo .1 responde.
            var reachable = ip.ToString().EndsWith(".1");
            return Task.FromResult(new HostProbe(
                reachable, reachable ? "router.local" : null, reachable ? 2.0 : null));
        }

        public Task<IReadOnlyList<int>> ScanPortsAsync(
            IPAddress ip, IReadOnlyList<int> ports, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<int>>(new[] { 80, 161 });
    }

    private sealed class FakeArp : IArpTable
    {
        public Task<IReadOnlyDictionary<string, string>> GetAllAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyDictionary<string, string>>(
                new Dictionary<string, string> { ["192.168.0.1"] = "50-C7-BF-11-22-33" });
    }

    private sealed class FakeVendor : IVendorLookup
    {
        public string? ResolveVendor(string? mac) => mac is null ? null : "TP-Link";
    }

    private sealed class InMemoryAssets : IAssetRepository
    {
        public readonly List<NetworkAsset> Saved = new();
        public Task<IReadOnlyList<NetworkAsset>> GetAllAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<NetworkAsset>>(Saved);
        public Task UpsertAsync(NetworkAsset asset, CancellationToken ct)
        {
            Saved.RemoveAll(a => a.IpAddress == asset.IpAddress);
            Saved.Add(asset);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task ScanAsync_DiscoversReachableHostAndPersists()
    {
        var assets = new InMemoryAssets();
        var engine = new NetworkDiscoveryEngine(
            new FakeScanner(), new FakeArp(), new FakeVendor(), assets);

        var found = await engine.ScanAsync("192.168.0.0/30", null, CancellationToken.None);

        var host = Assert.Single(found);
        Assert.Equal("192.168.0.1", host.Ip);
        Assert.Equal("TP-Link", host.Vendor);
        Assert.Equal(AssetType.Switch, host.Type);          // 161 + TP-Link
        Assert.Single(assets.Saved);
        Assert.Equal("discovery", assets.Saved[0].Origin);
    }
}
```

- [ ] **Step 2: Ejecutar y verificar que falla**

Run: `dotnet test --filter FullyQualifiedName~NetworkDiscoveryEngineTests --nologo`
Expected: FAIL (no compila).

- [ ] **Step 3: Implementación**

```csharp
using System.Net;
using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Application.Networking;

public sealed record DiscoveredHost(
    string Ip, string? Hostname, string? Mac, string? Vendor,
    IReadOnlyList<int> OpenPorts, AssetType Type, double? LatencyMs);

public sealed record ScanProgress(int Total, int Completed, int Found);

public sealed class NetworkDiscoveryEngine(
    INetworkScanner scanner,
    IArpTable arp,
    IVendorLookup vendorLookup,
    IAssetRepository assets)
{
    private const int PingConcurrency = 32;

    public async Task<IReadOnlyList<DiscoveredHost>> ScanAsync(
        string cidr, IProgress<ScanProgress>? progress, CancellationToken ct)
    {
        var hosts = Cidr.Hosts(cidr);
        var arpMap = await arp.GetAllAsync(ct);

        var found = new List<DiscoveredHost>();
        var completed = 0;
        using var gate = new SemaphoreSlim(PingConcurrency);

        var tasks = hosts.Select(async ip =>
        {
            await gate.WaitAsync(ct);
            try
            {
                var probe = await scanner.ProbeHostAsync(ip, ct);
                if (probe.Reachable)
                {
                    var openPorts = await scanner.ScanPortsAsync(ip, ScanPorts.Default, ct);
                    arpMap.TryGetValue(ip.ToString(), out var mac);
                    var vendor = vendorLookup.ResolveVendor(mac);
                    var type = AssetClassifier.Classify(openPorts, vendor);

                    var host = new DiscoveredHost(
                        ip.ToString(), probe.Hostname, mac, vendor,
                        openPorts, type, probe.LatencyMs);

                    lock (found) found.Add(host);
                    await PersistAsync(host, ct);
                }
            }
            finally
            {
                gate.Release();
                var done = Interlocked.Increment(ref completed);
                progress?.Report(new ScanProgress(hosts.Count, done, found.Count));
            }
        });

        await Task.WhenAll(tasks);
        return found.OrderBy(h => h.Ip).ToList();
    }

    private async Task PersistAsync(DiscoveredHost host, CancellationToken ct)
    {
        var existing = (await assets.GetAllAsync(ct))
            .FirstOrDefault(a => a.IpAddress == host.Ip);

        var asset = existing ?? new NetworkAsset { Name = host.Hostname ?? host.Ip };
        asset.IpAddress = host.Ip;
        if (existing is null && host.Hostname is not null)
            asset.Name = host.Hostname;

        asset.RecordDiscovery(
            host.Hostname, host.Mac, host.Vendor,
            host.OpenPorts, host.Type, host.LatencyMs, DateTimeOffset.UtcNow);

        await assets.UpsertAsync(asset, ct);
    }
}
```

- [ ] **Step 4: Ejecutar y verificar que pasa**

Run: `dotnet test --filter FullyQualifiedName~NetworkDiscoveryEngineTests --nologo`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/NetAdminStudio.Application/Networking/NetworkDiscoveryEngine.cs tests/NetAdminStudio.Tests/NetworkDiscoveryEngineTests.cs
git commit -m "Agrega motor de descubrimiento en 4 fases con persistencia"
```

---

### Task 10: Gestor de trabajos de escaneo + wiring de DI

**Files:**
- Create: `src/NetAdminStudio.Application/Networking/ScanJobManager.cs`
- Modify: `src/NetAdminStudio.Application/DependencyInjection.cs`
- Modify: `src/NetAdminStudio.Infrastructure/DependencyInjection.cs`

**Interfaces:**
- Consumes: `NetworkDiscoveryEngine`, `ScanProgress` (T9).
- Produces:
  - `record ScanStatus(Guid Id, string Cidr, bool Finished, int Total, int Completed, int Found, string? Error)`
  - `ScanJobManager` con `Guid Start(string cidr, Func<IServiceScopeFactory> ...)` — **ver nota**. Simplificamos: `Guid Start(string cidr, NetworkDiscoveryEngine engine)` que corre en background y `ScanStatus? Get(Guid id)`.

  Nota de diseño: para evitar problemas de scope de DI, `ScanJobManager` es singleton y recibe el `engine` por parámetro en `Start` (el endpoint lo resuelve del scope de la request y lo pasa). El trabajo corre en `Task.Run`; el estado se guarda en un `ConcurrentDictionary`.

- [ ] **Step 1: Implementar `ScanJobManager`**

```csharp
using System.Collections.Concurrent;

namespace NetAdminStudio.Application.Networking;

public sealed record ScanStatus(
    Guid Id, string Cidr, bool Finished,
    int Total, int Completed, int Found, string? Error);

public sealed class ScanJobManager
{
    private readonly ConcurrentDictionary<Guid, ScanStatus> _jobs = new();

    public Guid Start(string cidr, NetworkDiscoveryEngine engine)
    {
        var id = Guid.NewGuid();
        _jobs[id] = new ScanStatus(id, cidr, false, 0, 0, 0, null);

        _ = Task.Run(async () =>
        {
            try
            {
                var progress = new Progress<ScanProgress>(p =>
                    _jobs[id] = _jobs[id] with
                    {
                        Total = p.Total, Completed = p.Completed, Found = p.Found
                    });

                await engine.ScanAsync(cidr, progress, CancellationToken.None);
                _jobs[id] = _jobs[id] with { Finished = true };
            }
            catch (Exception ex)
            {
                _jobs[id] = _jobs[id] with { Finished = true, Error = ex.Message };
            }
        });

        return id;
    }

    public ScanStatus? Get(Guid id) => _jobs.TryGetValue(id, out var s) ? s : null;
}
```

- [ ] **Step 2: Registrar servicios**

En `Application/DependencyInjection.cs`, dentro de `AddApplication`, añadir:

```csharp
        services.AddScoped<NetworkDiscoveryEngine>();
        services.AddSingleton<ScanJobManager>();
```
(añadir `using NetAdminStudio.Application.Networking;` arriba).

En `Infrastructure/DependencyInjection.cs`, dentro de `AddInfrastructure`, añadir:

```csharp
        services.AddSingleton<INetworkScanner, TcpPortScanner>();
        services.AddSingleton<IArpTable, ArpTable>();
        services.AddSingleton<IVendorLookup, OuiVendorLookup>();
```

- [ ] **Step 3: Verificar compilación y tests**

Run: `dotnet build --nologo && dotnet test --nologo`
Expected: Build succeeded; todos los tests pasan.

- [ ] **Step 4: Commit**

```bash
git add src/NetAdminStudio.Application src/NetAdminStudio.Infrastructure
git commit -m "Agrega gestor de trabajos de escaneo y registra servicios de red"
```

---

### Task 11: Endpoints de escaneo en la API

**Files:**
- Modify: `src/NetAdminStudio.Api/Program.cs`

**Interfaces:**
- Consumes: `ScanJobManager`, `NetworkDiscoveryEngine`, `ScanStatus` (T9/T10).
- Produces: `POST /api/v1/network/scan` → `{ scanId }`; `GET /api/v1/network/scan/{id}` → `ScanStatus`.

- [ ] **Step 1: Añadir endpoints** (antes de `app.Run();`, junto a los demás `MapX`)

```csharp
app.MapPost("/api/v1/network/scan",
    (NetworkScanRequest request, ScanJobManager jobs,
     NetworkDiscoveryEngine engine) =>
    {
        try
        {
            // Valida el CIDR temprano (lanza si es inválido o demasiado grande).
            _ = NetAdminStudio.Application.Networking.Cidr.Hosts(request.Cidr);
            var id = jobs.Start(request.Cidr, engine);
            return Results.Accepted($"/api/v1/network/scan/{id}", new { scanId = id });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    });

app.MapGet("/api/v1/network/scan/{id:guid}",
    (Guid id, ScanJobManager jobs) =>
    {
        var status = jobs.Get(id);
        return status is null ? Results.NotFound() : Results.Ok(status);
    });
```

Y al final del archivo, junto a `AssistantRequest`:

```csharp
public sealed record NetworkScanRequest(string Cidr);
```

Añadir `using NetAdminStudio.Application.Networking;` arriba.

- [ ] **Step 2: Smoke test manual de la API**

Run (una terminal): `cd f:/NetAdminStudio && dotnet run --project src/NetAdminStudio.Api`
En otra terminal:
```bash
curl -s -X POST http://localhost:5188/api/v1/network/scan \
  -H "Content-Type: application/json" -d '{"cidr":"192.168.0.0/29"}'
```
Expected: `202` con `{"scanId":"..."}`. Luego `GET /api/v1/network/scan/<id>` devuelve estado con `total`, `completed`, y al terminar `finished:true`. Verificar `GET /api/v1/assets` muestra los hosts reales descubiertos. Detener con Ctrl+C.

> Si el puerto difiere, ver la URL que imprime `dotnet run` (buscar `Now listening on`).

- [ ] **Step 3: Commit**

```bash
git add src/NetAdminStudio.Api/Program.cs
git commit -m "Agrega endpoints de escaneo de red a la API"
```

---

### Task 12: Vista "Red" en la app de escritorio

**Files:**
- Modify: `src/NetAdminStudio.Desktop/Services/NetAdminApiClient.cs`
- Modify: `src/NetAdminStudio.Desktop/ViewModels/MainViewModel.cs`
- Modify: `src/NetAdminStudio.Desktop/MainWindow.xaml`

**Interfaces:**
- Consumes: endpoints de T11.
- Produces: en `NetAdminApiClient`, `Task<Guid> StartScanAsync(string cidr, CancellationToken ct)` y `Task<ScanStatusDto> GetScanStatusAsync(Guid id, CancellationToken ct)`; en `MainViewModel`, `ScanCommand`, `ScanCidr`, `ScanStatusText`. DTOs nuevos.

- [ ] **Step 1: Métodos en el ApiClient** (seguir el patrón de los métodos existentes `GetAssetsAsync`, etc. Añadir DTOs y métodos)

```csharp
public sealed record ScanStartedDto(Guid ScanId);
public sealed record ScanStatusDto(
    Guid Id, string Cidr, bool Finished, int Total, int Completed, int Found, string? Error);
```

```csharp
    public async Task<Guid> StartScanAsync(string cidr, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync(
            "/api/v1/network/scan", new { cidr }, ct);
        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<ScanStartedDto>(cancellationToken: ct);
        return dto!.ScanId;
    }

    public async Task<ScanStatusDto> GetScanStatusAsync(Guid id, CancellationToken ct) =>
        (await _http.GetFromJsonAsync<ScanStatusDto>(
            $"/api/v1/network/scan/{id}", ct))!;
```
(usar el mismo nombre de campo del `HttpClient` que ya exista en la clase; si es `apiClient`/`client`, ajustar).

- [ ] **Step 2: Comando en el ViewModel**

```csharp
    [ObservableProperty] private string scanCidr = "192.168.0.0/24";
    [ObservableProperty] private string scanStatusText = "Listo para escanear.";

    [RelayCommand]
    private async Task ScanAsync()
    {
        try
        {
            ScanStatusText = $"Iniciando escaneo de {ScanCidr}…";
            var id = await apiClient.StartScanAsync(ScanCidr, CancellationToken.None);

            while (true)
            {
                await Task.Delay(1000);
                var status = await apiClient.GetScanStatusAsync(id, CancellationToken.None);
                ScanStatusText =
                    $"Escaneando {status.Cidr}: {status.Completed}/{status.Total} " +
                    $"({status.Found} encontrados)";
                if (status.Finished)
                {
                    ScanStatusText = status.Error is null
                        ? $"Escaneo completo: {status.Found} dispositivos."
                        : $"Error: {status.Error}";
                    break;
                }
            }
            await RefreshAsync();   // recarga la grilla de activos con lo descubierto
        }
        catch (Exception ex)
        {
            ScanStatusText = $"No se pudo escanear: {ex.Message}";
        }
    }
```

- [ ] **Step 3: UI en MainWindow.xaml**

Añadir (en la zona de la vista de red / cerca de la grilla de Assets) un panel con el campo de rango, botón y estado:

```xml
<StackPanel Orientation="Horizontal" Margin="8">
    <TextBlock Text="Rango:" VerticalAlignment="Center" Margin="0,0,6,0"/>
    <TextBox Width="160" Text="{Binding ScanCidr, UpdateSourceTrigger=PropertyChanged}"/>
    <Button Content="Escanear red" Command="{Binding ScanCommand}" Margin="6,0"/>
    <TextBlock Text="{Binding ScanStatusText}" VerticalAlignment="Center" Margin="12,0"/>
</StackPanel>
```

Asegurar que la grilla de activos existente muestre columnas útiles (IP, Hostname, Vendor, State). Si ya existe un `DataGrid`/`ItemsControl` para `Assets`, reutilizarlo.

- [ ] **Step 4: Verificar compilación**

Run: `dotnet build src/NetAdminStudio.Desktop --nologo`
Expected: Build succeeded.

- [ ] **Step 5: Smoke test end-to-end**

1. Terminal A: `dotnet run --project src/NetAdminStudio.Api`
2. Terminal B: `dotnet run --project src/NetAdminStudio.Desktop`
3. En la app: escribir el rango de tu red real, pulsar **Escanear red**, ver el progreso y luego los dispositivos reales en la grilla.

Expected: aparecen dispositivos reales de tu red (al menos el router/gateway) con IP, y donde aplique hostname/fabricante/estado.

- [ ] **Step 6: Commit**

```bash
git add src/NetAdminStudio.Desktop
git commit -m "Agrega vista Red con escaneo y progreso en la app de escritorio"
```

---

### Task 13: Publicar el .exe autónomo

**Files:**
- Create: `docs/COMO_EJECUTAR.md`

- [ ] **Step 1: Publicar el escritorio como .exe self-contained**

Run:
```bash
cd f:/NetAdminStudio
dotnet publish src/NetAdminStudio.Desktop -c Release -r win-x64 \
  --self-contained true -p:PublishSingleFile=true -o publish/desktop
dotnet publish src/NetAdminStudio.Api -c Release -r win-x64 \
  --self-contained true -p:PublishSingleFile=true -o publish/api
```
Expected: se generan `publish/desktop/NetAdminStudio.Desktop.exe` y `publish/api/NetAdminStudio.Api.exe`.

- [ ] **Step 2: Documentar cómo ejecutar** — crear `docs/COMO_EJECUTAR.md`

```markdown
# Cómo ejecutar NetAdmin Studio

## Opción rápida (desarrollo)
1. Abrir dos terminales en la carpeta del proyecto.
2. Terminal 1: `dotnet run --project src/NetAdminStudio.Api`
3. Terminal 2: `dotnet run --project src/NetAdminStudio.Desktop`
4. En la app, escribir el rango de red (ej. 192.168.0.0/24) y pulsar "Escanear red".

## Opción .exe (para usar/distribuir)
1. Ejecutar los dos comandos `dotnet publish` de la tarea 13.
2. Copiar las carpetas `publish/api` y `publish/desktop` al equipo destino (Windows 10/11 x64).
3. Ejecutar primero `NetAdminStudio.Api.exe`, luego `NetAdminStudio.Desktop.exe`.

> Nota: el escaneo de red y la lectura de la tabla ARP funcionan mejor si la app
> se ejecuta con permisos normales en la misma red a inspeccionar.
```

- [ ] **Step 3: Añadir `publish/` al .gitignore**

Añadir la línea `publish/` a `.gitignore`.

- [ ] **Step 4: Commit**

```bash
git add docs/COMO_EJECUTAR.md .gitignore
git commit -m "Publica .exe autónomo y documenta cómo ejecutar"
```

---

## Notas de cierre

- Después de completar todas las tareas, hacer `git push origin main` para subir a GitHub (el remoto ya está configurado).
- Fuera de alcance (fases siguientes): SNMP/WMI reales, topología visual, escaneo remoto vía Agent, impresoras, usuarios/permisos, reportes e IA.
