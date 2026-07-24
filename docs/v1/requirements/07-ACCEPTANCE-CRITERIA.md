# 7. Acceptance Criteria

**Document ID:** KHAD-V1-AC  
**Status:** Draft for Approval  

Acceptance criteria use Given/When/Then. Criteria marked **POLICY-GATED** require a resolved business question before QA can finalize expected results.

---

## 7.1 Authentication

### AC-AUTH-01 — Customer Registration
- Given valid registration data and a unique identifier  
- When the customer submits registration  
- Then a customer account is created and the API returns authenticated session artifacts **or** a verification-required state (per Q-AUTH-003)  
- And no password is stored in plaintext  

### AC-AUTH-02 — Login Failure Lockout
- Given repeated invalid login attempts exceeding configured threshold  
- When another attempt is made  
- Then the account is temporarily locked and an audit/security event is recorded  

### AC-AUTH-03 — Refresh Rotation
- Given a valid refresh token  
- When refresh is requested  
- Then a new access and refresh token are issued and the old refresh token is invalidated  

---

## 7.2 Booking

### AC-BOOK-01 — Create Booking
- Given an authenticated customer and a bookable service  
- When a booking is created with required fields  
- Then a booking aggregate is persisted with an initial status from the approved state machine  
- And an audit/domain event is emitted  

### AC-BOOK-02 — Invalid Transition Rejected
- Given a booking in status `COMPLETED`  
- When a client attempts to move it to `REQUESTED`  
- Then the API rejects the transition with a domain error code  

### AC-BOOK-03 — 24-hour Reminder
- Given a confirmed booking starting in approximately 24 hours  
- When the reminder job runs  
- Then the customer (and optionally craftsman — Q-NTF-003) receives a notification via configured channels  

### AC-BOOK-04 — Cancellation (**POLICY-GATED**)
- Given Q-BOOK-001..003 are decided  
- When a customer cancels within/outside policy windows  
- Then booking status, payment refund/no-refund, and notifications match the decided policy  

---

## 7.3 Payments (Areeba IXOPAY Payment.js)

### AC-PAY-01 — Tokenized Debit
- Given Payment.js successfully returns a single-use `transactionToken`  
- When the backend debit is requested for a payable booking/subscription  
- Then the payment record is created with gateway references and moves to a non-terminal pending/success/failed state consistent with gateway response  
- And PAN/CVV are never received by KHADAMATI APIs  

### AC-PAY-02 — Webhook Finalization
- Given a payment awaiting callback  
- When a valid signed/authenticated IXOPAY callback arrives  
- Then payment status is updated idempotently and dependent booking/subscription side-effects run once  

### AC-PAY-03 — Idempotency
- Given the same idempotency key for a debit request  
- When retried  
- Then no duplicate capture is created  

### AC-PAY-04 — Adapter Isolation
- Given business payment use-cases  
- When gateway implementation is referenced  
- Then domain services depend only on a payment port/interface, not IXOPAY DTOs  

---

## 7.4 Craftsman Approval & Identity

### AC-ONB-01 — Submit Application
- Given required documents uploaded per checklist (Q-ONB-001)  
- When craftsman submits application  
- Then status becomes `PENDING_APPROVAL` and admins can review  

### AC-ONB-02 — Approve
- Given an application in `PENDING_APPROVAL`  
- When an authorized admin approves  
- Then craftsman becomes eligible for entitlement checks and is notified  
- And an audit log entry exists  

### AC-IDV-01 — GPS Proximity (**POLICY-GATED**)
- Given Q-IDV-001 threshold  
- When craftsman submits GPS verification for a job  
- Then pass/fail is computed and recorded with coordinates metadata (retention per policy)  

### AC-IDV-02 — Selfie Verification (**POLICY-GATED**)
- Given Q-IDV-002/003 provider and match threshold  
- When selfie is submitted  
- Then verification case is scored and job/onboarding gates update accordingly  

### AC-IDV-03 — QR/OTP Verification
- Given an active job verification challenge  
- When correct QR/OTP is presented  
- Then verification succeeds and job progress can advance  

---

## 7.5 Store Dashboard

### AC-STR-01 — Product Create
- Given an authenticated store operator  
- When a valid product is created  
- Then it appears in store catalog APIs and is scoped to that store only  

### AC-STR-02 — Cross-tenant Isolation
- Given Store A operator token  
- When accessing Store B resources  
- Then API returns 403/404 per security standard (choose one consistently — recommend 404 for existence hiding on customer data, 403 for explicit admin denials)  

---

## 7.6 Commissions & Subscriptions

### AC-COM-01 — Commission Line Created (**POLICY-GATED**)
- Given decided commission rules Q-COM-*  
- When a monetizable event completes (e.g., paid booking)  
- Then a commission line is persisted with basis amount, rate/rule id, and currency  

### AC-SUB-01 — Entitlement Gate
- Given a craftsman without required active subscription (per Q-SUB-002)  
- When attempting a gated action  
- Then API rejects with entitlement error and clients can deep-link to plans  

---

## 7.7 Notifications

### AC-NTF-01 — Template Render
- Given a template with locale keys and placeholders  
- When a notification is triggered  
- Then the rendered message uses the recipient locale fallback chain  

### AC-NTF-02 — Channel Failure Isolation
- Given SMS provider failure  
- When multi-channel send is attempted  
- Then email/in-app may still succeed and failure is recorded per channel  

---

## 7.8 Admin Governance

### AC-ADM-01 — Audit Log
- Given an admin changes commission configuration  
- When the change is saved  
- Then audit log stores actor, timestamp, action, entity ids, and before/after snapshots  

### AC-ADM-02 — RBAC Enforcement
- Given a support admin without finance permission  
- When accessing withdrawal approval APIs  
- Then access is denied  

---

## 7.9 Quality Follow-up

### AC-QUA-01 — Survey Trigger
- Given a job marked completed  
- When follow-up schedule elapses  
- Then a satisfaction survey notification is sent once  

### AC-QUA-02 — Restriction Applied (**POLICY-GATED**)
- Given quality score breaches restriction rule  
- When scoring job runs  
- Then craftsman/store receives restriction state and gated actions fail  

---

## 7.10 Cross-cutting Acceptance (Definition of Done Excerpt)

For any P0 story to be “Done”:

1. API OpenAPI updated  
2. Flyway migration included if schema changed  
3. Unit + API tests for happy path and authz negative path  
4. No secrets committed  
5. Observability: structured log fields + error codes  
6. Security review checklist checked for money/PII flows  

## 7.11 Questions Requiring Business Decision

All **POLICY-GATED** criteria depend on the master open-questions register.
