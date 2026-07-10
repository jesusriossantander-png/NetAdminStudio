$ErrorActionPreference = "Stop"

Write-Host "Validando SDK..."
dotnet --version

Write-Host "Restaurando..."
dotnet restore "$PSScriptRoot\..\NetAdminStudio.sln"

Write-Host "Compilando..."
dotnet build "$PSScriptRoot\..\NetAdminStudio.sln" -c Debug

Write-Host "Ejecutando pruebas..."
dotnet test "$PSScriptRoot\..\NetAdminStudio.sln"

Write-Host "Validación completada."
