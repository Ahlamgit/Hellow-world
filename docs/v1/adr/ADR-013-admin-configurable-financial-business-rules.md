# ADR-013: Admin Configurable Financial Business Rules

**Document ID:** ADR-013  
**Status:** Accepted — 2026-07-24  
**Decision Owner:** Product / Finance / Solution Architecture  

---

## Decision

The following business rule domains **must not be hardcoded** in application code:

1. **Commission rules**  
2. **Cancellation rules**  
3. **Refund rules**  
4. **Withdrawal rules**  
5. **Settlement rules**  

They are **admin-configurable policies** managed exclusively through the KHADAMATI **Administration Web Portal** by authorized finance/super-admin roles, evaluated dynamically at runtime by domain engines (booking, payment, commission, ledger, withdrawal).

Code may implement a **generic policy evaluation engine** (match criteria → apply outcomes). Code must **not** embed fixed percentages, fixed cancellation windows, fixed refund percentages, or a single permanent withdrawal rail as business truth.

---

## Reason

- Lebanon-first launch will iterate commercial terms without redeploys  
- GCC/international expansion needs market-specific rules  
- Investor/ops require auditable, changeable financial policy  
- Hardcoded money rules create production change risk and silent policy drift  
- Aligns with Market readiness and immutable ledger architecture (ADR-001, ADR-004)

---

## Impact

### Product / Admin Portal
New (or expanded) Admin modules:

| Module | Purpose |
|--------|---------|
| Commission Configuration | Versioned commission rules with dimensions & effective dates |
| Cancellation Policy Management | Who/when/penalties/refund linkage |
| Refund Rules Management | Full/partial/none/manual-approval outcomes |
| Withdrawal Configuration | Methods, mins, approval workflow, enablement |
| Settlement Configuration | Cadence, holds/reserves, settlement batching rules |

All changes require **MFA-authenticated web admin**, **RBAC**, **audit log**, and **change history**.

### Runtime Engines
- Booking cancellation path loads **active** cancellation policy for market + status + actor + time  
- Refund service resolves refund rule → creates payment refund + **ledger** postings  
- Commission service resolves matching commission rule(s) at calculation time → ledger  
- Withdrawal service validates against configured methods/minimums/approval  
- Settlement jobs use settlement configuration, not hardcoded cron business meaning  

### Validation (pre-coding / code review gates)
- [ ] No hardcoded commission percentages/amounts as sole source of truth  
- [ ] No hardcoded cancellation windows/actors  
- [ ] No hardcoded refund percentages  
- [ ] No single permanently fixed withdrawal provider/method in domain logic  

---

## Database Impact

New / expanded configuration tables (logical):

| Table | Notes |
|-------|-------|
| `commission_rules` | Criteria JSON/cols + percent/fixed outcomes; market_id; effective_from/to; active; priority |
| `commission_rule_history` | Immutable snapshots on change |
| `cancellation_policies` | Actor set, allowed statuses, time conditions, penalty, refund_rule_ref, approval_required |
| `cancellation_policy_history` | Snapshots |
| `refund_rules` | Outcome type (FULL/PARTIAL/NONE/MANUAL), basis %, conditions |
| `refund_rule_history` | Snapshots |
| `withdrawal_methods` | code, market_id, enabled, min_amount_minor, currency, provider_adapter_key |
| `withdrawal_configs` | Approval workflow flags, limits |
| `withdrawal_config_history` | Snapshots |
| `settlement_rules` | Cadence, reserve %, hold days, market_id, active |
| `settlement_rule_history` | Snapshots |

Money movement tables remain immutable operational records:

- `ledger_accounts`, `ledger_entries`  
- `payments`, `refunds`  
- `commission_lines` (computed instances referencing `commission_rule_id` + rule version)  
- `withdrawal_requests`  
- `settlement_batches` / periods  

**Never** soft-delete ledger entries; policy history is append-only.

---

## API Impact

Admin (audience `admin-web` only), permission-gated:

```text
/api/v1/admin/commission-rules
/api/v1/admin/cancellation-policies
/api/v1/admin/refund-rules
/api/v1/admin/withdrawal-methods
/api/v1/admin/withdrawal-configs
/api/v1/admin/settlement-rules
```

Each supports CRUD (or create-new-version), activate/deactivate, history listing.

Runtime (internal / domain):

- `PolicyEvaluationPort.evaluateCommission(context)`  
- `PolicyEvaluationPort.evaluateCancellation(context)`  
- `PolicyEvaluationPort.evaluateRefund(context)`  
- `WithdrawalPolicyPort.validateRequest(context)`  
- `SettlementPolicyPort.resolveActiveRule(marketId)`  

Public mobile/store APIs **consume** evaluated outcomes (e.g., cancellation preview) but **cannot** mutate policies.

---

## Security Impact

| Control | Requirement |
|---------|-------------|
| Channel | Admin Web Portal only |
| MFA | Required for all admins; step-up recommended on policy save |
| RBAC | `Finance Admin`: commissions, refunds, withdrawals, settlements, cancellation policies; `Super Admin`: full |
| Audit | Every create/update/activate/deactivate logged with before/after |
| Change history | Dedicated history tables + audit_events |
| Least privilege | Non-finance admins cannot edit money policies |

Example permissions:

- `commission_rules:read|write`  
- `cancellation_policies:read|write`  
- `refund_rules:read|write`  
- `withdrawal_config:read|write`  
- `settlement_rules:read|write`  
- `withdrawals:approve`  

---

## Future Scalability Impact

- Market-specific rules via `market_id` without code forks  
- Category / provider-type / subscription-tier / booking-value dimensions extensible  
- Country-specific withdrawal methods as configuration rows + adapters  
- Rule versioning allows historical bookings to remain explainable (store `rule_id` + `rule_version` on computed lines)  

---

## Financial Flow (Confirmed)

```text
Customer Payment
  → Payment Gateway (IXOPAY Payment.js)
  → Escrow / Holding State
  → Ledger Entries
  → Completion Approval
  → Commission Calculation (from active commission rules)
  → Provider Earnings (ledger)
  → Withdrawal Request (validated by withdrawal config)
  → Admin Approval
  → Settlement (per settlement rules)
```

All steps produce immutable, auditable financial records.

---

## Consequences

- Q-COM-001 structure becomes **configuration schema**, not a single hardcoded rate (rates still entered by admin; no inventing production rates in code)  
- Q-BOOK-001..004 become **cancellation/refund policy configuration**, not open inventable constants in code  
- Q-SET-* become **settlement/withdrawal configuration**  
- Implementation must ship Admin UI for these modules before money go-live  
- Seed data may load **starter** Lebanon policies for staging only; production values set by Finance Admin  

## Related

- ADR-004 Financial Ledger  
- ADR-006 Admin Web-Only + MFA  
- Master Prompt § Admin Configurable Financial Rules  
- Feature Traceability: BR-ADM-FIN-*  
