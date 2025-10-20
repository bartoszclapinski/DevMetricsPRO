# DevMetrics Pro - JWT Token Decoder
# This script decodes a JWT token and displays its contents

param(
    [Parameter(Mandatory=$false)]
    [string]$token = ""
)

# If no token provided, prompt user
if ([string]::IsNullOrEmpty($token)) {
    Write-Host "Enter JWT token to decode:" -ForegroundColor Yellow
    $token = Read-Host
}

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host "JWT Token Decoder" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Split token into parts
$parts = $token.Split('.')

if ($parts.Length -ne 3) {
    Write-Host "❌ Invalid JWT token format" -ForegroundColor Red
    Write-Host "Expected format: Header.Payload.Signature" -ForegroundColor Yellow
    exit 1
}

try {
    # Decode header
    Write-Host "📋 HEADER:" -ForegroundColor Yellow
    $headerPadded = $parts[0].Replace('-', '+').Replace('_', '/')
    $headerPadded = $headerPadded.PadRight($headerPadded.Length + (4 - $headerPadded.Length % 4) % 4, '=')
    $headerJson = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($headerPadded))
    $header = $headerJson | ConvertFrom-Json
    $header | Format-List
    Write-Host ""

    # Decode payload
    Write-Host "🔐 PAYLOAD (Claims):" -ForegroundColor Yellow
    $payloadPadded = $parts[1].Replace('-', '+').Replace('_', '/')
    $payloadPadded = $payloadPadded.PadRight($payloadPadded.Length + (4 - $payloadPadded.Length % 4) % 4, '=')
    $payloadJson = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($payloadPadded))
    $payload = $payloadJson | ConvertFrom-Json

    # Pretty print claims
    $payload | Get-Member -MemberType NoteProperty | ForEach-Object {
        $name = $_.Name
        $value = $payload.$name
        
        # Convert Unix timestamp for 'exp' claim
        if ($name -eq "exp") {
            try {
                $expDate = [DateTimeOffset]::FromUnixTimeSeconds($value).DateTime.ToLocalTime()
                $now = Get-Date
                $timeLeft = $expDate - $now
                
                Write-Host "  $name : $value" -ForegroundColor White
                Write-Host "    └─ Expires: $expDate" -ForegroundColor Gray
                
                if ($timeLeft.TotalSeconds -gt 0) {
                    Write-Host "    └─ Time left: $([math]::Floor($timeLeft.TotalMinutes)) minutes" -ForegroundColor Green
                } else {
                    Write-Host "    └─ ⚠️  Token EXPIRED!" -ForegroundColor Red
                }
            } catch {
                Write-Host "  $name : $value" -ForegroundColor White
            }
        } else {
            Write-Host "  $name : $value" -ForegroundColor White
        }
    }

    Write-Host ""
    Write-Host "🔒 SIGNATURE:" -ForegroundColor Yellow
    Write-Host "  $($parts[2].Substring(0, [Math]::Min(50, $parts[2].Length)))..." -ForegroundColor Gray
    Write-Host "  (Signature is encrypted - cannot be decoded)" -ForegroundColor Gray
    Write-Host ""

    Write-Host "✅ Token structure is valid" -ForegroundColor Green
    Write-Host "================================" -ForegroundColor Cyan
    
} catch {
    Write-Host "❌ Error decoding token: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Make sure you provided a valid JWT token." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "💡 TIP: You can pass the token as a parameter: .\decode-jwt-token.ps1 -token 'your-token-here'" -ForegroundColor Yellow

