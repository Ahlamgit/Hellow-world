# Business Workflow Validation (Final)

**Document ID:** KHAD-V1-WF-FINAL  

---

## 1. Customer Booking (Canonical)

```text
Customer request
    ↓
Provider confirmation (accept)
    ↓
Payment (Payment.js → token → backend → IXOPAY → webhook → ledger/escrow)
    ↓
Service execution (GPS → Selfie → QR/OTP → In progress)
    ↓
Completion confirmation (provider complete → customer confirm)
    ↓
Commission calculation (admin rules) + ledger
    ↓
Provider earnings available (per settlement/hold rules)
    ↓
Review / survey
```

**Validated against:** ADR-005, ADR-013, Payment.js architecture, FTM booking/pay rows.

---

## 2. Provider Verification

```text
Registration
    ↓
Documents upload
    ↓
Verification (OCR + face integration points)
    ↓
Admin approval
    ↓
Active listing (subject to subscription entitlements)
```

Applies to Craftsman and Store onboarding.

---

## 3. Payment Flow (Areeba IXOPAY Payment.js)

```text
Client → Payment.js hosted fields → tokenize
    ↓
Backend Payment Service (server amount authoritative)
    ↓
IXOPAY debit
    ↓
Webhook (signature verified, idempotent)
    ↓
Ledger update (+ escrow hold)
```

| Control | Status |
|---------|--------|
| No raw card storage | Confirmed |
| Abstraction port | Confirmed |
| Webhook idempotency | Confirmed |
| Reconciliation job | Required (worker) |
| Commissions outside gateway adapter | Confirmed |

---

## 4. Withdrawal & Settlement

```text
Withdrawal request → validate admin withdrawal config
    ↓
Admin approval (Finance)
    ↓
Settlement per settlement rules → payout adapter
    ↓
Ledger postings
```

---

## 5. Cancellation / Refund

```text
Cancel request → evaluate cancellation policy → optional approval
    ↓
Evaluate refund rule → refund record + ledger
```

Dynamic policies only (ADR-013).
