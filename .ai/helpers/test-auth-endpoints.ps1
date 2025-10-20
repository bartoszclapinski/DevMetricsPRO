# DevMetrics Pro - Authentication API Testing Script
# This script tests both Register and Login endpoints

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Testing DevMetrics Pro Auth API" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$baseUrl = "http://localhost:5234"
$testEmail = "testuser@devmetrics.com"
$testPassword = "TestUser1234!"

# Test 1: Register
Write-Host "1. Testing REGISTER endpoint..." -ForegroundColor Yellow
$registerBody = @{
    email = $testEmail
    password = $testPassword
    confirmPassword = $testPassword
} | ConvertTo-Json

try {
    $registerResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" `
        -Method POST `
        -Body $registerBody `
        -ContentType "application/json"
    
    Write-Host "✅ Register SUCCESS!" -ForegroundColor Green
    Write-Host "Email: $($registerResponse.email)" -ForegroundColor White
    Write-Host "Display Name: $($registerResponse.displayName)" -ForegroundColor White
    Write-Host "Token: $($registerResponse.token.Substring(0, 50))..." -ForegroundColor White
    Write-Host "Expires: $($registerResponse.expiresAt)" -ForegroundColor White
    Write-Host ""
    
    $token = $registerResponse.token
} catch {
    Write-Host "❌ Register FAILED!" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    
    # Try to get detailed error
    if ($_.Exception.Response) {
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "Error Details: $responseBody" -ForegroundColor Red
        } catch {
            # Ignore if we can't read response
        }
    }
    Write-Host ""
}

# Wait a moment
Start-Sleep -Seconds 1

# Test 2: Login
Write-Host "2. Testing LOGIN endpoint..." -ForegroundColor Yellow
$loginBody = @{
    email = $testEmail
    password = $testPassword
    rememberMe = $false
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
        -Method POST `
        -Body $loginBody `
        -ContentType "application/json"
    
    Write-Host "✅ Login SUCCESS!" -ForegroundColor Green
    Write-Host "Email: $($loginResponse.email)" -ForegroundColor White
    Write-Host "Display Name: $($loginResponse.displayName)" -ForegroundColor White
    Write-Host "Token: $($loginResponse.token.Substring(0, 50))..." -ForegroundColor White
    Write-Host "Expires: $($loginResponse.expiresAt)" -ForegroundColor White
    Write-Host ""
    
    $token = $loginResponse.token
} catch {
    Write-Host "❌ Login FAILED!" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    
    # Try to get detailed error
    if ($_.Exception.Response) {
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "Error Details: $responseBody" -ForegroundColor Red
        } catch {
            # Ignore if we can't read response
        }
    }
    Write-Host ""
}

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Testing Complete!" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "💡 TIP: You can customize the test user by editing the variables at the top of this script" -ForegroundColor Yellow
Write-Host "💡 TIP: Make sure the application is running before executing this script" -ForegroundColor Yellow

