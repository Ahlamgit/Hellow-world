# BLOCKER-003 — Business Approval Record (Vendors)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-BUSINESS-001 |
| **Version** | 1.0 |
| **Blocker** | BLOCKER-003 — Vendors |
| **Decision** | **PARTIALLY APPROVED** (business vendor categories) |
| **Approver** | Project Owner / Business Owner |
| **Date** | 2026-07-25 |
| **Consolidation** | [BUSINESS_APPROVAL_CONSOLIDATION_RECORD.md](../../BUSINESS_APPROVAL_CONSOLIDATION_RECORD.md) |
| **Blocker closure** | **NOT CLOSED** — vendor dossier · TA validation · payment/OCR technical pending |

---

## SMS provider — APPROVED (purpose-limited)

| Allowed | Not approved |
|---------|--------------|
| User verification | Marketing messages |
| Provider verification | Promotional campaigns |
| Security verification | |

**Technical vendor selection:** Integration Lead + Technical Architect (evidence pending).

---

## Email provider — APPROVED (purpose-limited)

| Allowed | Not approved |
|---------|--------------|
| Account verification | Marketing automation (without future approval) |
| Security emails | |
| Transactional notifications | |

**Technical vendor selection:** Integration Lead + Technical Architect (evidence pending).

---

## Storage provider — APPROVED

| Purpose |
|---------|
| Images |
| Documents |
| Application files |
| Required media |

**Requirements:** Secure access · backup policy · permission control.

**Technical vendor selection:** Integration Lead + Technical Architect (evidence pending).

---

## Maps / location provider — NOT APPROVED FOR V1

**Decision:** Do **NOT** implement maps/location in V1.

| Forbidden in V1 |
|-----------------|
| Maps integration |
| GPS tracking |
| Location tracking |
| Location-based matching |

**Record:** `MAPS_V1_EXCLUSION_RECORD.md` — future enhancement only after separate approval.

---

## Payment · OCR / face

Not covered by this business approval record. Technical evaluation and sandbox evidence remain required per `EVIDENCE_CHECKLIST.md`.

---

## Closure note

PO/BO vendor **category** decisions recorded. **BLOCKER-003 remains open** until `VENDOR_READINESS_DOSSIER_v1.0`, per-integration evidence, and Technical Architect approval.
