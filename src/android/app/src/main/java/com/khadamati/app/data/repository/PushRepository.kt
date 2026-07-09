package com.khadamati.app.data.repository

import android.os.Build
import com.khadamati.app.data.preferences.PushTokenManager
import com.khadamati.app.data.remote.ApiService
import com.khadamati.app.data.remote.dto.RegisterPushTokenRequestDto

class PushRepository(
    private val apiService: ApiService,
    private val pushTokenManager: PushTokenManager,
) {
    suspend fun registerCurrentDevice() {
        val token = pushTokenManager.getOrCreateToken()
        apiService.registerPushToken(
            RegisterPushTokenRequestDto(
                token = token,
                platform = "Android",
                deviceName = "${Build.MANUFACTURER} ${Build.MODEL}",
            ),
        )
    }

    suspend fun unregisterCurrentDevice() {
        val token = pushTokenManager.getOrCreateToken()
        runCatching { apiService.unregisterPushToken(token) }
    }
}
