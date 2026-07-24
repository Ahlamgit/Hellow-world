# 38. Environment Configuration

**Document ID:** KHAD-V1-ENV  
**Status:** Draft for Approval  

---

## 38.1 Environments

| Name | Purpose |
|------|---------|
| `local` | Developer machines / compose |
| `dev` | Optional shared development |
| `staging` | Pre-prod integration & QA |
| `production` | Live |

## 38.2 Configuration Categories

| Category | Examples |
|----------|----------|
| Runtime | Server port, context path |
| Datasource | JDBC URL, user, password, pool |
| Security | JWT secret/JWKS, token TTLs, CORS origins |
| Payment | IXOPAY API URL, user, password, public integration key, callback base URL |
| Notifications | Email/SMS provider keys, from-addresses |
| IDV | OCR/face endpoints & keys, GPS thresholds (or DB settings) |
| Storage | S3 endpoint, bucket, credentials |
| Observability | Log level, OTLP endpoint |
| Feature flags | bootstrap defaults |

## 38.3 Delivery Mechanism

- 12-factor env vars / secret manager  
- Spring profiles: `local|staging|prod`  
- `.env.example` committed; real `.env` not committed  
- Mobile: flavor-specific config; no privileged secrets in apps (only public payment key + API URL)

## 38.4 Sample Variable Names (Illustrative)

```text
KHADAMATI_ENV=staging
DATABASE_URL=jdbc:postgresql://...
DATABASE_USER=...
DATABASE_PASSWORD=...
JWT_ACCESS_SECRET=...
IXOPAY_API_URL=...
IXOPAY_USERNAME=...
IXOPAY_PASSWORD=...
IXOPAY_PUBLIC_INTEGRATION_KEY=...
IXOPAY_CALLBACK_BASE_URL=https://api.staging...
CORS_ALLOWED_ORIGINS=https://admin.staging...
```

## 38.5 Per-Environment Isolation Rules

- Separate DB  
- Separate IXOPAY sandbox vs live credentials  
- Separate object storage buckets  
- Separate push notification apps/projects  

## 38.6 Questions

Final cloud secret manager choice follows Q-DEP-001.
