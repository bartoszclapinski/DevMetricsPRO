# DevMetrics Pro - Single Endpoint Tester
# Flexible script for testing individual endpoints

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('register', 'login')]
    [string]$endpoint,
    
    [Parameter(Mandatory=$false)]
    [string]$email = "",
    
    [Parameter(Mandatory=$false)]
    [string]$password = "",
    
    [Parameter(Mandatory=$false)]
    [string]$baseUrl = "http://localhost:5234"
)

# Prompt for missing parameters
if ([string]::IsNullOrEmpty($email)) {
    $email = Read-Host "Enter email"
}

if ([string]::IsNullOrEmpty($password)) {
    $password = Read-Host "Enter password" -AsSecureString
    $password = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)
    )
}

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host "Testing $($endpoint.ToUpper()) endpoint" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

if ($endpoint -eq "register") {
    # Test Register
    $body = @{
        email = $email
        password = $password
        confirmPassword = $password
    } | ConvertTo-Json
    
    try {
        Write-Host "Sending request to: $baseUrl/api/auth/register" -ForegroundColor Gray
        Write-Host ""
        
        $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" `
            -Method POST `
            -Body $body `
            -ContentType "application/json" `
            -ErrorAction Stop
        
        Write-Host "✅ REGISTER SUCCESS!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Response:" -ForegroundColor Yellow
        Write-Host "  Email: $($response.email)" -ForegroundColor White
        Write-Host "  Display Name: $($response.displayName)" -ForegroundColor White
        Write-Host "  Expires At: $($response.expiresAt)" -ForegroundColor White
        Write-Host "  Token: $($response.token.Substring(0, 50))..." -ForegroundColor White
        Write-Host "  Refresh Token: $($response.refreshToken.Substring(0, 20))..." -ForegroundColor White
        Write-Host ""
        Write-Host "💾 Full token saved to clipboard!" -ForegroundColor Green
        
        # Copy token to clipboard
        $response.token | Set-Clipboard
        
    } catch {
        Write-Host "❌ REGISTER FAILED!" -ForegroundColor Red
        Write-Host ""
        Write-Host "Status Code: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
        
        # Try to get detailed error
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "Error Details:" -ForegroundColor Red
            Write-Host $responseBody -ForegroundColor Yellow
        } catch {
            Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }
    
} elseif ($endpoint -eq "login") {
    # Test Login
    $body = @{
        email = $email
        password = $password
        rememberMe = $false
    } | ConvertTo-Json
    
    try {
        Write-Host "Sending request to: $baseUrl/api/auth/login" -ForegroundColor Gray
        Write-Host ""
        
        $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
            -Method POST `
            -Body $body `
            -ContentType "application/json" `
            -ErrorAction Stop
        
        Write-Host "✅ LOGIN SUCCESS!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Response:" -ForegroundColor Yellow
        Write-Host "  Email: $($response.email)" -ForegroundColor White
        Write-Host "  Display Name: $($response.displayName)" -ForegroundColor White
        Write-Host "  Expires At: $($response.expiresAt)" -ForegroundColor White
        Write-Host "  Token: $($response.token.Substring(0, 50))..." -ForegroundColor White
        Write-Host "  Refresh Token: $($response.refreshToken.Substring(0, 20))..." -ForegroundColor White
        Write-Host ""
        Write-Host "💾 Full token saved to clipboard!" -ForegroundColor Green
        
        # Copy token to clipboard
        $response.token | Set-Clipboard
        
    } catch {
        Write-Host "❌ LOGIN FAILED!" -ForegroundColor Red
        Write-Host ""
        Write-Host "Status Code: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
        
        # Try to get detailed error
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "Error Details:" -ForegroundColor Red
            Write-Host $responseBody -ForegroundColor Yellow
        } catch {
            Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }
}

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host "Testing Complete!" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "💡 EXAMPLES:" -ForegroundColor Yellow
Write-Host "  .\test-single-endpoint.ps1 -endpoint register -email 'user@test.com' -password 'Pass1234!'" -ForegroundColor Gray
Write-Host "  .\test-single-endpoint.ps1 -endpoint login -email 'user@test.com' -password 'Pass1234!'" -ForegroundColor Gray

