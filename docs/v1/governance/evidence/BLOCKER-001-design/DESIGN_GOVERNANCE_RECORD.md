# BLOCKER-001 — Design Governance Record

| Field | Value |
|-------|-------|
| **Document ID** | EVD-001-GOV-001 |
| **Version** | 1.1 |
| **Date** | 2026-07-25 |
| **Status** | **BUSINESS APPROVED** — evidence closure pending |
| **ADR** | ADR-023 (UI blocked until BLOCKER-001 Closed) |

---

## 1. Visual style (approved)

- Modern premium marketplace
- Clean minimal interface
- Trust-focused
- Professional service marketplace feeling
- **Orange** primary brand identity

---

## 2. Customer mobile app (approved features)

- Service categories
- Search
- Provider profiles
- Booking request
- Booking tracking
- Chat (booking-scoped per ADR-020)
- Reviews
- Payment initiation

---

## 3. Provider application (approved features)

- Provider profile
- Service management
- Availability management
- Booking management
- Customer communication
- Earnings visibility

**Excluded:** Wallet withdrawal implementation (V1 scope).

---

## 4. Administrator web portal (approved features)

### Platform governance

- User approval
- Provider approval
- Verification workflow
- Provider status management

### Service management

- Service categories
- Subcategories
- Service activation / deactivation

### Subscription management

- Provider subscription plans
- Store / service advertiser plans
- Pricing configuration
- Activation rules

### Advertisement management

- Advertisement packages
- Featured listings
- Promotional campaigns
- Approval workflow

### Moderation

- Reports
- Suspensions
- Content review

### Configuration

- Platform settings
- Business rules configuration
- Regional configuration

---

## 5. Localization (mandatory)

| Language | Direction |
|----------|-----------|
| Arabic | **RTL** |
| English | **LTR** |

**Architecture:** Multi-country ready.

### Lebanon defaults (configurable — not hardcoded)

| Setting | Default |
|---------|---------|
| Country | Lebanon |
| Phone prefix | +961 |
| Currency | USD |

Defaults only — administrator / configuration layer; **no hardcoding** in application logic (ADR-013).

---

## 6. Related evidence

| Document | Purpose |
|----------|---------|
| `BUSINESS_APPROVAL_RECORD.md` | PO/BO business approval |
| `DESIGN_ASSET_REFERENCE.md` | `theme(1).mp4` · `ic-khadamati(1).jpg` |
| `LOGO_REFINEMENT_SPECIFICATION.md` | Logo deliverables |
| `LOGO_USAGE_GUIDELINES.md` | Logo usage rules |
| `DESIGN_FINAL_SPECIFICATION.md` | Surface and journey specification |
| `DESIGN_SYSTEM_BASELINE.md` | Typography, spacing, components |
| `DESIGN_APPROVAL_SIGNOFF.md` | Design Lead sign-off (pending) |
| `APPROVAL_RECORD.md` | Full blocker approval (pending) |
| `EVIDENCE_CHECKLIST.md` | Closure checklist |

**BLOCKER-001 NOT CLOSED** until full evidence per GOV-BEMF-001.
