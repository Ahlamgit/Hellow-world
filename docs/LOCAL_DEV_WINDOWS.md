# KHADAMATI — Windows local development (Ahlam)

## Your machine

| Item | Value |
|------|-------|
| **GitHub repo** | [Ahlamgit/Khadamati](https://github.com/Ahlamgit/Khadamati) |
| **Project folder** | `C:\Users\Ahlam\Documents\Khadamati` |
| **API** | http://localhost:5000 |
| **Web (Vite)** | http://localhost:5173 (or the port Vite prints) |
| **SQL Server** | Local SQL Server 2014 on `localhost` |
| **Admin login** | `admin@khadamati.com` / `Admin@123456` |

## First-time setup

```powershell
cd C:\Users\Ahlam\Documents\Khadamati
git pull
```

### 1. Backend (API)

```powershell
cd C:\Users\Ahlam\Documents\Khadamati\src\backend
dotnet run --project Khadamati.API
```

Leave this terminal open. Swagger: http://localhost:5000/swagger

**SQL connection (SQL Server 2014 on Windows):**

If `appsettings.Development.json` has **no connection string**, copy the example file:

```powershell
cd C:\Users\Ahlam\Documents\Khadamati\src\backend\Khadamati.API
copy appsettings.Development.example.json appsettings.Development.json
```

Or paste this **entire file** into `appsettings.Development.json`:

`C:\Users\Ahlam\Documents\Khadamati\src\backend\Khadamati.API\appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=KhadamatiDb;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true;Connect Timeout=20"
  },
  "Database": {
    "MigrateOnStartup": true,
    "SeedDemoData": true,
    "ContinueOnFailure": true
  },
  "App": {
    "WebBaseUrl": "http://localhost:5173",
    "ExposeAuthLinks": true
  },
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": false
  }
}
```

If login fails, edit only the `DefaultConnection` line in that file:

**Named instance (e.g. SQLEXPRESS):**
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=KhadamatiDb;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true;Connect Timeout=20"
```

**SQL login (sa):**
```json
"DefaultConnection": "Server=localhost;Database=KhadamatiDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true;Connect Timeout=20"
```

`Encrypt=False` is required for many SQL Server 2014 setups.

Make sure **SQL Server service is running** (Services → SQL Server (MSSQLSERVER) or SQL Server (SQLEXPRESS)).

### 2. Web

Open a **second** terminal:

```powershell
cd C:\Users\Ahlam\Documents\Khadamati\src\web
npm install
npm run dev
```

Open the URL Vite prints (usually http://localhost:5173).

### 3. Fix `.env` if the site cannot connect

If you have `src\web\.env`, make sure it uses the **direct API URL**:

```env
VITE_API_URL=http://localhost:5000/api/v1
```

**Do not use** `VITE_API_URL=/api/v1` unless you know you want the Vite proxy.

Or delete `src\web\.env` entirely — the app defaults to `http://localhost:5000/api/v1`.

After changing `.env`, restart the web dev server (`Ctrl+C`, then `npm run dev` again).

### 4. Clear stale login (if needed)

In the browser: DevTools → Application → Local Storage → clear for your site, then log in again.

## Quick health check

1. API running: open http://localhost:5000/swagger
2. Web running: open http://localhost:5173
3. In browser DevTools → Network, login should call `http://localhost:5000/api/v1/auth/login` (status 200 or 401, not “failed” / blocked)

## Pull latest fixes

```powershell
cd C:\Users\Ahlam\Documents\Khadamati
```

**If git says** `Please commit your changes or stash them before you merge` **for `appsettings.json`:**

Do **not** keep local edits in `appsettings.json`. Stash or discard them, then pull:

```powershell
git checkout -- src/backend/Khadamati.API/appsettings.json
git pull origin cursor/auth-region-fixes-7b80
```

Put **your SQL password / server name** in `appsettings.Local.json` instead (never committed):

```powershell
cd src\backend\Khadamati.API
copy appsettings.Local.json.example appsettings.Local.json
notepad appsettings.Local.json
```

Edit `DefaultConnection` in that file only, save, restart the API.

Or use `appsettings.Development.json` (see SQL section above).

```powershell
git pull origin cursor/auth-region-fixes-7b80
```

**Register and change password** include a **Confirm Password** field. After pulling, restart the web dev server (`Ctrl+C`, then `npm run dev`) and hard-refresh the browser (`Ctrl+Shift+R`).

Then restart **both** API and web terminals.

## Free stuck web ports (5173, 5174, …)

You have **many old `npm run dev` windows** still running. Close them, or run:

```powershell
taskkill /F /IM node.exe
```

Then start web again:

```powershell
cd C:\Users\Ahlam\Documents\Khadamati\src\web
npm run dev
```

It should say `http://localhost:5173` (not 5178).

Or use the helper script (after git pull):

```powershell
cd C:\Users\Ahlam\Documents\Khadamati
powershell -ExecutionPolicy Bypass -File scripts\stop-web-windows.ps1
powershell -ExecutionPolicy Bypass -File scripts\start-web-windows.ps1
```

**Note:** `taskkill /F /IM node.exe` stops **all** Node apps on your PC. Close other Node projects first if needed.

## Troubleshooting

| Problem | Fix |
|---------|-----|
| `Please commit your changes or stash them before you merge` on `appsettings.json` | Run `git checkout -- src/backend/Khadamati.API/appsettings.json` then pull; put SQL settings in `appsettings.Local.json` or `appsettings.Development.json` |
| `no such file or directory` for `src\backend` | You are not in the project folder — `cd C:\Users\Ahlam\Documents\Khadamati` first |
| Swagger error / “Failed to load API definition” | API did not start — check the API terminal for red SQL errors; fix connection string above; restart API |
| API hangs on startup | Wrong SQL connection — set `Encrypt=False` and correct `Server=` value; ensure SQL Server service is running |
| “Cannot reach the API” (web) | Start the API terminal; open http://localhost:5000/swagger |
| CORS / network errors | Use `VITE_API_URL=http://localhost:5000/api/v1`; restart web dev server |
| Port 5173 in use / Vite tries 5174, 5175… | Old dev servers still running — see **Free stuck web ports** below |
| Empty region/city dropdowns | Admin → Regions / Cities — add active regions and cities |
| Site loads but login/register fails | Database not connected — open http://localhost:5000/api/v1/health/ready (should be 200) |
