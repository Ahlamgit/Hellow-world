# KHADAMATI Web — Windows quick start
$ProjectRoot = "C:\Users\Ahlam\Documents\Khadamati"

if (-not (Test-Path $ProjectRoot)) {
    Write-Error "Project not found at $ProjectRoot. Clone Ahlamgit/Khadamati there or edit this script."
    exit 1
}

Set-Location "$ProjectRoot\src\web"

if (Test-Path ".env") {
    $envContent = Get-Content ".env" -Raw
    if ($envContent -match "VITE_API_URL\s*=\s*/api/v1") {
        Write-Warning "src\web\.env uses VITE_API_URL=/api/v1 which can cause connection issues."
        Write-Warning "Change it to VITE_API_URL=http://localhost:5000/api/v1 or delete .env"
    }
}

# Free ports 5173+ if old Vite instances are still running
$stopScript = Join-Path $ProjectRoot "scripts\stop-web-windows.ps1"
if (Test-Path $stopScript) {
    & $stopScript
}

Write-Host "Starting KHADAMATI web dev server on http://localhost:5173 ..."
npm run dev
