# 5. Use Cases

**Document ID:** KHAD-V1-UC  
**Status:** Draft for Approval  

---

## 5.1 Use Case Format

Each use case includes: ID, actor(s), goal, preconditions, main flow, alternate flows, and open questions when policy is ambiguous.

---

## 5.2 Identity & Access

### UC-AUTH-01 — Register Customer
- **Actor:** Customer  
- **Goal:** Create a customer account  
- **Preconditions:** Valid registration payload; identifier not already registered  
- **Main flow:** Submit registration → validate → create user+customer profile → issue tokens (or require verification first — Q-AUTH-003) → return session  
- **Alternate:** Duplicate identifier → reject; weak password → reject  

### UC-AUTH-02 — Register Craftsman
- **Actor:** Craftsman  
- **Goal:** Create craftsman account and enter onboarding  
- **Main flow:** Register → create craftsman profile in `PENDING_ONBOARDING` → guide to document upload  

### UC-AUTH-03 — Login
- **Actors:** Any authenticated human actor  
- **Main flow:** Submit credentials → validate → check lockout/status → issue access+refresh tokens  
- **Alternate:** Invalid credentials; locked; suspended  

### UC-AUTH-04 — Refresh Token
- **Main flow:** Present refresh token → rotate → return new pair  
- **Alternate:** Reused/revoked refresh → revoke family (theft detection)

### UC-AUTH-05 — Logout / Revoke
- **Main flow:** Revoke refresh token(s) for device/session  

---

## 5.3 Customer Marketplace

### UC-CUS-01 — Browse Categories
### UC-CUS-02 — Search Services
### UC-CUS-03 — Browse Stores
### UC-CUS-04 — View Service/Store Detail
### UC-CUS-05 — Create Booking
- **Preconditions:** Authenticated customer; service bookable; slot/rules satisfied (slot model Q-BOOK-007)  
- **Main flow:** Select service → provide schedule/location/notes → create booking in `DRAFT/PENDING_PAYMENT` or `REQUESTED` (Q-BOOK-008) → proceed to payment if required  

### UC-CUS-06 — Pay for Booking
- **Main flow:** See Payment UC-PAY-01  

### UC-CUS-07 — Track Booking Status
### UC-CUS-08 — Cancel Booking
- **Policy-dependent:** refund eligibility Q-BOOK-001..003  

### UC-CUS-09 — Submit Rating/Review
- **Preconditions:** Booking in completed state; not already reviewed (or update allowed? Q-RATE-001)  

### UC-CUS-10 — View Notifications  

---

## 5.4 Craftsman Operations

### UC-CRF-01 — Guided Onboarding
### UC-CRF-02 — Upload Documents
### UC-CRF-03 — Submit for Approval
### UC-CRF-04 — View Approval Decision
### UC-CRF-05 — Manage Service Catalog
### UC-CRF-06 — Subscribe to Plan
### UC-CRF-07 — Accept/Acknowledge Job (if assignment model requires — Q-BOOK-009)
### UC-CRF-08 — GPS Proximity Verification
### UC-CRF-09 — Selfie Verification
### UC-CRF-10 — QR/OTP Job Verification
### UC-CRF-11 — Update Job Progress
### UC-CRF-12 — View Earnings & KPIs
### UC-CRF-13 — Request Withdrawal
### UC-CRF-14 — View Notifications  

---

## 5.5 Store Operations

### UC-STR-01 — Manage Store Profile
### UC-STR-02 — Manage Products
### UC-STR-03 — Manage Services
### UC-STR-04 — Manage Orders
### UC-STR-05 — Manage Store Bookings
### UC-STR-06 — Request/Manage Advertisements
### UC-STR-07 — View Store Customers
### UC-STR-08 — View Store Reports  

---

## 5.6 Administration

### UC-ADM-01 — Manage Users & Roles
### UC-ADM-02 — Approve/Reject Craftsman Onboarding
### UC-ADM-03 — Suspend/Reinstate Actors
### UC-ADM-04 — Configure Commission Rules
### UC-ADM-05 — Manage Subscription Plans
### UC-ADM-06 — Manage Notification Templates
### UC-ADM-07 — Moderate Ratings/Reviews
### UC-ADM-08 — Manage Global Settings
### UC-ADM-09 — View Settlement Overview
### UC-ADM-10 — Review Withdrawal Requests
### UC-ADM-11 — Manage Advertisements (platform-level)
### UC-ADM-12 — View Audit Logs
### UC-ADM-13 — View Analytics/Reports
### UC-ADM-14 — Configure Quality Restriction Rules  

---

## 5.7 Payments

### UC-PAY-01 — Tokenize Card via Payment.js and Debit
- **Actors:** Customer (or Craftsman for subscription), API, Areeba IXOPAY  
- **Main flow:** Client initializes Payment.js → user enters card in hosted fields → tokenize → client sends token to API → API creates payment intent → API calls IXOPAY debit with `transactionToken` → persist result → handle 3DS redirect if required → webhook/callback confirms final state  

### UC-PAY-02 — Handle Payment Callback/Webhook
### UC-PAY-03 — Refund Payment (policy-gated)
### UC-PAY-04 — Reconcile Payment Status  

---

## 5.8 Quality & Follow-up

### UC-QUA-01 — Send 24-hour Booking Reminder
### UC-QUA-02 — Trigger Arrival Verification Window
### UC-QUA-03 — Monitor Job Progress SLAs
### UC-QUA-04 — Send Satisfaction Survey
### UC-QUA-05 — Compute Quality Score
### UC-QUA-06 — Apply Restriction Rules  

---

## 5.9 Detailed Example — UC-ADM-02 Craftsman Approval

| Field | Content |
|-------|---------|
| **Goal** | Decide whether a craftsman may operate |
| **Preconditions** | Application submitted; required documents present (checklist TBD Q-ONB-001) |
| **Main success** | Admin reviews dossier → approve → craftsman status `APPROVED` → entitlements evaluated → notify craftsman |
| **Alternate A** | Reject with reason → status `REJECTED` → notify → allow resubmit? (Q-ONB-002) |
| **Alternate B** | Request more info → status `NEEDS_INFO` → notify |
| **Postconditions** | Audit event recorded; notification dispatched |

---

## 5.10 Questions Requiring Business Decision

Referenced: `Q-AUTH-003`, `Q-BOOK-001..009`, `Q-RATE-001`, `Q-ONB-001/002`
