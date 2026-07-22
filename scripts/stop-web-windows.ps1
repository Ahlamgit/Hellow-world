# Stop old KHADAMATI / Vite dev servers on Windows
Write-Host "Stopping Node processes using ports 5173-5180..."

$ports = 5173..5180
foreach ($port in $ports) {
    $connections = netstat -ano | Select-String ":$port\s"
    foreach ($line in $connections) {
        $parts = ($line -replace '\s+', ' ').Trim().Split(' ')
        $pid = $parts[-1]
        if ($pid -match '^\d+$' -and $pid -ne '0') {
            Write-Host "  Killing PID $pid (port $port)"
            taskkill /PID $pid /F 2>$null | Out-Null
        }
    }
}

Write-Host "Done. You can now run: npm run dev"
