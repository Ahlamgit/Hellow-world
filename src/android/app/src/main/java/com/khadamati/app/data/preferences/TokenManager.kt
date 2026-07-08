package com.khadamati.app.data.preferences

import android.content.Context
import androidx.datastore.core.DataStore
import androidx.datastore.preferences.core.Preferences
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import androidx.security.crypto.EncryptedSharedPreferences
import androidx.security.crypto.MasterKey
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.map

private val Context.dataStore: DataStore<Preferences> by preferencesDataStore(name = "khadamati_prefs")

class TokenManager(context: Context) {

    private val dataStore = context.dataStore

    private val masterKey = MasterKey.Builder(context)
        .setKeyScheme(MasterKey.KeyScheme.AES256_GCM)
        .build()

    private val securePrefs = EncryptedSharedPreferences.create(
        context,
        PREFS_FILE,
        masterKey,
        EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
        EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM,
    )

    private val accessTokenKey = stringPreferencesKey("access_token")
    private val refreshTokenKey = stringPreferencesKey("refresh_token")
    private val expiresAtKey = stringPreferencesKey("expires_at")

    val isLoggedIn: Flow<Boolean> = dataStore.data.map { prefs ->
        !prefs[accessTokenKey].isNullOrBlank()
    }

    suspend fun getAccessToken(): String? =
        dataStore.data.first()[accessTokenKey]
            ?: securePrefs.getString(KEY_ACCESS, null)

    suspend fun getRefreshToken(): String? =
        dataStore.data.first()[refreshTokenKey]
            ?: securePrefs.getString(KEY_REFRESH, null)

    suspend fun saveTokens(accessToken: String, refreshToken: String, expiresAt: String) {
        dataStore.edit { prefs ->
            prefs[accessTokenKey] = accessToken
            prefs[refreshTokenKey] = refreshToken
            prefs[expiresAtKey] = expiresAt
        }
        securePrefs.edit()
            .putString(KEY_ACCESS, accessToken)
            .putString(KEY_REFRESH, refreshToken)
            .putString(KEY_EXPIRES, expiresAt)
            .apply()
    }

    suspend fun clearTokens() {
        dataStore.edit { it.clear() }
        securePrefs.edit().clear().apply()
    }

    companion object {
        private const val PREFS_FILE = "khadamati_secure_prefs"
        private const val KEY_ACCESS = "access_token"
        private const val KEY_REFRESH = "refresh_token"
        private const val KEY_EXPIRES = "expires_at"
    }
}
