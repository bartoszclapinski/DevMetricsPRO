# Kill DevMetricsPro development server
# Checks both dotnet processes and port 5234

Write-Host "================================" -ForegroundColor Cyan
Write-Host "DevMetricsPro Server Killer" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Method 1: Kill dotnet processes running from DevMetricsPro directory
Write-Host "1. Szukam procesów dotnet DevMetricsPro..." -ForegroundColor Yellow
# $currentPath = (Get-Location).Path
$processes = Get-Process -Name dotnet -ErrorAction SilentlyContinue | Where-Object { 
    try {
        # $processPath = $_.Path
        # Check if process is running from current directory
        (Get-CimInstance Win32_Process -Filter "ProcessId = $($_.Id)").CommandLine -like "*DevMetricsPro*"
    } catch {
        $false
    }
}

if ($processes) {
    Write-Host "   ✓ Znaleziono $($processes.Count) proces(y)" -ForegroundColor Green
    $processes | ForEach-Object {
        Write-Host "     - PID: $($_.Id), Start: $($_.StartTime)" -ForegroundColor Gray
    }
    $processes | Stop-Process -Force
    Write-Host "   ✓ Procesy dotnet zatrzymane" -ForegroundColor Green
} else {
    Write-Host "   - Brak procesów dotnet DevMetricsPro" -ForegroundColor Gray
}

Write-Host ""

# Method 2: Kill process on port 5234
Write-Host "2. Sprawdzam port 5234..." -ForegroundColor Yellow
try {
    $connections = @(Get-NetTCPConnection -LocalPort 5234 -ErrorAction Stop)
    $processId = ($connections | Select-Object -First 1).OwningProcess
    
    $process = Get-Process -Id $processId -ErrorAction Stop
    Write-Host "   ⚠ Port 5234 zajęty przez:" -ForegroundColor Yellow
    Write-Host "     - Proces: $($process.ProcessName)" -ForegroundColor Gray
    Write-Host "     - PID: $processId" -ForegroundColor Gray
    Write-Host "     - Path: $($process.Path)" -ForegroundColor Gray
    
    Write-Host "   ✓ Zabijam proces na porcie 5234..." -ForegroundColor Red
    Stop-Process -Id $processId -Force
    Write-Host "   ✓ Port 5234 zwolniony" -ForegroundColor Green
} catch {
    Write-Host "   ✓ Port 5234 jest wolny" -ForegroundColor Green
}

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host "✅ Gotowe! Możesz uruchomić serwer" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "💡 Uruchom: dotnet run --project .\src\DevMetricsPro.Web\" -ForegroundColor Yellow
