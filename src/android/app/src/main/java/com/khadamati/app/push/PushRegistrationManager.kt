package com.khadamati.app.push

import android.Manifest
import android.app.Activity
import android.content.pm.PackageManager
import android.os.Build
import android.util.Log
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import com.google.firebase.FirebaseApp
import com.google.firebase.messaging.FirebaseMessaging
import com.khadamati.app.KhadamatiApplication
import com.khadamati.app.data.preferences.PushTokenManager
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch
import kotlinx.coroutines.tasks.await

/**
 * Requests notification permission, fetches the FCM token, and syncs it with the backend.
 * Falls back to a dev-prefixed token when Firebase is unavailable (e.g. placeholder google-services.json).
 */
object PushRegistrationManager {
    private const val TAG = "PushRegistration"
    const val NOTIFICATION_PERMISSION_REQUEST_CODE = 4101

    private val scope = CoroutineScope(SupervisorJob() + Dispatchers.IO)

    fun initialize(application: KhadamatiApplication) {
        scope.launch {
            ensureFirebaseInitialized(application)
            refreshFcmToken(application)
        }
    }

    fun requestNotificationPermissionIfNeeded(activity: Activity) {
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.TIRAMISU) return
        if (ContextCompat.checkSelfPermission(
                activity,
                Manifest.permission.POST_NOTIFICATIONS,
            ) == PackageManager.PERMISSION_GRANTED
        ) {
            return
        }
        ActivityCompat.requestPermissions(
            activity,
            arrayOf(Manifest.permission.POST_NOTIFICATIONS),
            NOTIFICATION_PERMISSION_REQUEST_CODE,
        )
    }

    fun onNotificationPermissionResult(application: KhadamatiApplication, granted: Boolean) {
        if (!granted) {
            Log.w(TAG, "POST_NOTIFICATIONS denied — push delivery may be limited on Android 13+")
        }
        scope.launch { refreshFcmToken(application) }
    }

    fun onFcmTokenReceived(application: KhadamatiApplication, token: String) {
        val pushTokenManager = application.container.pushTokenManager
        val previous = pushTokenManager.getStoredToken()
        pushTokenManager.updateToken(token)
        if (previous != token) {
            scope.launch { syncTokenWithBackendIfLoggedIn(application) }
        }
    }

    suspend fun refreshFcmToken(application: KhadamatiApplication) {
        if (!ensureFirebaseInitialized(application)) {
            ensureDevFallbackToken(application.container.pushTokenManager)
            return
        }

        runCatching {
            val token = FirebaseMessaging.getInstance().token.await()
            onFcmTokenReceived(application, token)
        }.onFailure { error ->
            Log.w(TAG, "FCM token fetch failed — using dev fallback token", error)
            ensureDevFallbackToken(application.container.pushTokenManager)
        }
    }

    suspend fun syncTokenWithBackendIfLoggedIn(application: KhadamatiApplication) {
        if (application.container.tokenManager.getAccessToken().isNullOrBlank()) return
        runCatching {
            application.container.pushRepository.registerCurrentDevice()
        }.onFailure { error ->
            Log.w(TAG, "Failed to register push token with API", error)
        }
    }

    private fun ensureDevFallbackToken(pushTokenManager: PushTokenManager) {
        val stored = pushTokenManager.getStoredToken()
        if (stored.isNullOrBlank() || !pushTokenManager.isSimulatedToken(stored)) {
            pushTokenManager.getOrCreateToken()
        }
    }

    private fun ensureFirebaseInitialized(application: KhadamatiApplication): Boolean {
        return runCatching {
            if (FirebaseApp.getApps(application).isEmpty()) {
                FirebaseApp.initializeApp(application)
            }
            FirebaseApp.getApps(application).isNotEmpty()
        }.getOrDefault(false)
    }
}
