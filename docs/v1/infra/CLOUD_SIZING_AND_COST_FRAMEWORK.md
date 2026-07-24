# KHADAMATI V1 — Cloud Sizing and Cost Framework

**Document ID:** KHAD-V1-CLOUD-SIZING-COST  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Cloud Architect & FinOps Analyst  
**BLOCKER-004 status:** **IN PREPARATION**  

**Sources:**  
[`CLOUD_INFRASTRUCTURE_DECISION.md`](./CLOUD_INFRASTRUCTURE_DECISION.md) · Master Prompt v1.0 · Scope Baseline · ADR-024  

```text
DO NOT deploy infrastructure.
DO NOT create cloud resources.
DO NOT select a cloud provider.
DO NOT write application code.
DO NOT publish vendor-specific prices.

This framework supports evaluation only.
Numeric commercial/load targets remain Pending Infrastructure Approval
unless already fixed elsewhere as architecture defaults.
```

---

## Purpose

Define the **framework** used to:

1. Estimate infrastructure requirements from workload  
2. Compare cloud providers fairly (vendor-neutral)  
3. Plan scaling from Lebanon launch → regional → international  

| Item | State |
|------|-------|
| Cloud architecture | **IN PREPARATION** ([`CLOUD_INFRASTRUCTURE_DECISION.md`](./CLOUD_INFRASTRUCTURE_DECISION.md)) |
| Provider selection | **PENDING** — not selected |
| Vendor | **NOT SELECTED** |
| BLOCKER-004 COMPLETED | ☐ No |

---

# 1. Workload Model

Estimation inputs — fill with Product/Ops forecasts before provider RFPs.  
Until filled: **Pending Infrastructure Approval**.

## 1.1 Users

| Segment | Estimation input | Initial / launch value | Notes |
|---------|------------------|------------------------|-------|
| Customers | MAU / DAU / registered | **Pending Infrastructure Approval** | Peak concurrent for API sizing |
| Providers (Craftsman) | Active verified providers | **Pending Infrastructure Approval** | |
| Stores | Active store operators | **Pending Infrastructure Approval** | |
| Admin users | Concurrent Admin Portal users | **Pending Infrastructure Approval** | Low relative volume; MFA web-only |

Capture: average, peak, and growth % month-over-month for Year 1.

## 1.2 Transactions

| Transaction type | Unit | Launch estimate | Peak factor | Status |
|------------------|------|-----------------|-------------|--------|
| Bookings per month | count | | | **Pending Infrastructure Approval** |
| Payments per month | count | | | **Pending Infrastructure Approval** |
| Notifications per month | count | | | **Pending Infrastructure Approval** |
| Chat messages | count / month | | | **Pending Infrastructure Approval** |
| Reviews | count / month | | | **Pending Infrastructure Approval** |
| Webhooks (payment) | count / month | ≈ payments × retries | | Derive after payment estimate |
| Background jobs | executions / day | | | Derive from bookings + notifications + settlement |

## 1.3 Data Growth

| Data class | Initial volume | Monthly growth | Retention impact | Status |
|------------|----------------|----------------|------------------|--------|
| Provider documents (KYC) | | | Per Compliance BLOCKER-006 | **Pending Infrastructure Approval** |
| Images (services / catalog promo) | | | | **Pending Infrastructure Approval** |
| Attachments (chat / support) | | | | **Pending Infrastructure Approval** |
| Audit logs | | | Protected / long retention | **Pending Infrastructure Approval** |
| Financial records (ledger, payments) | | | Protected; no casual purge | **Pending Infrastructure Approval** |
| Database OLTP working set | | | | **Pending Infrastructure Approval** |
| Backups (DB + object) | ≈ primary × retention copies | | | Derive |

---

# 2. Infrastructure Mapping

Map workload → portable components (ADR-024). No cloud SKUs locked.

## 2.1 Backend (API)

| Requirement | Framework guidance | Decision |
|-------------|-------------------|----------|
| API instances | Stateless; size from peak RPS + latency SLO | **Pending Infrastructure Approval** |
| Scaling approach | Horizontal autoscaling; min replicas for HA | Horizontal (architecture) |
| Availability | Multi-AZ when cloud allows; LB health checks | **Pending Infrastructure Approval** |
| Web portals | Static/CDN hosting separate from API | Portable |

**Sizing inputs:** peak concurrent users, p95 latency target (§5), booking+payment request mix.

## 2.2 Database (PostgreSQL)

| Requirement | Framework guidance | Decision |
|-------------|-------------------|----------|
| PostgreSQL sizing | vCPU/RAM from connections + OLTP TPS; headroom for peaks | **Pending Infrastructure Approval** |
| Storage growth | Working set + indexes + WAL; project 12–24 months | **Pending Infrastructure Approval** |
| Backup size | Full snapshots × retention + PITR window storage | **Pending Infrastructure Approval** |
| Read scale | Replica when reporting/read load justifies | Phase 2+ typically |

## 2.3 Redis

| Requirement | Framework guidance | Decision |
|-------------|-------------------|----------|
| Cache size | Hot keys, sessions/rate-limits; not system of record | **Pending Infrastructure Approval** |
| Queue workload | Depth = job arrival − consume rate; size for payment/notify spikes | **Pending Infrastructure Approval** |
| HA | Managed HA if offered; treat data as rebuildable from DB | **Pending Infrastructure Approval** |

## 2.4 Workers

| Requirement | Framework guidance | Decision |
|-------------|-------------------|----------|
| Background jobs | Notifications, payment reconcile, webhook retries, settlement, schedules | Scale independent of API |
| Retry processing | Capacity for burst retries without starving new work | **Pending Infrastructure Approval** |
| Scheduled tasks | Settlement batches, retention jobs, reports | Off-peak windows |

**Sizing inputs:** jobs/minute at peak; max acceptable lag (outbox/queue).

## 2.5 Object Storage

| Requirement | Framework guidance | Decision |
|-------------|-------------------|----------|
| Initial storage | Sum of KYC + images + attachments at launch | **Pending Infrastructure Approval** |
| Monthly growth | From §1.3 | **Pending Infrastructure Approval** |
| Retention impact | Compliance retention increases long-term GB | Align BLOCKER-006 |
| Access pattern | Frequent CDN for public images; rare for KYC | Class/tier later |

---

# 3. Cost Evaluation Criteria

Use these **criteria** to compare providers.  
**Do not** attach vendor list prices in this document.

| Cost category | What to include in comparison | Normalization |
|---------------|-------------------------------|---------------|
| Compute cost | API + worker + (portal hosting if billed) | Per month at Phase 1 load; note autoscaling band |
| Database cost | Instance + storage + IOPS/IO + PITR/backup add-ons | Same vCPU/RAM/storage class assumptions |
| Storage cost | Object storage GB + requests + egress assumptions | Same GB + request profile |
| Backup cost | DB backup retention + object versioning | Same RPO window |
| Monitoring cost | Metrics, logs ingest/retention, alerts | Same cardinality / log volume |
| Network cost | LB, egress, CDN, cross-AZ if applicable | Same traffic model |
| Support cost | Support plan tier required for prod | Same severity/response expectation |

### Comparison rules (FinOps)

1. Same workload model (§1) for every vendor quote  
2. Same HA posture (e.g. multi-AZ API + managed DB)  
3. Separate **launch** vs **12-month** TCO  
4. Flag egress and log-ingest as common overrun risks  
5. Prefer portable IaC; reject quotes that force irreversible app coupling  
6. Record assumptions beside every estimate  

Vendor price sheets: attach externally when obtained — **not invented here**.

---

# 4. Scaling Phases

## Phase 1 — Lebanon launch

| Focus | Infrastructure evolution |
|-------|--------------------------|
| Goal | Single primary region; production isolation; payment + booking SLOs |
| Compute | Small HA API fleet (min 2); modest worker pool |
| Data | Single primary PostgreSQL; Redis; object storage; backups + restore drill |
| Observability | Core metrics/logs/alerts (§6 of Cloud Decision) |
| Cost posture | Optimize for predictable launch burn; avoid over-provision |

## Phase 2 — Regional expansion

| Focus | Infrastructure evolution |
|-------|--------------------------|
| Goal | More Markets / nearby region readiness; higher volume |
| Compute | Autoscaling bands raised; workers scaled with notify/pay load |
| Data | Consider read replica; storage tiering; larger PITR window if RPO tightens |
| Multi-market | Market-scoped config; no single-tenant rewrite |
| Cost posture | Rightsizing reviews; reserved/commit options if stable (vendor-neutral) |

## Phase 3 — International scaling

| Focus | Infrastructure evolution |
|-------|--------------------------|
| Goal | Multi-region or region-per-cluster as Business decides |
| Compute | Regional API/worker deployments as needed |
| Data | Cross-region DR options; data residency constraints |
| Cost posture | Per-region FinOps; egress controls; capacity forecasting |

Phase triggers (users/TPS/storage thresholds): **Pending Infrastructure Approval** (Product + DevOps).

---

# 5. Production Readiness Metrics

| Metric | Target | Status |
|--------|--------|--------|
| Availability target | | **Pending Infrastructure Approval** |
| Response time target (e.g. API p95) | | **Pending Infrastructure Approval** |
| Backup frequency | Align Cloud Decision (daily + PITR if available) | Confirm on approval |
| Recovery objectives — RPO | | **Pending Infrastructure Approval** (also Cloud Decision) |
| Recovery objectives — RTO | | **Pending Infrastructure Approval** |
| Queue lag SLO (outbox / workers) | | **Pending Infrastructure Approval** |
| Payment webhook processing lag | | **Pending Infrastructure Approval** |

Architecture supports measuring and alerting on these; numeric SLOs are a Business/DevOps decision.

---

# 6. Approval Checklist

| Decision | Status |
|----------|--------|
| Expected workload | Pending |
| Cloud provider | Pending |
| Infrastructure budget | Pending |
| RPO | Pending |
| RTO | Pending |
| DevOps approval | Pending |

Additional recommended: Security acknowledge · Product workload sign-off · FinOps budget owner

---

# 7. How this feeds BLOCKER-004

| Artifact | Role |
|----------|------|
| [`CLOUD_INFRASTRUCTURE_DECISION.md`](./CLOUD_INFRASTRUCTURE_DECISION.md) | What to host (components, DR, neutrality) |
| **This document** | How to size and compare cost without locking a vendor |
| Vendor quotes (external) | Filled later using §1–§3 |

BLOCKER-004 remains **IN PREPARATION** until provider/hosting/budget/RPO/RTO are approved.  
This framework alone does **not** complete BLOCKER-004 and does **not** select a vendor.

---

# 8. Explicit Non-Goals

- No deployment or cloud resource creation  
- No vendor selection or preferential ranking by brand  
- No application architecture changes  
- No invented user/transaction volumes or USD prices  

---

**End of Cloud Sizing and Cost Framework v1.0**
