# Run this script as Administrator.
$serviceName = "ArioTjaratAgent"

$existing = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

if ($existing) {
    Stop-Service -Name $serviceName -Force -ErrorAction SilentlyContinue
    sc.exe delete $serviceName | Out-Null
    Write-Host "Ario Tjarat Agent service removed." -ForegroundColor Green
}
else {
    Write-Host "Ario Tjarat Agent service is not installed."
}
