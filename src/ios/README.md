# KHADAMATI iOS

Native iOS client for the KHADAMATI maintenance and home services marketplace.

## Stack

| Layer | Technology |
|-------|------------|
| Language | Swift 5.9+ |
| UI | SwiftUI |
| Architecture | MVVM |
| Networking | `URLSession` + async/await |
| Auth storage | Keychain (JWT access + refresh tokens) |
| Localization | English + Arabic (RTL) |

**Bundle ID:** `com.khadamati.app`

## Project Structure

```
ios/
├── Khadamati.xcodeproj/          # Xcode project
└── Khadamati/
    ├── KhadamatiApp.swift        # App entry point
    ├── ContentView.swift         # Root navigation (auth gate + TabView)
    ├── Info.plist
    ├── Assets.xcassets/
    ├── Core/
    │   ├── AppSession.swift      # Global auth & locale state
    │   ├── Network/
    │   │   ├── APIClient.swift   # HTTP client with token refresh
    │   │   ├── APIEndpoints.swift
    │   │   ├── AuthService.swift
    │   │   └── NetworkError.swift
    │   ├── Models/
    │   └── Storage/
    │       └── TokenStorage.swift # Keychain JWT storage
    ├── Features/
    │   ├── Auth/                 # Login & Register
    │   ├── Home/
    │   ├── Services/
    │   └── Profile/
    ├── Shared/Theme/
    │   ├── AppTheme.swift
    │   └── L10n.swift
    └── Resources/
        ├── en.lproj/Localizable.strings
        └── ar.lproj/Localizable.strings
```

## Requirements

- Xcode 15.0+
- iOS 17.0+
- KHADAMATI API running (default: `http://localhost:5000/api/v1`)

## Getting Started

1. Open `Khadamati.xcodeproj` in Xcode.
2. Select the **Khadamati** scheme and an iOS Simulator or device.
3. For local API development, set the scheme environment variable:
   - **Product → Scheme → Edit Scheme → Run → Arguments → Environment Variables**
   - `KHADAMATI_API_URL` = `http://localhost:5000/api/v1`
4. Build and run (`⌘R`).

## API Integration

The app targets the KHADAMATI backend at `/api/v1`:

| Endpoint | Usage |
|----------|-------|
| `POST /auth/login` | Sign in |
| `POST /auth/register` | Create account |
| `POST /auth/refresh` | Refresh JWT (automatic on 401) |
| `POST /auth/revoke` | Sign out |
| `GET /users/me` | Profile |
| `GET /services/categories` | Home & Services filters |
| `GET /services` | Service listings |

## Push notifications (APNs)

The app registers for Apple Push Notification service on launch:

1. Enable **Push Notifications** capability in Xcode (Signing & Capabilities).
2. Use `Khadamati.entitlements` (`aps-environment` = development for debug builds).
3. Configure backend `Push:Apns` (Team ID, Key ID, `.p8` private key, bundle ID) when using `Push:Provider` = `firebase`.

`PushNotificationManager` requests user authorization, calls `registerForRemoteNotifications()`, and stores the hex device token via `PushTokenStorage`. The token is registered with `POST /devices/push-token` after sign-in. On simulator or when APNs registration fails, a `dev-ios-*` fallback token is used.

Token refresh triggers automatic re-registration when the user is signed in.

## Security

- Access and refresh tokens are stored in the iOS Keychain (`kSecAttrAccessibleAfterFirstUnlockThisDeviceOnly`).
- `APIClient` attaches `Authorization: Bearer <token>` to authenticated requests.
- On `401`, tokens are refreshed automatically; failed refresh clears Keychain and returns to login.

## Localization

- **English** (`en`) and **Arabic** (`ar`) string tables in `Resources/`.
- RTL layout is applied when Arabic is selected via `AppSession.layoutDirection`.
- User language preference syncs with the backend `preferredLanguage` field.

## Architecture Notes

- **MVVM:** Views observe `@StateObject` / `@Published` ViewModels.
- **AppSession:** Shared `ObservableObject` for authentication state and locale.
- **Dependency injection:** Services accept protocols for testability (`AuthServiceProtocol`, `TokenStorageProtocol`).

## License

Part of the KHADAMATI monorepo.
