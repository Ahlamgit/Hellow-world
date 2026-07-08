# KHADAMATI Platform

Enterprise-grade maintenance and home services marketplace connecting **Customers**, **Craftsmen**, **Stores**, and **Administrators**.

## Architecture

```
┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│  Web (React)│  │   Android   │  │     iOS     │
└──────┬──────┘  └──────┬──────┘  └──────┬──────┘
       │                │                │
       └────────────────┼────────────────┘
                        │ REST API (JWT)
              ┌─────────▼─────────┐
              │  ASP.NET Core 9   │
              │   Clean Arch.     │
              └─────────┬─────────┘
                        │
              ┌─────────▼─────────┐
              │   SQL Server DB   │
              └───────────────────┘
```

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 9, EF Core, JWT, Serilog, FluentValidation, AutoMapper, MediatR |
| Database | Microsoft SQL Server (stored procedures, views, indexes, audit logs) |
| Web | React 19, TypeScript, Material UI 9, i18next (AR/EN), RTL |
| Android | Kotlin, Jetpack Compose, MVVM, Retrofit, Room |
| iOS | Swift, SwiftUI, MVVM, async/await, Keychain |

## Project Structure

```
/workspace
├── src/
│   ├── backend/          # ASP.NET Core 9 Web API (Clean Architecture)
│   ├── database/         # SQL Server scripts (views, SPs, indexes)
│   ├── web/              # React TypeScript web application
│   ├── android/          # Native Android application
│   └── ios/              # Native iOS application
├── docs/                 # Architecture documentation
└── docker-compose.yml    # Full stack deployment
```

## Quick Start

### Prerequisites

- .NET 9 SDK
- Node.js 22+
- Docker & Docker Compose (for SQL Server + full stack)
- Android Studio (for Android)
- Xcode 15+ (for iOS)

### Run with Docker

```bash
docker compose up -d
```

- API: http://localhost:5000/swagger
- Web: http://localhost:3000
- SQL Server: localhost:1433

### Run Backend Locally

```bash
cd src/backend
dotnet run --project Khadamati.API
```

### Run Web Locally

```bash
cd src/web
cp .env.example .env
npm install
npm run dev
```

### Default Admin Credentials

| Field | Value |
|-------|-------|
| Email | admin@khadamati.com |
| Password | Admin@123456 |

## Security Features

- JWT authentication with refresh tokens
- BCrypt password hashing (work factor 12)
- Role-based authorization (Customer, Craftsman, Store, Administrator)
- Rate limiting on auth endpoints
- Account lockout after failed login attempts
- Soft delete with audit trails
- Input validation (FluentValidation)
- HTTPS enforcement
- CSRF protection (antiforgery tokens)
- SQL injection protection (parameterized queries via EF Core)

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/auth/register` | Register new user |
| POST | `/api/v1/auth/login` | Login |
| POST | `/api/v1/auth/refresh` | Refresh JWT token |
| POST | `/api/v1/auth/revoke` | Revoke refresh token |
| GET | `/api/v1/users/me` | Get current user profile |
| PUT | `/api/v1/users/me` | Update profile |
| POST | `/api/v1/users/me/addresses` | Add address |
| GET | `/api/v1/services/categories` | List service categories |
| GET | `/api/v1/services` | List services |
| POST | `/api/v1/services/requests` | Create service request |
| GET | `/api/v1/health` | Health check |

## License

Proprietary - KHADAMATI Platform
