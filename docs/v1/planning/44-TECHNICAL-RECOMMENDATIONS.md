# 44. Technical Recommendations

**Document ID:** KHAD-V1-REC  
**Status:** Draft for Approval  

---

## 44.1 Strong Recommendations (Adopt for V1)

1. **Modular monolith** with ArchUnit boundaries — not microservices first  
2. **Transactional outbox** for notifications/commissions side effects  
3. **Payment port/adapter** with IXOPAY isolated  
4. **Idempotency keys** on booking/payment/withdrawal creates  
5. **Problem Details** error standard  
6. **UUID PKs + timestamptz** everywhere  
7. **Money as minor units + currency**  
8. **Presigned uploads** to object storage (no binary through API)  
9. **Testcontainers PostgreSQL** in CI  
10. **HttpOnly cookie refresh for web** if CSRF strategy is implemented correctly (or Bearer+memory with short access TTL)  
11. **Feature flags** for payments, auto-restrictions, ads serving  
12. **Expand/contract migrations** only in production path  

## 44.2 Recommended Spikes (Before Full Build of Area)

| Spike | Why |
|-------|-----|
| Payment.js in Flutter WebView + 3DS return | Highest integration risk |
| OCR+Face provider selection | Accuracy/cost/privacy |
| Admin permission matrix prototyping | Unblocks portal IA |
| GPS spoofing threat model | Trust & safety strength |

## 44.3 Tooling Recommendations

| Area | Recommendation |
|------|----------------|
| API docs | springdoc-openapi |
| Mapping | MapStruct optional |
| Web monorepo | pnpm + Vite |
| Server state (web) | TanStack Query |
| Flutter state | Riverpod or Bloc (pick one early) |
| CI | GitHub Actions |
| Observability | OpenTelemetry + structured logs |
| Cache | Redis only if needed for rate limit/flags (Q-DEP-003) |

## 44.4 Explicit Non-Recommendations for V1

- Shared DB tables without module ownership  
- Synchronously sending SMS inside booking API transaction  
- Storing Payment.js tokens long-term unnecessarily  
- Building generic purple/cream marketing UI before brand assets  
- Re-implementing legacy ASP.NET domain blindly  

## 44.5 Decision Records

Start `docs/v1/adr/` at implementation kickoff for:

- ADR-001 Modular monolith  
- ADR-002 Payment gateway abstraction  
- ADR-003 Auth token storage web  
- ADR-004 Flutter state management  
- ADR-005 UUID strategy  
