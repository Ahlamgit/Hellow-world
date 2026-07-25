# ADR-006: Admin Web-Only + MFA Required

## Status
Accepted — 2026-07-24

## Decision
- Administrators authenticate **only** via Administration Portal (web).
- No admin login in Customer/Craftsman mobile apps.
- No admin functionality on mobile APIs.
- Login audience enforcement: admin roles require `admin-web`.
- **MFA is required** for admin users in V1.

## Consequences
Answers Q-AUTH-007, Q-AUTH-002. Aligns BR-008 and security architecture.
