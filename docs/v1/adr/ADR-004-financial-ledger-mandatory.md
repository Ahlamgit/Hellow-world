# ADR-004: Financial Ledger Mandatory

## Status
Accepted — 2026-07-24 (Master Prompt v1.0) + Architecture Audit

## Context
Wallet balances alone are insufficient for escrow, commissions, withdrawals, and auditability.

## Decision
Implement immutable double-entry (or equivalent debit/credit) **ledger**.

All payment captures, refunds, commissions, subscription charges, and withdrawal movements post ledger entries.

Financial rows are not soft-deleted; corrections use reversing entries.

## Consequences
Answers audit P0. Enables escrow readiness and investor-grade financial reporting.
