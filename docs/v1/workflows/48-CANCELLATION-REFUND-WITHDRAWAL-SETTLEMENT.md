# 48. Cancellation, Refund, Withdrawal & Settlement Policy Workflows

**Document ID:** KHAD-V1-POLICY-FLOWS  
**Status:** Aligned to ADR-013  

---

## 48.1 Cancellation

```text
Cancel request (Customer|Craftsman|Store|Admin)
  → Load active cancellation_policies for market + status + actor + time-before-service
  → Allow / Deny + penalty + refund_rule_ref + approval_required
  → If approval required → admin queue
  → Apply booking status transition + audit
  → Trigger refund evaluation if linked
```

No fixed “24h free cancel” (or similar) in code unless present as an **active configured policy**.

## 48.2 Refund

```text
Refund context (from cancellation or admin action)
  → Evaluate refund_rules (status, reason, time, party, payment status)
  → FULL | PARTIAL | NONE | MANUAL_APPROVAL
  → Create refund record
  → Post ledger entries (immutable)
  → Audit
```

## 48.3 Withdrawal

```text
Provider withdrawal request
  → Validate method enabled for market
  → Validate min amount / currency
  → Check available ledger balance
  → Enter approval workflow per withdrawal_configs
  → Admin approve/reject
  → Settlement / payout adapter by method.adapter_key
  → Ledger postings
```

Lebanon methods (examples as **config**, not code constants): manual payout, bank transfer, local providers.

## 48.4 Settlement

```text
Settlement job / admin run
  → Load settlement_rules for market
  → Select eligible ledger balances (after holds/reserves)
  → Create settlement batch
  → Mark lines settled
  → Audit
```

## 48.5 Canonical Money Path

```text
Customer Payment → Gateway → Escrow/Hold → Ledger
→ Completion Approval → Commission (rules) → Provider Earnings
→ Withdrawal Request → Admin Approval → Settlement (rules)
```
