$serviceName = "DisplayCountWatcherService"
$exePath = Join-Path $PSScriptRoot "DisplayCountWatcherService.exe"

if (Get-Service -Name $serviceName -ErrorAction SilentlyContinue) {
    Write-Host "Service already exists. Restarting it..."
    Restart-Service -Name $serviceName
} else {
    Write-Host "Creating service..."
    sc.exe create $serviceName binPath= "`"$exePath`""
    Start-Service -Name $serviceName
    Write-Host "Service installed and started."
}
