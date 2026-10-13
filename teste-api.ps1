# teste-api.ps1
$baseUrl = "http://localhost:5211"

# Login
$login = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
    -Method Post -ContentType "application/json" `
    -Body (@{ login = "admin"; senha = "admin123" } | ConvertTo-Json)

$headers = @{ Authorization = "Bearer $($login.token)" }

# Lista perfis
Write-Host "`n=== Perfis ===" -ForegroundColor Cyan
Invoke-RestMethod -Uri "$baseUrl/api/perfis" -Headers $headers | ConvertTo-Json -Depth 5

# Lista pacotes
Write-Host "`n=== Pacotes ===" -ForegroundColor Cyan
Invoke-RestMethod -Uri "$baseUrl/api/pacotes" -Headers $headers | ConvertTo-Json -Depth 5

# Lista deploys
Write-Host "`n=== Deploys ===" -ForegroundColor Cyan
Invoke-RestMethod -Uri "$baseUrl/api/deploy" -Headers $headers | ConvertTo-Json -Depth 5