# 6. User Stories

**Document ID:** KHAD-V1-US  
**Status:** Draft for Approval  
**Format:** As a … I want … so that …  

Stories are grouped by epic. Story points intentionally omitted until team velocity baseline exists.

---

## 6.1 Epic: Authentication & Profiles

| ID | Story |
|----|-------|
| US-AUTH-01 | As a customer, I want to register so that I can book services. |
| US-AUTH-02 | As a craftsman, I want to register so that I can start onboarding. |
| US-AUTH-03 | As a user, I want to log in and stay signed in securely so that I can use the app without repeated friction. |
| US-AUTH-04 | As a user, I want to update my profile so that my contact/location data stays accurate. |
| US-AUTH-05 | As an admin, I want to disable a compromised account so that platform risk is contained. |

## 6.2 Epic: Customer Booking & Discovery

| ID | Story |
|----|-------|
| US-CUS-01 | As a customer, I want to browse categories so that I can find the right service type. |
| US-CUS-02 | As a customer, I want to search services so that I can quickly locate providers. |
| US-CUS-03 | As a customer, I want to browse stores so that I can buy from trusted merchants. |
| US-CUS-04 | As a customer, I want to create a booking so that a job is scheduled. |
| US-CUS-05 | As a customer, I want to pay online so that my booking is confirmed. |
| US-CUS-06 | As a customer, I want to track booking status so that I know what happens next. |
| US-CUS-07 | As a customer, I want booking history so that I can reorder or review past jobs. |
| US-CUS-08 | As a customer, I want to rate and review completed jobs so that I can share feedback. |
| US-CUS-09 | As a customer, I want notifications so that I don’t miss reminders or status changes. |

## 6.3 Epic: Craftsman Onboarding & Trust

| ID | Story |
|----|-------|
| US-CRF-01 | As a craftsman, I want guided onboarding so that I know which documents and steps are required. |
| US-CRF-02 | As a craftsman, I want to upload documents so that I can be verified. |
| US-CRF-03 | As a craftsman, I want to see my approval status so that I know if I can take jobs. |
| US-CRF-04 | As an admin, I want to approve/reject onboarding so that only qualified craftsmen operate. |
| US-CRF-05 | As a craftsman, I want GPS and selfie verification so that job attendance is trusted. |
| US-CRF-06 | As a craftsman, I want QR/OTP verification so that job start/completion is confirmed. |

## 6.4 Epic: Craftsman Work & Monetization

| ID | Story |
|----|-------|
| US-CRF-07 | As a craftsman, I want a service catalog so that customers can find my offerings. |
| US-CRF-08 | As a craftsman, I want a dashboard with KPIs so that I can track performance. |
| US-CRF-09 | As a craftsman, I want to see earnings so that I understand what I’ve made. |
| US-CRF-10 | As a craftsman, I want to subscribe to a plan so that I unlock platform access tiers. |
| US-CRF-11 | As a craftsman, I want to request withdrawals so that I can receive payouts. |

## 6.5 Epic: Store Operations

| ID | Story |
|----|-------|
| US-STR-01 | As a store operator, I want to manage my profile so that customers see accurate info. |
| US-STR-02 | As a store operator, I want to manage products/services so that my catalog stays current. |
| US-STR-03 | As a store operator, I want to manage orders so that fulfillment is controlled. |
| US-STR-04 | As a store operator, I want to manage bookings so that scheduled work is visible. |
| US-STR-05 | As a store operator, I want to run advertisements so that I can acquire demand. |
| US-STR-06 | As a store operator, I want reports so that I can understand performance. |

## 6.6 Epic: Payments, Commissions, Settlements

| ID | Story |
|----|-------|
| US-PAY-01 | As a customer, I want secure card payment without sharing card data with KHADAMATI servers directly. |
| US-PAY-02 | As finance admin, I want payment statuses reconciled so that books match the gateway. |
| US-PAY-03 | As platform admin, I want commissions calculated automatically so that revenue share is consistent. |
| US-PAY-04 | As finance admin, I want a settlement overview so that I can monitor liabilities. |

## 6.7 Epic: Notifications & Quality

| ID | Story |
|----|-------|
| US-NTF-01 | As an admin, I want editable templates so that communications stay on-brand and multi-language ready. |
| US-NTF-02 | As a customer, I want a 24-hour reminder so that I don’t miss a booking. |
| US-NTF-03 | As operations, I want satisfaction surveys so that quality issues are detected early. |
| US-NTF-04 | As admin, I want restriction rules so that poor performers are limited automatically. |

## 6.8 Epic: Platform Governance

| ID | Story |
|----|-------|
| US-ADM-01 | As an admin, I want user management so that access is controlled. |
| US-ADM-02 | As an admin, I want global settings so that behavior can change without redeploying code. |
| US-ADM-03 | As an admin, I want audit logs so that sensitive actions are attributable. |
| US-ADM-04 | As an admin, I want analytics dashboards so that I can steer the marketplace. |
| US-ADM-05 | As a moderator, I want to manage ratings that violate policy. |

## 6.9 Story Splitting Rules

- Split by vertical slice (API + one client) when possible  
- Separate “policy configuration” stories from “runtime enforcement” stories  
- Do not implement UI chrome before design assets are approved  

## 6.10 Questions Requiring Business Decision

Stories that are blocked on policy (cancellation, commission math, payout rails, entitlement gating) must remain “Ready” only after corresponding `Q-*` decisions.
