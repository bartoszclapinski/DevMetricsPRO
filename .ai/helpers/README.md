# DevMetrics Pro - Helper Scripts

This directory contains utility scripts for testing and development.

## 📁 Scripts

### 🧪 `test-auth-endpoints.ps1`
**Purpose**: Test both Register and Login API endpoints in one go.

**Usage**:
```powershell
# Default test (uses testuser@devmetrics.com)
.\test-auth-endpoints.ps1

# Customize test user by editing variables at top of script
```

**What it does**:
- ✅ Tests `/api/auth/register` endpoint
- ✅ Tests `/api/auth/login` endpoint
- ✅ Shows response with tokens
- ✅ Handles errors gracefully

---

### 🔍 `decode-jwt-token.ps1`
**Purpose**: Decode and inspect JWT tokens.

**Usage**:
```powershell
# Interactive mode (prompts for token)
.\decode-jwt-token.ps1

# Direct mode (pass token as parameter)
.\decode-jwt-token.ps1 -token "eyJhbGci..."
```

**What it does**:
- ✅ Decodes JWT header
- ✅ Decodes JWT payload (claims)
- ✅ Shows token expiration time
- ✅ Calculates time remaining
- ✅ Validates token structure

---

### 🎯 `test-single-endpoint.ps1`
**Purpose**: Test individual endpoints with custom data.

**Usage**:
```powershell
# Test register
.\test-single-endpoint.ps1 -endpoint register -email "user@test.com" -password "Pass1234!"

# Test login
.\test-single-endpoint.ps1 -endpoint login -email "user@test.com" -password "Pass1234!"

# Interactive mode (prompts for missing parameters)
.\test-single-endpoint.ps1 -endpoint register
```

**What it does**:
- ✅ Tests single endpoint with custom credentials
- ✅ Copies JWT token to clipboard automatically
- ✅ Shows detailed error messages
- ✅ Supports interactive mode

---

## 🚀 Quick Start

1. **Start the application**:
   ```powershell
   dotnet run --project src/DevMetricsPro.Web
   ```

2. **Open PowerShell in this directory** (`.ai/helpers/`)

3. **Run a test script**:
   ```powershell
   # Test both endpoints
   .\test-auth-endpoints.ps1
   
   # Or test a specific endpoint
   .\test-single-endpoint.ps1 -endpoint register -email "test@dev.com" -password "Test1234!"
   ```

4. **Decode the JWT token**:
   ```powershell
   # The token is in your clipboard after running test-single-endpoint.ps1
   .\decode-jwt-token.ps1
   # Then paste the token when prompted
   ```

---

## 💡 Tips

### Token in Clipboard
After running `test-single-endpoint.ps1`, the JWT token is automatically copied to your clipboard! You can:
- Paste it into `decode-jwt-token.ps1` to inspect it
- Use it in Postman/Insomnia for testing protected endpoints
- Store it for manual testing

### Custom Base URL
All scripts default to `http://localhost:5234`. If your app runs on a different port, modify the `$baseUrl` variable.

### Execution Policy
If PowerShell blocks the scripts, run:
```powershell
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
```

Or run scripts with bypass flag:
```powershell
powershell -ExecutionPolicy Bypass -File .\test-auth-endpoints.ps1
```

---

## 🔧 Customization

### Changing Test Credentials
Edit `test-auth-endpoints.ps1` and modify these lines:
```powershell
$testEmail = "your-email@example.com"
$testPassword = "YourPassword123!"
```

### Changing Base URL
If your app runs on a different port:
```powershell
$baseUrl = "https://localhost:7270"
```

---

## 📝 Notes

- Scripts are designed for **Development** environment only
- JWT tokens expire after 60 minutes (configurable in `appsettings.json`)
- Make sure the application is running before executing scripts
- Scripts handle errors and show detailed error messages

---

## 🎓 Learning Resources

### Understanding the Scripts
- **Invoke-RestMethod**: PowerShell cmdlet for HTTP requests
- **ConvertTo-Json**: Converts PowerShell objects to JSON
- **Base64 Decoding**: JWT tokens are Base64URL encoded
- **Error Handling**: try-catch blocks for robust error handling

### JWT Structure
```
Header.Payload.Signature
│      │       └─ HMAC-SHA256 signature (encrypted)
│      └─ Claims (user data, expiration, etc.)
└─ Algorithm and token type
```

All three parts are Base64URL encoded!

---

## 🆘 Troubleshooting

### "Connection refused"
- ✅ Make sure the application is running
- ✅ Check the port number in `$baseUrl`

### "Invalid token format"
- ✅ Make sure you copied the entire token
- ✅ Token should have exactly 2 dots (3 parts)

### "User already exists"
- ✅ Change the email in test script
- ✅ Or use the login endpoint instead

---

**Happy Testing! 🚀**

