# KHADAMATI API — Windows quick start
# Default project path for Ahlam's machine
$ProjectRoot = "C:\Users\Ahlam\Documents\Khadamati"

if (-not (Test-Path $ProjectRoot)) {
    Write-Error "Project not found at $ProjectRoot. Clone Ahlamgit/Khadamati there or edit this script."
    exit 1
}

Set-Location "$ProjectRoot\src\backend"
Write-Host "Starting KHADAMATI API at http://localhost:5000 ..."
dotnet run --project Khadamati.API
