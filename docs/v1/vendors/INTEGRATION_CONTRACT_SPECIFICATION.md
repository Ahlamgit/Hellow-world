# KHADAMATI V1 — Integration Contract Specification

**Document ID:** KHAD-V1-INTEGRATION-CONTRACTS  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Integration Architect  
**BLOCKER-003 status:** **IN PREPARATION**  

**Sources:**  
[`VENDOR_INTEGRATION_READINESS_MATRIX.md`](./VENDOR_INTEGRATION_READINESS_MATRIX.md) · [`VENDOR_EVALUATION_MATRIX.md`](./VENDOR_EVALUATION_MATRIX.md) · [`VENDOR_RISK_AND_SLA_ASSESSMENT.md`](./VENDOR_RISK_AND_SLA_ASSESSMENT.md) · ADR-025 · Master Prompt v1.0 · [`../workflows/23-PAYMENT-FLOW.md`](../workflows/23-PAYMENT-FLOW.md)

```text
DO NOT write production code.
DO NOT create API clients.
DO NOT integrate vendors.
DO NOT select vendors.

This document defines KHADAMATI-internal ports/contracts only.
No vendor API endpoints. No SDK bindings. No adapter implementation.
```

---

## Status Snapshot

| Item | State |
|------|-------|
| Architecture | **APPROVED** |
| Integration pattern | Ports and Adapters (ADR-025) |
| Vendor evaluation | **IN PREPARATION** |
| Vendor selection | **PENDING** |
| BLOCKER-003 | **IN PREPARATION** |
| Contract readiness (this doc) | Framework published; checklist rows **Pending** |

---

## Purpose

Define **KHADAMATI internal integration contracts** required before any vendor adapter implementation.

Goals:

| Goal | Result |
|------|--------|
| Vendor replacement capability | Swap adapter without rewriting domain |
| Stable domain architecture | Booking, ledger, commission stay vendor-free |
| Clear responsibilities | Port vs adapter vs domain ownership |
| Reduced vendor coupling | No vendor DTOs/SDKs in business services |

Companion docs:

- Evaluation framework: [`VENDOR_INTEGRATION_READINESS_MATRIX.md`](./VENDOR_INTEGRATION_READINESS_MATRIX.md)  
- Scoring / selection sheet: [`VENDOR_EVALUATION_MATRIX.md`](./VENDOR_EVALUATION_MATRIX.md)  

---

# 1. Integration Contract Principles

## 1.1 Core rule

```text
External vendors are adapters.

Application Domain
  → Integration Port (contract)
  → Adapter Layer (vendor-specific)
  → External Vendor
```

Core business logic **must not** depend on vendor APIs, SDKs, or vendor DTOs.

## 1.2 What every contract must define

| Dimension | Required content |
|-----------|------------------|
| **Input** | Domain-facing command/query fields (IDs, amounts, locales, correlation ids) |
| **Output** | Normalized result model (status, ids, safe metadata) |
| **Errors** | Typed failure categories (not vendor error codes in domain) |
| **Security requirements** | Auth, encryption, secret handling, webhook verification where applicable |
| **Retry behaviour** | Idempotent vs non-idempotent; who retries; backoff ownership |
| **Audit requirements** | What must be logged; PII/PAN exclusions; correlation |

## 1.3 Responsibility split

| Layer | Owns |
|-------|------|
| Domain / application services | Business rules, ledger, commission, booking state |
| Port (contract) | Stable interface + normalized models |
| Adapter | Vendor mapping, HTTP/SDK calls, signature verify, vendor retries |
| Infrastructure / ops | Secrets, network, observability sinks |

## 1.4 Naming note

Port names below are **contract names**. Existing ADR / workflow aliases (`PaymentGatewayPort`, `SmsSenderPort`, `ObjectStoragePort`, etc.) map to the same contracts. Implementation may use one canonical Java interface name per concern; domain must not import vendor packages.

---

# 2. Payment Integration Contract

**Port:** `PaymentPort` (alias: `PaymentGatewayPort`)  
**Candidate vendor (architecture only):** Areeba IXOPAY Payment.js — **not selected for implementation authorization** until validation + approvals  
**Do not** define Areeba/IXOPAY API endpoints in this contract.

## 2.1 Responsibilities

| Capability | Port responsibility |
|------------|---------------------|
| Create payment | Accept domain payment intent (payable id, amount, currency, customer ref, callback context); return KHADAMATI payment id + client-safe init data |
| Check payment status | Return normalized status from KHADAMATI state and/or adapter status query |
| Receive payment confirmation | Accept verified confirmation event; map to domain outcome |
| Handle webhook events | Adapter verifies authenticity; port delivers normalized `PaymentEvent` |
| Handle failures | Map declines/timeouts/errors to typed failures; keep payment reconciliable |
| Refund support | Accept refund command (full/partial per Admin policy); return refund outcome |
| Transaction reconciliation | Support lookup by merchant/payment id and gateway reference for settle/reconcile jobs |

## 2.2 Input (conceptual)

| Field group | Examples (domain-level) |
|-------------|-------------------------|
| Identity | `paymentId`, `payableId`, `payableType`, `marketId` |
| Money | `amountMinor`, `currency` (server-authoritative) |
| Client token | Opaque token from hosted fields (never PAN/CVV) |
| URLs / context | Success/cancel/error return context; webhook correlation |
| Idempotency | Client/API idempotency key + merchant transaction id |

## 2.3 Output (normalized)

| Outcome | Meaning |
|---------|---------|
| `CREATED` / `PENDING` | Intent recorded; awaiting capture/action |
| `REQUIRES_ACTION` | 3DS/redirect or similar; client must continue |
| `CAPTURED` / `SUCCEEDED` | Funds confirmed for domain finalization |
| `FAILED` / `DECLINED` | Terminal failure for this attempt |
| `REFUNDED` / `PARTIALLY_REFUNDED` | Refund applied per policy |

Safe metadata only (e.g. last4/brand if provided by gateway). **No PAN/CVV** in KHADAMATI storage or logs.

## 2.4 Errors

| Category | Domain handling |
|----------|-----------------|
| `VALIDATION` | Reject before vendor call |
| `DECLINED` | User-visible decline; no ledger credit |
| `REQUIRES_ACTION` | Drive client continuation |
| `TIMEOUT_UNKNOWN` | Leave PENDING; reconcile + webhook |
| `VENDOR_UNAVAILABLE` | Fail soft; retry policy on safe operations |
| `AUTH_OR_SIGNATURE` | Alert Security; do not finalize |
| `IDEMPOTENT_REPLAY` | Return original outcome |

## 2.5 Security / retry / audit

| Area | Requirement |
|------|-------------|
| Security | Hosted fields / PCI minimization; server credentials in secrets manager; webhook signature verification in adapter |
| Retry | Debit: careful — prefer idempotency + reconcile over blind re-debit; status/refund: policy-bounded retries |
| Audit | Log payment id, payable, amount, currency, gateway ref, outcome; never log card data or raw tokens |

## 2.6 KHADAMATI money flow (domain owns post-confirmation)

```text
Customer Payment
  → Payment Provider (via PaymentPort / adapter)
  → Payment Confirmation (verified webhook / capture)
  → Ledger Entry
  → Commission Calculation (Admin policy)
  → Provider Balance
  → Settlement (Admin policy)
```

Adapter stops at payment I/O. Ledger, commission, and settlement are **not** vendor concerns.

---

# 3. Notification Integration Contract

**Port:** `NotificationPort`  
**Channels:** SMS · Email · Push (channel may be separate sub-ports: `SmsPort`, `EmailPort`, `PushPort` — same contract principles)

## 3.1 Supports

| Channel | Typical uses |
|---------|--------------|
| SMS | OTP, booking notifications, provider alerts, security notifications |
| Email | Account communication, notifications, admin alerts, reports |
| Push | Booking/status alerts (when product enables) |

## 3.2 Responsibilities

| Capability | Requirement |
|------------|-------------|
| Send | Accept channel, recipient ref, **message template reference**, locale, variables |
| Delivery status | Return/accept normalized status: `QUEUED` · `SENT` · `DELIVERED` · `FAILED` · `UNKNOWN` |
| Retry handling | Transient failures retried at adapter/worker boundary with idempotency key |
| Failure logging | Structured failure reason; no OTP secrets in clear logs |

## 3.3 Input / output / errors

| Dimension | Contract |
|-----------|----------|
| Input | `notificationId`, `channel`, `templateRef`, `locale`, `recipient`, `variables`, `correlationId`, `priority` |
| Output | `providerMessageId` (opaque), `status`, timestamps |
| Errors | `INVALID_RECIPIENT` · `TEMPLATE_MISSING` · `RATE_LIMITED` · `VENDOR_UNAVAILABLE` · `REJECTED` |

## 3.4 Security / retry / audit

| Area | Requirement |
|------|-------------|
| Security | Credentials in secrets manager; OTP content minimized in logs |
| Retry | Exponential backoff; dead-letter after max attempts; alert on security/OTP failure spikes |
| Audit | Who/what/when/template/channel/status; link to user/booking where applicable |

---

# 4. Maps Integration Contract

**Port:** `MapsPort` (aliases may include geocoding / distance helpers)

## 4.1 Location services

| Capability | Purpose |
|------------|---------|
| Geocoding | Address ↔ coordinates |
| Distance calculation | Availability search, ETA aids, fee inputs if policy uses distance |
| Provider location | Store/update provider position for discovery |
| Service area validation | Check point/area against provider service areas |

## 4.2 Input / output / errors

| Dimension | Contract |
|-----------|----------|
| Input | Coordinates or address text, market/locale, units, provider/service-area refs |
| Output | Normalized lat/lng, distance meters, boolean in-area, confidence/quality if available |
| Errors | `GEOCODE_FAILED` · `OUT_OF_COVERAGE` · `INVALID_COORDINATES` · `VENDOR_UNAVAILABLE` · `QUOTA_EXCEEDED` |

## 4.3 Security / retry / audit

| Area | Requirement |
|------|-------------|
| Security | API keys server-side where possible; rate limits; no unnecessary PII in map queries |
| Retry | Safe retries on geocode/distance; circuit break on quota |
| Audit | Request purpose (search/validate), market, outcome — avoid logging full raw addresses in high-volume paths if policy restricts |

---

# 5. Identity Verification Contract

**Port:** `IdentityVerificationPort`  
(Sub-capabilities may be `OcrPort` / `FaceVerificationPort` behind the same ownership model.)

## 5.1 Supports

| Capability | Purpose |
|------------|---------|
| Document verification | Provider identity documents during onboarding |
| Face verification | Liveness / face match where product requires |
| Verification status | Normalized: `PENDING` · `PASSED` · `FAILED` · `MANUAL_REVIEW` · `EXPIRED` |
| Failure reasons | Typed reasons for Admin/ops (not raw vendor dumps in UI) |

## 5.2 Input / output / errors

| Dimension | Contract |
|-----------|----------|
| Input | Provider id, document refs (via StoragePort), verification type, correlation id |
| Output | Status, reason codes, vendor session/ref (opaque), reviewed-at |
| Errors | `DOCUMENT_UNREADABLE` · `DOCUMENT_UNSUPPORTED` · `FACE_MISMATCH` · `VENDOR_UNAVAILABLE` · `POLICY_BLOCK` |

## 5.3 Security · retention · audit

| Area | Requirement |
|------|-------------|
| Security | Encrypted transport; least-privilege access; no identity images in application logs |
| Retention | Align with Compliance (BLOCKER-006) and Admin retention policy; vendor retention must not conflict |
| Audit | Who initiated, what type, outcome, Admin override/manual review actions |

Phased deferral of OCR/Face only with Product risk acknowledgement (see Evaluation Matrix).

---

# 6. Storage Integration Contract

**Port:** `StoragePort` (alias: `ObjectStoragePort`)

## 6.1 Supports

| Capability | Requirement |
|------------|-------------|
| Upload | Store object; return storage key / URI handle |
| Download | Authorized read (signed URL or mediated stream) |
| Access control | Private by default; role/purpose-scoped access |
| File metadata | Content type, size, checksum, owner, purpose, created-at |
| Deletion rules | Soft/hard delete per retention & Admin policy; legal hold awareness |

## 6.2 Input / output / errors

| Dimension | Contract |
|-----------|----------|
| Input | Stream/bytes ref, `purpose` (KYC, service image, attachment), owner ids, content type, size limit |
| Output | `objectKey`, metadata, optional time-limited access URL |
| Errors | `TOO_LARGE` · `TYPE_NOT_ALLOWED` · `NOT_FOUND` · `ACCESS_DENIED` · `VENDOR_UNAVAILABLE` |

## 6.3 Security / retry / audit

| Area | Requirement |
|------|-------------|
| Security | Encryption at rest/in transit; no public KYC buckets; signed URLs short-lived |
| Retry | Safe retries on upload/download; idempotent upload keys where supported |
| Audit | Upload/download/delete actor, purpose, object key, outcome |

Align vendor choice with BLOCKER-004 cloud storage decision where practical; still behind this port.

---

# 7. Vendor Failure Handling

Common behaviour for **all** adapters:

| Condition | Required behaviour |
|-----------|-------------------|
| Timeout | Do not invent success; mark unknown/pending where money or verification is involved; reconcile |
| Vendor unavailable | Typed `VENDOR_UNAVAILABLE`; circuit breaker at adapter boundary |
| Invalid response | Reject; log safe payload fingerprint; alert if systemic |
| Retry | Retry only safe/idempotent operations; bounded attempts + backoff |
| Fallback handling | Optional secondary adapter only if config + Product/Eng approved; never silent vendor switch for payments |
| Manual review | Identity verification, disputed payments, reconciliation mismatches → Admin/ops queues |

Domain must remain able to explain state to users/Admin without vendor-specific jargon.

---

# 8. Security Requirements

Every integration **must** support:

| Requirement | Expectation |
|-------------|-------------|
| Authentication | Vendor auth method documented; credentials never in git |
| Encryption | TLS in transit; encryption at rest for stored secrets/objects as applicable |
| Audit logging | Structured logs with correlation ids; sensitive field redaction |
| Access restriction | Least privilege for keys/roles; private networking where required |
| Secret management | Secrets manager; rotation process defined before go-live |

Webhook-bearing integrations (especially Payment) **must** verify authenticity before domain finalization.

---

# 9. Approval Checklist

| Integration | Contract Ready | Vendor Selected | Validation |
|-------------|----------------|-----------------|------------|
| Payment | **Pending** | **Pending** | **Pending** |
| SMS | **Pending** | **Pending** | **Pending** |
| Email | **Pending** | **Pending** | **Pending** |
| Maps | **Pending** | **Pending** | **Pending** |
| OCR/Face | **Pending** | **Pending** | **Pending** |
| Storage | **Pending** | **Pending** | **Pending** |

**Notes:**

- *Contract Ready* = this specification accepted by Architect + Eng Lead for that row (still Pending).  
- *Vendor Selected* = Business + Technical (+ Compliance where required) approval complete.  
- *Validation* = sandbox/technical validation complete (Payment also depends on BLOCKER-007).  

---

# 10. Relationship to BLOCKER-003 Closure

This document advances **contract preparation** only.

BLOCKER-003 remains **IN PREPARATION** until:

- [ ] Vendors selected (or deferred with Product risk ack)  
- [ ] Contracts approved (commercial + this technical contract acceptance)  
- [ ] Sandbox credentials available  
- [ ] Technical validation completed  
- [ ] Integration authorization granted (after Implementation Gate → A, or explicit phased waiver)

---

# 11. Explicit Non-Goals

- No production code  
- No API client generation  
- No vendor SDK integration  
- No vendor selection  
- No Areeba/IXOPAY (or other vendor) endpoint catalogs in this file  

---

**End of Integration Contract Specification v1.0**
