$ErrorActionPreference = "Stop"
Push-Location "$PSScriptRoot\.."
dotnet test .\NetAdminStudio.sln
Pop-Location
