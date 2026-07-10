$ErrorActionPreference = "Stop"
Push-Location "$PSScriptRoot\.."
dotnet restore .\NetAdminStudio.sln
Pop-Location
