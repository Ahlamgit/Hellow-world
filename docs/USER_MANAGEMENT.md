# User Management Module

Administrator API for full user lifecycle management with permission-based authorization.

## Endpoints

Base: `/api/v1/admin/users`  
Auth: Bearer JWT + `[HasPermission]` per action

| Method | Route | Permission | Description |
|--------|-------|------------|-------------|
| GET | `/` | `Users.View` | Search users (search, status, role, date range, sort, pagination) |
| GET | `/{id}` | `Users.View` | User detail with roles and permissions |
| POST | `/` | `Users.Create` | Create user (any of 9 roles) |
| PUT | `/{id}` | `Users.Edit` | Update profile fields and status |
| DELETE | `/{id}` | `Users.Delete` | Soft-delete user |
| POST | `/{id}/suspend` | `Users.Suspend` | Suspend account |
| POST | `/{id}/activate` | `Users.Edit` | Activate account |
| PUT | `/{id}/roles` | `Users.Edit` | Assign roles (syncs legacy `User.Role` enum) |
| POST | `/{id}/verify-email` | `Users.VerifyEmail` | Manual email verification |

## Architecture

```
AdminUsersController → MediatR → IUserManagementService → IUserRepository + IIdentityRepository
```

## Role Assignment

Updates `UserRoles`, `PrimaryRoleId`, and legacy `User.Role` enum via `RoleNames.MapToLegacyEnum()`.

## Create User

- Validates password policy
- Creates profile and role-specific entities (CraftsmanProfile, StoreProfile)
- Optional verification email (`sendVerificationEmail` default `true`)
- Records password history and security audit log

## Tests

```bash
cd src/backend && dotnet test --filter UserManagement
```

6 new tests in `UserManagementServiceTests` and `AdminUserValidatorTests`.
