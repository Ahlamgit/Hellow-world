# Secret Rotation Plan (Documentation Only)

**Status:** Prepared — **do not execute** until Phase 1B+ approval and scheduled maintenance window.  
**Phase 1B scope:** This document only. No secrets are rotated in Phase 1B.

---

## 1. JWT signing key rotation

| Step | Action |
|------|--------|
| 1 | Generate new `Jwt:Secret` (min 32 chars, cryptographically random) |
| 2 | Deploy API with **dual validation** window (optional): accept tokens signed with old + new key for 15–30 minutes |
| 3 | Update `Jwt:Secret` in all environments (staging first) |
| 4 | Force refresh token rotation on next login (existing refresh tokens remain valid until expiry unless revoked) |
| 5 | Revoke all refresh tokens if breach suspected (`RevokeAllRefreshTokensAsync`) |
| 6 | Remove old key from configuration after access-token TTL elapses |

**Impact:** All users receive new access tokens on next refresh; active sessions continue until refresh fails or logout.

---

## 2. SQL credential rotation

| Step | Action |
|------|--------|
| 1 | Create new SQL login with same `db_owner` / least-privilege role |
| 2 | Update `ConnectionStrings:DefaultConnection` in secret store / env vars |
| 3 | Rolling restart API instances |
| 4 | Disable old login after health checks pass |
| 5 | Audit `RefreshTokens`, `AuditLogs` for anomalous access during window |

**Impact:** Brief connection errors during rolling deploy if not coordinated.

---

## 3. Git history cleanup plan

If secrets were ever committed:

1. Identify commits containing secrets (`git log -S`, `trufflehog`, GitHub secret scanning).
2. Rotate affected secrets **before** history rewrite (rotation makes leaked secrets useless).
3. Use `git filter-repo` or BFG to remove files/lines.
4. Force-push protected branches only after team coordination.
5. Invalidate all CI/CD caches and redeploy from clean history.

**KHADAMATI note:** `appsettings.json` contains development placeholders only. Production uses environment variables per `appsettings.Production.json`.

---

## 4. Payment / webhook secrets

| Secret | Config key | Rotation |
|--------|------------|----------|
| Moyasar API key | `Payment:Moyasar:ApiKey` | Moyasar dashboard → update → deploy |
| Moyasar webhook secret | `Payment:Moyasar:WebhookSecret` | Regenerate in Moyasar → update both sides → test webhook |
| Moyasar publishable key | `Payment:Moyasar:PublishableKey` | Client-side; low risk |

After webhook secret rotation, send test webhook from Moyasar dashboard before production cutover.

---

## 5. Deployment impact summary

| Secret type | Downtime | User impact |
|-------------|----------|-------------|
| JWT | None (rolling) | Re-login on refresh failure |
| SQL | None (rolling) | None if health checks pass |
| Webhook | None | Payment confirmations fail until both sides updated |
| SMTP/SMS/push | None | Notifications fail until updated |

---

## 6. Approval checklist (before execution)

- [ ] Maintenance window scheduled
- [ ] Staging rotation tested end-to-end
- [ ] Rollback credentials retained (time-boxed)
- [ ] Monitoring alerts configured for 401/403 spikes and webhook failures
- [ ] Stakeholder sign-off

**Phase 1B does not execute any item in this plan.**
