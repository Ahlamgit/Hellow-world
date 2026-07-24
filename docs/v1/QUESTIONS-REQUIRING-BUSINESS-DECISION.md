# Questions Requiring Business Decision

**Document ID:** KHAD-V1-QUESTIONS  
**Status:** Partially dispositioned by Master Prompt v1.0  
**How to use:** Status = `Answered` / `Deferred-to-V2` / `Assumed-with-owner-signoff` / `Open`

---

## A. Identity, Auth, RBAC

| ID | Question | Status |
|----|----------|--------|
| Q-AUTH-001 | Login identifier email, phone, or either? | **Answered (directional):** OTP authentication required; phone OTP primary for mobile MENA launch (+961). Email may coexist for web store/admin — confirm email-for-admin in IAM design. |
| Q-AUTH-002 | MFA mandatory for admins? | **Answered: Yes — required** (ADR-006) |
| Q-AUTH-003 | Verify before first session? | Open (OTP implies verification for phone flows) |
| Q-AUTH-004 | Password reset channel? | Open (admin/store); mobile OTP-centric |
| Q-AUTH-005 | Refresh TTL / multi-device? | Open |
| Q-AUTH-006 | Permissions in JWT vs lookup? | Open |
| Q-AUTH-007 | Admin login from mobile? | **Answered: No — web portal only; no mobile admin APIs** (ADR-006) |
| Q-RBAC-001 | Final admin permission matrix? | Open (roles exist; matrix detail TBD) |
| Q-STR-001 | Multi-user store accounts? | **Answered (directional):** Yes — provider/staff affiliation required |

## B. Localization, Currency, Region, Compliance

| ID | Question | Status |
|----|----------|--------|
| Q-LOC-001 | Launch country? | **Answered: Lebanon** (ADR-001) |
| Q-LOC-002 | Languages? | **Answered: Arabic RTL primary, English LTR secondary** |
| Q-LOC-003 | Currency? | **Answered: USD default** |
| Q-LOC-004 | Tax invoice requirements? | Open |
| Q-CUR-001 | FX in V1? | **Answered (directional):** Single active currency USD at launch; multi-currency **ready** via Market + money columns; no FX engine required V1 |
| Q-CMP-001 | Privacy regimes? | Open (Lebanon + future GCC) |
| Q-CMP-002 | Marketing opt-in defaults? | Open |
| Q-CMP-003 | Store raw webhook payloads? | Open |

## C. Booking & Relationships

| ID | Question | Status |
|----|----------|--------|
| Q-BOOK-001..006 | Cancel/refund/no-show/dispute policy details | **Partially answered:** Cancellation + refund rules are **admin-configurable** (ADR-013). Lebanon values set by Finance Admin. No-show/dispute detail may remain Open. |
| Q-BOOK-007 | Scheduling model | **Answered: Provider Availability Calendar (ADR-019)** |
| Q-BOOK-008 | Prepay before confirm? | **Answered: No — provider confirmation then payment** (ADR-005) |
| Q-BOOK-009 | Assignment model? | **Answered: Customer selects provider/listing** |
| Q-BOOK-010 | Bidding? | **Answered: No** (not in master prompt) |
| Q-REL-001 | Craftsman independent and/or store-affiliated? | **Answered: Unified Provider; affiliation supported** (ADR-003) |
| Q-REL-002 | Store services fulfilled via platform? | **Answered: Yes — stores are service providers** |
| Q-REL-003 | Product delivery? | **N/A — products out of scope** (ADR-002) |

## D. Payments

| ID | Question | Status |
|----|----------|--------|
| Q-PAY-001 | Store product orders online pay? | **N/A — no products** (ADR-002) |
| Q-PAY-002 | Saved cards / withRegister? | Open |
| Q-PAY-003 | Currencies on Areeba? | **Answered (directional):** USD for Lebanon launch; confirm merchant config with Areeba |

## E. Commissions & Settlements

| ID | Question | Status |
|----|----------|--------|
| Q-COM-001..005 | Commission structure details | **Answered (structural):** Admin-configurable multi-dimensional rules (ADR-013). Rates/content set by Finance Admin — not hardcoded. |
| Q-SET-001..004 | Settlement cadence / rails / reserves | **Answered (structural):** Admin-configurable settlement + withdrawal methods (ADR-013). Lebanon methods configured, not hardcoded. |

## F. Subscriptions

| ID | Question | Status |
|----|----------|--------|
| Q-SUB-001 | Store subscriptions in V1? | Open (craftsman subscriptions required; store TBD) |
| Q-SUB-002 | Entitlement gates? | Open (configurable required) |
| Q-SUB-003 | Auto-renew / grace? | Open |
| Q-SUB-004 | Free tier? | Open |

## G. Onboarding & IDV

| ID | Question | Status |
|----|----------|--------|
| Q-ONB-001..004 | Checklist / resubmit / auto-approve | Partially: human approval workflow required; checklist Open |
| Q-IDV-001..008 | Thresholds / providers / liveness | Open (workflow mandatory; thresholds Open) |
| Q-OCR-001 | OCR provider? | Open (integration point required) |

## H. Notifications & Communications

| ID | Question | Status |
|----|----------|--------|
| Q-NTF-001/002 | Email/SMS providers? | Open |
| Q-NTF-003..006 | Reminder recipients / matrix / prefs | Partially: 24h reminder required |
| Q-COMMS-001 | Chat in V1? | **Answered: Yes** (ADR-009) |

## I. Ads, Catalog, Ratings, Quality

| ID | Question | Status |
|----|----------|--------|
| Q-ADS-001..005 | Ads monetization / slots | **Answered (directional):** Admin-managed promotions/packages/featured/slots; no self-serve ads marketplace (ADR-007) |
| Q-PRD-001 | Product inventory model? | **N/A — products removed** (ADR-002) |
| Q-RATE-001 | Editable ratings? | Open |
| Q-QUA-001/002 | Survey/score formula? | Open |
| Q-QUA-003 | Restriction rules? | **Answered:** warn → flag → restrict visibility; **admin review**; no permanent auto-block (ADR-008) |
| Q-QUA-004 | Progress SLAs? | Open |

## J. Platform Engineering

| ID | Question | Status |
|----|----------|--------|
| Q-DEP-001 | Cloud provider? | Open |
| Q-DEP-002 | Worker process? | **Answered: Required separate workers** (ADR-012) |
| Q-DEP-003 | Redis in V1? | **Answered: Required** (ADR-012) |
| Q-DEP-004/005 | RPO/RTO / Flyway job style? | Open |
| Q-BE-001 | Virtual threads? | Open |
| Q-FE-001/002 | Monorepo / cookie auth? | Open |
| Q-FL-001/002 | Riverpod vs Bloc / OS versions? | Open |
| Q-DB-001..003 | Translations / UUID / retention? | Open (account deletion rules required) |
| Q-STO-001 / Q-MAP-001 | Storage / maps? | Open |
| Q-API-001 | Swagger in prod? | Open |
| Q-OBS-* / Q-SEC-* / Q-RPT-* / Q-CICD-* / Q-KPI-* | Ops details | Partially: Admin MFA yes; malware scan Open; pen-test Open |

---

## Decision Log

| ID | Decision | Date | Links |
|----|----------|------|-------|
| Q-AUTH-007 / BR-008 | Admins web-only; no mobile admin APIs | 2026-07-24 | ADR-006 |
| ADR-013 | Commission/cancellation/refund/withdrawal/settlement rules admin-configurable; never hardcoded | 2026-07-24 | ADR-013 |
| Q-AUTH-002 | Admin MFA required | 2026-07-24 | ADR-006 |
| Q-LOC-001..003 | Lebanon / AR+EN / USD | 2026-07-24 | ADR-001 |
| Products | Out of scope for stores | 2026-07-24 | ADR-002 |
| Provider/Listing | Unified marketplace model | 2026-07-24 | ADR-003 |
| Ledger | Mandatory | 2026-07-24 | ADR-004 |
| Q-BOOK-008/009 | Confirm then pay; customer selects provider | 2026-07-24 | ADR-005 |
| Ads | Admin-managed promotions | 2026-07-24 | ADR-007 |
| Q-QUA-003 | No permanent auto-block | 2026-07-24 | ADR-008 |
| Q-COMMS-001 | Chat in V1 | 2026-07-24 | ADR-009 |
| Design | Inspiration + improve; design system first | 2026-07-24 | ADR-010 |
| Mobile | Flutter | 2026-07-24 | ADR-011 |
| Redis/Workers | Required | 2026-07-24 | ADR-012 |

---

## Still Blocking Money Go-Live

Policy **structures** are decided (ADR-013 — admin configurable).  
**Numeric commercial values** are entered by Finance Admin (not invented in code/docs as production truth).

Before money go-live, Finance Admin must configure at least one active set for Lebanon:

1. Commission rule(s)  
2. Cancellation policy set  
3. Refund rule set  
4. Withdrawal method(s) + mins/approval  
5. Settlement rule  

Open questions Q-COM-*/Q-BOOK-001..004/Q-SET-* become **configuration content**, not justification to hardcode.  
