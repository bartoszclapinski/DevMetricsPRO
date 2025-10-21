# Kill process on specific port
# Usage: .\kill-port.ps1 -Port 5234

param(
    [Parameter(Mandatory=$false)]
    [int]$Port = 5234
)

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Port Killer - Port $Port" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

try {
    $connection = Get-NetTCPConnection -LocalPort $Port -ErrorAction Stop
    $pid = $connection.OwningProcess | Select-Object -First 1
    
    Write-Host "Znaleziono proces na porcie $Port" -ForegroundColor Green
    Write-Host ""
    
    $process = Get-Process -Id $pid
    Write-Host "Szczegóły procesu:" -ForegroundColor Yellow
    Write-Host "  PID:     $pid" -ForegroundColor White
    Write-Host "  Nazwa:   $($process.ProcessName)" -ForegroundColor White
    Write-Host "  Path:    $($process.Path)" -ForegroundColor White
    Write-Host "  Start:   $($process.StartTime)" -ForegroundColor White
    Write-Host ""
    
    Write-Host "Zabijam proces..." -ForegroundColor Red
    Stop-Process -Id $pid -Force
    
    Write-Host ""
    Write-Host "✅ Proces zatrzymany! Port $Port jest wolny." -ForegroundColor Green
    
} catch {
    Write-Host "❌ Nie znaleziono procesu na porcie $Port" -ForegroundColor Yellow
    Write-Host "   Port jest prawdopodobnie wolny." -ForegroundColor Gray
}

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan


