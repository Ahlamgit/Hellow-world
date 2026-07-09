package com.khadamati.app.data.preferences

import android.content.Context
import java.util.UUID

class PushTokenManager(context: Context) {
    private val prefs = context.applicationContext.getSharedPreferences(PREFS, Context.MODE_PRIVATE)

    fun getOrCreateToken(): String {
        val existing = prefs.getString(KEY_TOKEN, null)
        if (!existing.isNullOrBlank()) return existing
        val token = "android-${UUID.randomUUID()}"
        prefs.edit().putString(KEY_TOKEN, token).apply()
        return token
    }

    companion object {
        private const val PREFS = "khadamati_push"
        private const val KEY_TOKEN = "device_push_token"
    }
}
