# BLOCKER-005 — Administrator Finance Control Model

| Field | Value |
|-------|-------|
| **Document ID** | EVD-005-ADMIN-FIN-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-005 — Finance |
| **Business approval** | **COMPLETE** — 2026-07-25 |
| **Technical validation** | **PENDING** — Technical Architect |
| **Blocker closure** | **NOT CLOSED** |

```text
Financial business values are NOT hardcoded.
Administrator controls operational configuration after implementation.
```

---

## 1. Governance principle

| Rule | Detail |
|------|--------|
| No hardcoded values | Commission percentages, subscription prices, and ad package prices MUST NOT be fixed in application code |
| Administrator authority | Platform Administrator configures operational financial parameters |
| Immutable ledger | Financial events recorded per ADR-004 — configuration changes do not rewrite history |
| Separation | Payment capture (BLOCKER-007) separate from settlement configuration |

---

## 2. Administrator-controlled domains

| Domain | Administrator manages | Hardcode allowed? |
|--------|----------------------|-------------------|
| **Commission percentage** | Global and per-category rates; effective dates | **No** |
| **Provider subscription plans** | Basic / Pro / Premium pricing and features | **No** |
| **Store/service advertising packages** | Package pricing, duration, placement rules | **No** |
| **Featured placement pricing** | Promotional placement fees | **No** |
| **Operational pricing configuration** | Activation rules, billing cycles, grace periods | **No** |

**Business strategy and revenue model creation** remain with **Project Owner / Business Owner** — not Administrator.

---

## 3. Administrator portal surfaces (post–Gate A)

| Surface | Function |
|---------|----------|
| Commission configuration | Set and update commission rules |
| Subscription management | Provider plan pricing and entitlements |
| Advertisement packages | Ad product pricing and availability |
| Platform finance settings | Operational parameters within approved model |

---

## 4. Technical architecture requirements (TA validation)

| # | Requirement | ADR | TA confirmed |
|---|-------------|-----|--------------|
| 1 | All financial values stored in configuration store | ADR-013 | ☐ |
| 2 | No launch percentages/prices in source code | ADR-013 | ☐ |
| 3 | Configuration changes audited | Governance | ☐ |
| 4 | Ledger entries immutable | ADR-004 | ☐ |
| 5 | Settlement uses configured rules at transaction time | ADR-030 | ☐ |
| 6 | Administrator cannot bypass payment security | ADR-030 | ☐ |

---

## 5. Excluded from V1 (unchanged)

| Feature | Status |
|---------|--------|
| Customer wallet | Excluded |
| Provider wallet UI | Excluded |
| Instant withdrawal | Excluded |
| Manual financial adjustment screens | Excluded |

---

## 6. Related documents

| Document | Purpose |
|----------|---------|
| `FINANCE_RULE_MATRIX.md` | Who controls what — governance framework |
| `ADMIN_CONFIG_MODEL_VALIDATION.md` | TA validation checklist |
| `COMMISSION_MODEL_APPROVAL.md` | Business approval — commission |
| `SUBSCRIPTION_PRICING_APPROVAL.md` | Business approval — subscriptions |

---

## 7. Sign-off (pending)

| Approver | Role | Attestation | Date | Status |
|----------|------|-------------|------|--------|
| | Technical Architect | "No financial business values are hardcoded in architecture" | | **Pending** |

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Administrator finance control model — Gate B evidence finalization |
