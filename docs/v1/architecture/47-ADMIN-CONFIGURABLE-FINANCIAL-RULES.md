# Admin Configurable Financial Business Rules — Architecture

**Document ID:** KHAD-V1-FIN-POLICY  
**Status:** Approved direction (ADR-013)  
**Audience:** Architecture / Admin Portal / Booking / Ledger  

---

## 1. Principle

| Must be configurable in Admin Portal | Must not be hardcoded |
|--------------------------------------|------------------------|
| Commission rules | Fixed % / fixed amount as sole logic |
| Cancellation policies | Fixed who/when/penalty |
| Refund rules | Fixed full/partial/none matrix |
| Withdrawal configuration | Single permanent payout method |
| Settlement rules | Fixed cadence/holds in code |

Engines evaluate **active, effective-dated** policies at runtime.

---

## 2. Commission Configuration Module

### Admin can configure
- Default commission percentage  
- Fixed commission amount (optional / combined)  
- Service-category based commission  
- Provider-type based commission (craftsman vs store)  
- Future market-specific rules (`market_id`)  
- Subscription-level dimension (future-ready)  
- Booking value thresholds (future-ready)  
- Effective dates (`effective_from`, `effective_to`)  
- Activation / deactivation  
- Priority / specificity for matching  
- Commission history + audit trail  

### Matching context (example)
`market + category + provider_type + subscription_level + booking_value` → selected rule version.

### Runtime
On completion (or configured trigger): resolve rule → write `commission_lines` with `rule_id` + `rule_version` → post ledger.

---

## 3. Cancellation Policy Module

### Admin can define
- **Who can cancel:** Customer, Craftsman, Store, Admin  
- **Allowed booking statuses**  
- **Time-based conditions** (e.g., hours before scheduled start)  
- **Penalties**  
- **Refund behavior** (link to refund rule)  
- **Approval requirements** (auto vs admin approval)  

Booking engine evaluates active policies dynamically; returns allow/deny + outcomes.

---

## 4. Refund Rules Module

### Outcomes
- Full refund  
- Partial refund (configurable basis %)  
- No refund  
- Manual approval refund  

### May depend on
Booking status · cancellation reason · time before service · responsible party · payment status  

### Mandatory side effects
- Financial records (`refunds`)  
- Ledger entries (reversing/escrow release as applicable)  
- Audit history  

---

## 5. Withdrawal Configuration Module

### Admin can configure
- Available withdrawal methods (enabled/disabled)  
- Minimum withdrawal amount (per method/currency)  
- Approval workflow  
- Processing statuses (driven by workflow, not hardwired rails)  
- Country/market-specific methods (future)  

### Lebanon examples (config rows, not code constants)
- Manual payout  
- Bank transfer  
- Local payment providers (adapter key only)

Domain calls `WithdrawalMethodPort` by configured `adapter_key`.

---

## 6. Settlement Configuration

Admin configures cadence, reserves/holds, eligibility, batching. Settlement jobs read config.

---

## 7. Ledger (Confirmed)

Every financial event creates **immutable** ledger records including:

Transaction type · Booking reference · User/provider reference · Debit account · Credit account · Amount · Currency · Status · Created date · Audit information  

Balances are derived; never the only source of truth.

---

## 8. End-to-End Money Path

```text
Customer Payment
↓ Payment Gateway (Payment.js / IXOPAY)
↓ Escrow / Holding State
↓ Ledger Entries
↓ Completion Approval
↓ Commission Calculation (admin rules)
↓ Provider Earnings (ledger)
↓ Withdrawal Request (admin withdrawal config)
↓ Admin Approval
↓ Settlement (admin settlement rules)
```

---

## 9. Security

- Web-only Admin Portal  
- MFA required  
- RBAC: Finance Admin vs Super Admin  
- Audit logs + policy change history on every mutation  

---

## 10. Coding Gate Checklist

Before money features merge:

1. Policies loaded from DB/config module  
2. No magic commission/cancellation/refund constants in domain services  
3. Withdrawal methods from config table  
4. Admin screens + permissions exist for all five rule domains  
5. History + audit covered by tests  
