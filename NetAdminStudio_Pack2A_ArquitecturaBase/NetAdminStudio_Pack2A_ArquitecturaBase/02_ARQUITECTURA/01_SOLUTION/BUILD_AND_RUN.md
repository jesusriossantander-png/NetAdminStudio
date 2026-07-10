# Compilación y Ejecución

## Requisitos

- .NET 8 SDK.
- Visual Studio 2022 o VS Code.
- Windows 10/11.

## Comandos iniciales

```bash
dotnet restore
dotnet build
dotnet test
```

## Configuraciones

- Debug
- Release
- Production

## Publicación inicial

```bash
dotnet publish src/NetAdmin.Console -c Release -r win-x64 --self-contained false
```

## Regla

La solución debe compilar desde línea de comandos sin depender del IDE.
