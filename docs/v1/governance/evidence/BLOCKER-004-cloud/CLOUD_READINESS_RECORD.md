# BLOCKER-004 — Cloud Readiness Record

| Field | Value |
|-------|-------|
| **Document ID** | EVD-004-READINESS-001 |
| **Version** | 1.1 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-004 — Cloud |
| **Business status** | **Approved direction** |
| **Evidence status** | **Closure preparation** — technical validation pending |
| **Blocker closure** | **NOT CLOSED** |

---

## 1. Business approval (complete)

| Item | Status | Approver |
|------|--------|----------|
| Cloud-hosted V1 platform direction | **Approved** | Project Owner / Business Owner |
| No deployment during evidence collection | **Confirmed** | GOV-BEMF-001 |

---

## 2. Hosting decision (technical — pending)

| Item | Preparation | TA approved | DevOps approved |
|------|-------------|-------------|-----------------|
| Cloud provider selection | Framework documented | ☐ | ☐ |
| Region / data residency alignment | Lebanon baseline · multi-region ready | ☐ | ☐ |
| Secure hosting environment | Requirement documented | ☐ | ☐ |

---

## 3. Environment strategy (pending)

| Environment | Purpose | Preparation | Validated |
|-------------|---------|-------------|-----------|
| Development | Engineering | Documented | ☐ |
| Staging | Pre-production validation | Documented | ☐ |
| Production | Live platform | Documented | ☐ |

---

## 4. Security baseline (pending)

| Control | Requirement | Validated |
|---------|-------------|-----------|
| Network segmentation | Required | ☐ |
| Encryption in transit (TLS) | Required | ☐ |
| Encryption at rest | Required | ☐ |
| Secrets management | Required | ☐ |
| Audit logging | Required | ☐ |
| RBAC for infrastructure access | Required | ☐ |

---

## 5. Backup strategy (pending)

| Item | Preparation | Approved |
|------|-------------|----------|
| Backup frequency approach | Documented | ☐ |
| Backup retention approach | Documented | ☐ |
| Recovery testing plan | Pending detail | ☐ |

---

## 6. Disaster recovery (pending)

| Item | Preparation | Approved |
|------|-------------|----------|
| DR approach documented | Direction only | ☐ |
| RPO target | **Pending technical decision** | ☐ |
| RTO target | **Pending technical decision** | ☐ |

---

## 7. Monitoring approach (pending)

| Item | Preparation | Approved |
|------|-------------|----------|
| Infrastructure monitoring | Documented | ☐ |
| Application health monitoring | Documented | ☐ |
| Alerting / escalation | Documented | ☐ |
| Cost monitoring | Documented | ☐ |

---

## 8. DevOps ownership

| Role | Responsibility | Status |
|------|----------------|--------|
| **DevOps Lead** | Operations readiness · backup/DR · monitoring | **Pending validation** |
| **Technical Architect** | Hosting architecture alignment | **Pending validation** |

**Closure artifact:** `CLOUD_READINESS_DECISION_RECORD_v1.0` — **not filed**.

**No infrastructure creation. No deployment.**
