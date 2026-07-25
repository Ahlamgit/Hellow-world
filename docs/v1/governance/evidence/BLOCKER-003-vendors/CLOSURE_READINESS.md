# BLOCKER-003 — Vendor Closure Readiness

| Field | Value |
|-------|-------|
| **Blocker** | BLOCKER-003 — Vendors |
| **Date** | 2026-07-25 |
| **Gate** | Gate A TRANSITION IN PROGRESS |
| **Status** | **Open** — not Closed |
| **Architecture** | Ports/adapters (ADR-025) — unchanged |

## Vendor dependency matrix

| Integration | Primary candidate | Responsibility | Sandbox | Evidence folder |
|-------------|-------------------|----------------|---------|-----------------|
| **Payment** | Areeba IXOPAY Payment.js | Integration Lead + Finance | Pending | `payment/` |
| **SMS** | TBD (Q-NTF-001) | Integration Lead | Pending | `sms/` |
| **Email** | TBD (Q-NTF-001) | Integration Lead | Pending | `email/` |
| **Maps** | TBD | Integration Lead | Pending | `maps/` |
| **OCR / Face** | TBD (Q-OCR-001) | Integration Lead + Compliance | Pending | `ocr-face/` |
| **Storage** | TBD | DevOps Lead | Pending | `storage/` |

## Areeba IXOPAY responsibility matrix

| Area | Owner | Responsibility |
|------|-------|----------------|
| Gateway contract / commercial | Program Sponsor | Vendor selection approval |
| Payment.js integration design | Technical Architect | Port/adapter pattern |
| Sandbox access | Integration Lead | Credentials + test environment |
| Webhook endpoints | Technical Lead | Verification strategy (BLOCKER-007) |
| Finance reconciliation | Finance Ops | Acceptance criteria |
| PCI scope | Compliance | No card storage — Payment.js only |

## Gaps

| Gap | Owner | Action |
|-----|-------|--------|
| Vendor evaluation per integration | Integration Lead | Complete evaluation matrix |
| Selected vendors documented | Integration Lead | Document decisions |
| Sandbox readiness evidence | Integration Lead | File per subfolder |
| `VENDOR_READINESS_DOSSIER_v1.0` | Program Governance | File on closure |
| `APPROVAL_RECORD.md` signed | TA + Integration Lead | Pending |

**No integration code** during evidence collection.
