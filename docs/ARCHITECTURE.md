# KHADAMATI Architecture

## Overview

KHADAMATI is a maintenance and home services marketplace built with Clean Architecture principles, designed for scalability, maintainability, and enterprise-grade security.

## Backend Clean Architecture

```
Khadamati.API          → Controllers, Middleware, DI Configuration
Khadamati.Application  → DTOs, Commands/Queries (CQRS), Validators, Mappings
Khadamati.Domain       → Entities, Enums, Repository Interfaces
Khadamati.Infrastructure → EF Core, Repositories, Auth Services, External Integrations
```

### Design Patterns

- **Repository Pattern** with generic `IRepository<T>` and `IUnitOfWork`
- **CQRS** via MediatR for command/query separation
- **Dependency Injection** throughout all layers
- **AutoMapper** for entity-to-DTO mapping
- **FluentValidation** for request validation
- **Soft Delete** with global query filters

## Database Design

### Core Entities

- `Users` - All user types with role, status, verification, subscription
- `UserProfiles` - Name, bio, profile picture, language preference
- `Addresses` - GPS-enabled addresses with default flag
- `RefreshTokens` - JWT refresh token management
- `CraftsmanProfiles` - Specialization, rating, availability
- `StoreProfiles` - Store details, hours, rating
- `ServiceCategories` - Hierarchical service categories (AR/EN)
- `Services` - Individual services with pricing
- `ServiceRequests` - Customer service bookings
- `StoreProducts` - Store inventory
- `AuditLogs` - Change tracking

### Database Objects

- **Views**: `vw_ActiveUsers`, `vw_ServiceRequestSummary`
- **Stored Procedures**: `sp_GetNearbyCraftsmen`, `sp_GetDashboardStats`, `sp_InsertAuditLog`
- **Indexes**: Composite indexes on status/role, request status, address coordinates

## Authentication Flow

1. User registers/logs in with email + password
2. Server validates credentials (BCrypt hash verification)
3. Server generates JWT access token (15 min) + refresh token (7 days)
4. Client stores tokens and attaches Bearer token to requests
5. On 401, client uses refresh token to obtain new access token
6. Refresh token rotation on each refresh (old token revoked)

## Mobile Architecture

### Android (MVVM + Compose)

```
UI Layer (Compose Screens) → ViewModels → Repositories → Remote (Retrofit) / Local (Room)
```

### iOS (MVVM + SwiftUI)

```
Views → ViewModels → Services (APIClient) → Keychain Storage
```

## Localization

All client applications support Arabic (RTL) and English (LTR):
- Web: i18next with JSON translation files
- Android: `values/strings.xml` + `values-ar/strings.xml`
- iOS: `Localizable.strings` (en/ar)

## Deployment

Production deployment uses Docker Compose with:
- SQL Server 2022 container
- ASP.NET Core API container
- Nginx-served React SPA container

For production, configure:
- Strong JWT secret (256-bit minimum)
- SQL Server with encrypted connections
- HTTPS termination at reverse proxy
- Environment-specific connection strings
