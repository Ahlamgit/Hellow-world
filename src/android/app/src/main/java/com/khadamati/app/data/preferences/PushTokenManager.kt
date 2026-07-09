package com.khadamati.app.data.preferences

import android.content.Context
import java.util.UUID

/**
 * Stores the device push token. Uses a dev-prefixed simulated token until FCM is integrated.
 * Call [updateToken] when FirebaseMessaging delivers a real registration token.
 */
class PushTokenManager(context: Context) {
    private val prefs = context.applicationContext.getSharedPreferences(PREFS, Context.MODE_PRIVATE)

    fun getOrCreateToken(): String {
        val existing = prefs.getString(KEY_TOKEN, null)
        if (!existing.isNullOrBlank()) return existing
        return createDevToken()
    }

    fun updateToken(token: String) {
        if (token.isBlank()) return
        prefs.edit().putString(KEY_TOKEN, token.trim()).apply()
    }

    fun isSimulatedToken(token: String): Boolean =
        token.startsWith(DEV_PREFIX, ignoreCase = true)

    private fun createDevToken(): String {
        val token = "$DEV_PREFIX${UUID.randomUUID()}"
        prefs.edit().putString(KEY_TOKEN, token).apply()
        return token
    }

    companion object {
        const val DEV_PREFIX = "dev-android-"
        private const val PREFS = "khadamati_push"
        private const val KEY_TOKEN = "device_push_token"
    }
}
