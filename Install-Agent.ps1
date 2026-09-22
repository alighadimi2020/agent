# Run this script as Administrator.
$ErrorActionPreference = "Stop"

$serviceName = "ArioTjaratAgent"
$displayName = "Ario Tjarat Agent"
$sourceExe = Join-Path $PSScriptRoot "ArioTjarat.Agent.exe"
$installDir = Join-Path $env:ProgramFiles "ArioTjarat\Agent"
$targetExe = Join-Path $installDir "ArioTjarat.Agent.exe"

if (-not (Test-Path $sourceExe)) {
    Write-Host "ArioTjarat.Agent.exe was not found next to this installer script." -ForegroundColor Red
    exit 1
}

New-Item -ItemType Directory -Path $installDir -Force | Out-Null
Copy-Item $sourceExe $targetExe -Force

$existing = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

if ($existing) {
    Stop-Service -Name $serviceName -Force -ErrorAction SilentlyContinue
    sc.exe delete $serviceName | Out-Null
    Start-Sleep -Seconds 2
}

sc.exe create $serviceName binPath= "`"$targetExe`"" start= auto DisplayName= "`"$displayName`"" | Out-Null
sc.exe description $serviceName "Ario Tjarat background activity agent." | Out-Null
sc.exe failure $serviceName reset= 86400 actions= restart/5000/restart/10000/restart/30000 | Out-Null

Start-Service -Name $serviceName

Write-Host ""
Write-Host "Ario Tjarat Agent installed and started successfully." -ForegroundColor Green
Write-Host "Service: $serviceName"
Write-Host "Data: C:\ProgramData\ArioTjarat\Agent"
