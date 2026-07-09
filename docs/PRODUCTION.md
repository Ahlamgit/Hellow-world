# Production deployment

Configure external integrations via environment variables or a secrets manager. The API reads standard `appsettings.Production.json` keys; override any value with the `Section__Key` environment variable pattern (ASP.NET Core configuration).

## Provider switches

| Integration | Config key | Production value |
|-------------|------------|------------------|
| Payments | `Payment:Provider` | `Moyasar` |
| Push | `Push:Provider` | `firebase` |
| Email | `Email:Provider` | `SendGrid` or `smtp` |
| SMS | `Sms:Provider` | `Twilio` |

## Required secrets

### Database
- `ConnectionStrings__DefaultConnection` — SQL Server connection string

### JWT
- `Jwt__Secret` — minimum 32 characters

### Moyasar (payments)
- `Payment__Moyasar__SecretKey`
- `Payment__Moyasar__PublishableKey`
- `Payment__Moyasar__WebhookSecret` — validates `X-Moyasar-Signature` on webhooks
- `Payment__Moyasar__CallbackUrl` — `https://<api-host>/api/v1/webhooks/moyasar`
- `Payment__Moyasar__SuccessUrl` — post-checkout redirect

### Firebase / FCM (Android push)
- `Push__Firebase__ServerKey` — FCM legacy server key
- `Push__Firebase__ProjectId`

### APNs (iOS push)
- `Push__Apns__TeamId`
- `Push__Apns__KeyId`
- `Push__Apns__BundleId` — `com.khadamati.app`
- `Push__Apns__PrivateKey` — `.p8` key contents (use `\n` for newlines in env vars)
- `Push__Apns__UseSandbox` — `false` for App Store builds

### Email (SendGrid example)
- `Email__SendGrid__ApiKey`
- `Email__SendGrid__FromAddress`

### SMS (Twilio)
- `Sms__Twilio__AccountSid`
- `Sms__Twilio__AuthToken`
- `Sms__Twilio__FromNumber`

## Mobile clients

### Android
Replace `src/android/app/google-services.json` with the file from Firebase Console (package `com.khadamati.app`).

### iOS
- Enable **Push Notifications** in Xcode Signing & Capabilities
- Set `aps-environment` to `production` in `Khadamati.entitlements` for release builds
- Upload APNs key to Apple Developer portal; mirror credentials in `Push:Apns` above

## Readiness checks

After deployment, verify integrations:

```bash
# Liveness
curl https://api.khadamati.com/api/v1/health

# Integration readiness (no auth)
curl https://api.khadamati.com/api/v1/health/integrations

# Admin system health (requires admin JWT)
curl -H "Authorization: Bearer <token>" https://api.khadamati.com/api/v1/admin/system/health
```

`productionReady: true` means all four provider families (payment, push, email, SMS) report **Ready** status.

Startup logs warn when any provider is still on **Development** or **Misconfigured**.

## Smoke tests

1. **Payment** — create a booking, open Moyasar checkout, complete payment, confirm webhook updates booking status
2. **Push** — sign in on a physical device, confirm `POST /devices/push-token` stores a non-`dev-` token, trigger a booking notification
3. **Email** — register a user and confirm verification email is delivered (not logged only)

See `appsettings.Production.json` for the full production template with `${PLACEHOLDER}` variable names.
