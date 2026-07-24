# 4. Actors

**Document ID:** KHAD-V1-ACTORS  
**Status:** Draft for Approval  

---

## 4.1 Primary Human Actors

| Actor | Description | Primary Channel |
|-------|-------------|-----------------|
| **Customer** | End user booking services / purchasing store offerings | Customer Flutter app |
| **Craftsman** | Service provider subject to onboarding, verification, subscriptions | Craftsman Flutter app |
| **Store Operator** | Manages a store’s catalog, orders, bookings, ads | Store Dashboard |
| **Platform Administrator** | Full platform governance | Administration Portal (**web only**) |
| **Operations / Support Admin** | Day-to-day support within limited permissions | Administration Portal (**web only**) |
| **Finance Admin** | Settlements, commissions, withdrawal reviews | Administration Portal (**web only**) |
| **Content / Growth Admin** | Ads, templates, campaigns | Administration Portal (**web only**) |

> Exact admin permission matrix is undecided — see Q-RBAC-001.

## 4.2 Secondary Human Actors

| Actor | Description |
|-------|-------------|
| **Approver** | Admin role performing craftsman onboarding decisions |
| **Moderator** | Admin role moderating ratings/reviews/ads |
| **Store Staff** (optional) | Additional store users under one store — existence TBD Q-STR-001 |

## 4.3 System Actors

| Actor | Description |
|-------|-------------|
| **API Gateway / Load Balancer** | Terminates TLS, routes to API instances |
| **KHADAMATI API** | Core business system |
| **PostgreSQL** | System of record |
| **Areeba IXOPAY Gateway** | Tokenization (Payment.js) + transaction processing + callbacks |
| **Email Provider** | Delivers email notifications |
| **SMS Provider** | Delivers SMS notifications |
| **Push Provider (FCM/APNs)** | Delivers mobile push |
| **Object Storage** | Stores documents/media binaries |
| **OCR Service** | Extracts text/fields from documents |
| **Face Recognition Service** | Compares selfie to reference |
| **Scheduler / Job Runner** | Executes reminders, expirations, settlement jobs |
| **CI/CD System** | Builds, tests, deploys |
| **Observability Stack** | Logs, metrics, traces |

## 4.4 Actor Relationships (Context)

```text
Customer ----books----> Booking Engine <----fulfills---- Craftsman
   |                         |                              |
   | pays                    | emits events                 | verifies identity
   v                         v                              v
Areeba IXOPAY <----> Payments Domain                 Trust & Safety
                             |
                             +----> Commissions / Notifications / Quality

Store Operator ----manages----> Store Domain ----may link----> Booking/Orders
Platform Admin ----governs----> All domains via Admin Portal
```

## 4.5 Actor Authentication Requirements

| Actor | Auth Method (V1) |
|-------|------------------|
| Customer | Email/phone + password (exact identifier Q-AUTH-001); JWT; Customer Flutter app |
| Craftsman | Same pattern; JWT; elevated actions require verification state; Craftsman Flutter app |
| Store Operator | Email + password; JWT; store-scoped authorization; Store Dashboard (web) |
| Admins | Email + password; JWT; permission-scoped; MFA recommended (Q-AUTH-002); **Administration Portal (web) only** |
| System integrations | Signed webhooks / API keys / mTLS where applicable |

### Confirmed channel rule — Administrators

**Administrators must not log in from mobile apps.**  
Admin authentication and session use are allowed **only** on the Administration Portal (web). Customer and Craftsman Flutter apps must not offer admin login, and the API must reject admin-role authentication attempts from mobile client audiences.

## 4.6 Questions Requiring Business Decision

- Q-RBAC-001: Admin role taxonomy and permission matrix  
- Q-STR-001: Multi-user store accounts vs single operator  
- Q-AUTH-001: Login identifier (email, phone, both)  
- Q-AUTH-002: MFA mandatory for admins in V1?  
- ~~Q-AUTH-007~~ **Decided:** Admins web-only (see above) — not available on Flutter apps
