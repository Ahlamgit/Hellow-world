# BLOCKER-003 — Vendor Responsibility Matrix

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-RESP-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |

---

## Responsibility matrix

| Role | Responsibility | BLOCKER-003 closure |
|------|----------------|---------------------|
| **Project Owner / Business Owner** | Vendor category approval (SMS/email/storage; maps excluded) | **Complete** |
| **Integration Lead** | Vendor selection evidence · sandbox · dossier completion | **Pending** |
| **Technical Architect** | Ports/adapters · security architecture · final validation | **Pending** |
| **Administrator** | Operational enforcement of approved vendor usage limits | Post–Gate A |
| **Program Governance Manager** | Evidence archival · tracker integrity | **In progress** |

---

## Vendor capability ownership

| Capability | Business owner | Technical owner | Ops owner |
|------------|----------------|-----------------|-----------|
| SMS OTP | PO/BO (purpose) | Integration Lead | Administrator (config) |
| Email verification | PO/BO (purpose) | Integration Lead | Administrator (config) |
| File storage | PO/BO (purpose) | Integration Lead + TA | DevOps (infra) |
| Payment (IXOPAY) | PO/BO (BLOCKER-007) | Integration Lead | Finance ops |
| Maps | **Excluded** — PO/BO | N/A V1 | N/A V1 |
