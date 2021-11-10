$StartLocation = Get-Location
Set-Location $PSScriptRoot

$Args = $Args -join ''
dotnet run --project 'tools\Tooling.DevOps.Tool.Build\LanceC.Tooling.DevOps.Tool.Build.csproj' -- $Args

Set-Location $StartLocation
