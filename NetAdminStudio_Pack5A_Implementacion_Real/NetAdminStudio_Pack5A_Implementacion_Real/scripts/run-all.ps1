$root = Resolve-Path "$PSScriptRoot\.."

Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$root\src\NetAdminStudio.Api'; dotnet run"
Start-Sleep -Seconds 4

Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$root\src\NetAdminStudio.Agent'; dotnet run"
Start-Sleep -Seconds 2

Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$root\src\NetAdminStudio.Desktop'; dotnet run"
