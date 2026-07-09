# KHADAMATI Android

Native Android client for the **KHADAMATI** maintenance and home services marketplace.

## Stack

| Layer | Technology |
|-------|------------|
| Language | Kotlin |
| UI | Jetpack Compose + Material Design 3 |
| Architecture | MVVM |
| Networking | Retrofit 2 + OkHttp |
| Local cache | Room |
| Auth tokens | DataStore + EncryptedSharedPreferences |
| Navigation | Navigation Compose |
| Images | Coil |

**Package:** `com.khadamati.app`  
**Min SDK:** 26 · **Target SDK:** 35

## Project structure

```
app/src/main/java/com/khadamati/app/
├── data/
│   ├── local/          # Room entities, DAOs, database
│   ├── remote/         # ApiService, RetrofitClient, DTOs
│   ├── repository/     # AuthRepository, ServicesRepository
│   └── preferences/    # TokenManager (JWT storage)
├── domain/model/       # Domain models + mappers
├── di/                 # AppContainer (factory pattern)
└── ui/
    ├── theme/          # Material 3 theme
    ├── navigation/     # NavGraph, routes
    ├── screens/        # Compose screens
    └── viewmodel/      # AuthViewModel, ServicesViewModel
```

## Configuration

### API base URL

Set in `gradle.properties`:

```properties
API_BASE_URL=http://10.0.2.2:5000/api/v1/
```

| Environment | URL |
|-------------|-----|
| Android Emulator | `http://10.0.2.2:5000/api/v1/` |
| Physical device (same LAN) | `http://<your-machine-ip>:5000/api/v1/` |
| Production | `https://api.khadamati.example/api/v1/` |

The value is exposed as `BuildConfig.API_BASE_URL` at compile time.

Cleartext HTTP is enabled for local development (`usesCleartextTraffic="true"`). Disable or use network security config for production HTTPS.

### Backend alignment

Endpoints mirror the KHADAMATI .NET API:

- `POST /auth/login`, `/auth/register`, `/auth/refresh`, `/auth/revoke`
- `GET /users/me`
- `GET /services/categories`, `GET /services`

## Authentication

`TokenManager` stores JWT access/refresh tokens using:

1. **DataStore Preferences** (primary read path)
2. **EncryptedSharedPreferences** (encrypted backup)

`AuthInterceptor` attaches the Bearer token. `RetrofitClient` uses an OkHttp `Authenticator` to refresh tokens on 401 responses.

## Localization

- `res/values/strings.xml` — English (default)
- `res/values-ar/strings.xml` — Arabic

RTL is supported via `android:supportsRtl="true"`.

## Push notifications (FCM)

The app integrates Firebase Cloud Messaging:

1. Create a Firebase project and add an Android app with package `com.khadamati.app`.
2. Download `google-services.json` and replace `app/google-services.json` (see `app/google-services.json.example`).
3. Enable Cloud Messaging in the Firebase console.

On launch the app requests `POST_NOTIFICATIONS` (Android 13+), fetches the FCM registration token, and registers it with `POST /devices/push-token` after sign-in. If Firebase is unavailable (e.g. placeholder config), a `dev-android-*` fallback token is used for local API testing.

`KhadamatiFirebaseMessagingService` handles token refresh and incoming messages.

## Build

Open `/workspace/src/android` in Android Studio (Ladybug or newer recommended), sync Gradle, and run on an emulator or device.

From the command line (with Android SDK installed):

```bash
cd /workspace/src/android
./gradlew assembleDebug
```

## Screens

| Screen | Route | Description |
|--------|-------|-------------|
| Splash | `splash` | Session check, routes to home or login |
| Login | `login` | Email/password sign-in |
| Register | `register` | New account (Customer, Craftsman, Store) |
| Home | `home` | Hero, CTA, how-it-works |
| Services | `services` | Categories + service list (cached) |
| Profile | `profile` | User profile, logout |

Bottom navigation: Home · Services · Profile.

## Dependency injection

`AppContainer` wires dependencies manually. Replace with Hilt or Koin as the app grows:

```kotlin
val container = (application as KhadamatiApplication).container
val authViewModel = container.provideAuthViewModel()
```

## Next steps

- [ ] Add Hilt for DI
- [ ] Service request booking flow
- [x] Push notifications (FCM)
- [ ] Instrumented UI tests
- [ ] Release signing + Play Store config
- [ ] Network security config for production

## License

Part of the KHADAMATI monorepo.
