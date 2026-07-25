# ADR-026: Lebanon Finance Policy Configuration

**Status:** Accepted — 2026-07-24  
**Related:** ADR-013  

---

## Decision

Financial **values and commercial terms** for Lebanon (and future markets) are **Admin Portal configuration**.

### Must not be hardcoded

- Commission rates  
- Withdrawal methods  
- Settlement timing  
- Holds  
- Reserves  
- Refund percentages  
- Cancellation penalties / windows (policy content)  

Admin Portal (Finance / Super Admin, MFA, audit) controls these policies via ADR-013 modules.

---

## Lebanon Launch Process

1. Architecture ships policy engines + empty/active schema  
2. Finance Admin configures Lebanon starter policies in Staging → Production  
3. Feature flag `payments.enabled` / money features remain off until policies active  

Seed data may exist for **non-production** demos only; production source of truth is Admin config.

---

## Consequences

Confirms BR-018 / ADR-013. Coding may implement engines; coding must not embed Lebanon commercial constants as permanent domain truth.
