# BLOCKER-004 — Cloud Readiness Decision Record

| Field | Value |
|-------|-------|
| **Document ID** | EVD-004-DECISION-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-004 — Cloud |
| **Business status** | **Cloud direction APPROVED** |
| **Technical validation** | **PENDING** — TA + DevOps signatures required |
| **Blocker closure** | **NOT CLOSED** |
| **Deployment** | **PROHIBITED** during Gate B |

```text
This record documents cloud readiness REQUIREMENTS and decision framework.
It does NOT authorize infrastructure deployment.
```

---

## 1. Business decision (complete)

| Decision | Status | Approver |
|----------|--------|----------|
| Cloud-hosted V1 platform direction | **APPROVED** | Project Owner / Business Owner |
| No deployment during evidence phase | **CONFIRMED** | GOV-BEMF-001 |

---

## 2. Hosting requirements (pending provider selection)

| Requirement | Detail | Selected | TA | DevOps |
|-------------|--------|----------|-----|--------|
| Cloud provider | Managed cloud hosting — **provider TBD** | ☐ | ☐ | ☐ |
| Region / residency | Lebanon launch baseline; multi-region ready | ☐ | ☐ | ☐ |
| Compute model | Containerized services (per architecture) | ☐ | ☐ | ☐ |
| Database hosting | Managed relational DB — provider TBD | ☐ | ☐ | ☐ |
| Object storage | Aligned with approved storage vendor (BLOCKER-003) | ☐ | ☐ | ☐ |
| CDN / edge | Static assets and media delivery | ☐ | ☐ | ☐ |
| TLS termination | HTTPS everywhere | ☐ | ☐ | ☐ |
| Environment isolation | Dev / staging / production separation | ☐ | ☐ | ☐ |

**Rule:** Do not invent provider names or costs — selection requires TA + DevOps validation.

---

## 3. Environment strategy

| Environment | Purpose | Requirements |
|-------------|---------|--------------|
| **Development** | Engineering integration | Isolated credentials; no production data |
| **Staging** | Pre-production validation; payment sandbox | Production-like config; test data only |
| **Production** | Live platform | Hardened access; audit logging; backup enabled |

| Control | Requirement |
|---------|-------------|
| Configuration management | Environment-specific secrets; no secrets in repository |
| Data separation | No production PII in non-production without governance exception |
| Promotion process | Staging validation before production release |

---

## 4. Backup strategy (requirements)

| Item | Requirement | Approved |
|------|-------------|----------|
| Database backups | Automated; point-in-time recovery capability | ☐ |
| Object storage durability | Provider-native redundancy | ☐ |
| Backup frequency | **Pending technical decision** — not invented | ☐ |
| Backup retention | **Pending technical decision** — not invented | ☐ |
| Recovery testing | Scheduled restore drills | ☐ |
| Off-site / cross-region | Per DR requirements below | ☐ |

---

## 5. Disaster recovery requirements

| Item | Requirement | Value | Approved |
|------|-------------|-------|----------|
| DR approach | Documented failover / restore runbook | Required | ☐ |
| RPO (Recovery Point Objective) | **Pending technical decision** | — | ☐ |
| RTO (Recovery Time Objective) | **Pending technical decision** | — | ☐ |
| Incident communication | Escalation path defined | Required | ☐ |
| Business continuity | Critical path: auth, booking, payment handoff | Documented | ☐ |

**Rule:** RPO/RTO values require TA + DevOps approval — do not invent.

---

## 6. Security requirements

| Control | Requirement | Validated |
|---------|-------------|-----------|
| Encryption in transit | TLS 1.2+ minimum | ☐ |
| Encryption at rest | Database and object storage | ☐ |
| Secrets management | Vault / managed secrets — no plaintext in config | ☐ |
| Network segmentation | Private subnets for data tier | ☐ |
| RBAC | Infrastructure access least-privilege | ☐ |
| Audit logging | Admin and infrastructure actions logged | ☐ |
| Vulnerability management | Patching cadence defined | ☐ |
| DDoS / WAF | Edge protection for public endpoints | ☐ |
| Monitoring and alerting | See §7 | ☐ |

---

## 7. Monitoring approach (requirements)

| Capability | Requirement | Approved |
|------------|-------------|----------|
| Application health | Uptime and error-rate monitoring | ☐ |
| Infrastructure metrics | CPU, memory, disk, connection pools | ☐ |
| Log aggregation | Centralized searchable logs | ☐ |
| Alerting | On-call escalation for critical failures | ☐ |
| Payment / webhook observability | Correlation IDs for payment events | ☐ |

---

## 8. Sign-off (pending)

| Approver | Role | Date | Status |
|----------|------|------|--------|
| | Technical Architect | | **Pending** |
| | DevOps Lead | | **Pending** |

**Closure artifact:** This document becomes `CLOUD_READINESS_DECISION_RECORD_v1.0` upon dual signature.

**Related:** `CLOUD_READINESS_RECORD.md` · `CLOUD_CLOSURE_VALIDATION_REPORT.md`

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Cloud readiness decision record — Gate B evidence finalization |
