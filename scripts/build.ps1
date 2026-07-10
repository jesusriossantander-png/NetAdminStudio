$ErrorActionPreference = "Stop"
Push-Location "$PSScriptRoot\.."
dotnet build .\NetAdminStudio.sln -c Debug
Pop-Location
