# KHADAMATI — Windows local development (Ahlam)

## Your machine

| Item | Value |
|------|-------|
| **GitHub repo** | [Ahlamgit/khadamati](https://github.com/Ahlamgit/khadamati) |
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

**SQL connection:** edit `src\backend\Khadamati.API\appsettings.json` (or `appsettings.Development.json`) if your SQL instance is not on port `1433`. Example for default SQL Server instance:

```json
"DefaultConnection": "Server=localhost;Database=KhadamatiDb;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

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
git pull origin cursor/auth-region-fixes-7b80
```

Then restart **both** API and web terminals.

## Troubleshooting

| Problem | Fix |
|---------|-----|
| `no such file or directory` for `src\backend` | You are not in the project folder — `cd C:\Users\Ahlam\Documents\Khadamati` first |
| “Cannot reach the API” | Start the API terminal; confirm http://localhost:5000/swagger loads |
| CORS / network errors | Use `VITE_API_URL=http://localhost:5000/api/v1`; restart web dev server |
| Port 5173 in use | Vite will use 5174, 5175, etc. — any localhost port works |
| Empty region/city dropdowns | Admin → Regions / Cities — add active regions and cities |
