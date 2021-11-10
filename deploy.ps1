$StartLocation = Get-Location
Set-Location $PSScriptRoot

$Args = $Args -join ''
dotnet run --project 'tools\Tooling.DevOps.Tool.Deploy\LanceC.Tooling.DevOps.Tool.Deploy.csproj' -- $Args

Set-Location $StartLocation
