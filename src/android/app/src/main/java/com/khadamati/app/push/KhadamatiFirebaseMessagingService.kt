package com.khadamati.app.push

import android.util.Log
import com.google.firebase.messaging.FirebaseMessagingService
import com.google.firebase.messaging.RemoteMessage
import com.khadamati.app.KhadamatiApplication

/**
 * Receives FCM registration token updates and foreground/background messages.
 */
class KhadamatiFirebaseMessagingService : FirebaseMessagingService() {

    override fun onNewToken(token: String) {
        super.onNewToken(token)
        val app = applicationContext as? KhadamatiApplication ?: return
        PushRegistrationManager.onFcmTokenReceived(app, token)
    }

    override fun onMessageReceived(message: RemoteMessage) {
        super.onMessageReceived(message)
        Log.d(TAG, "FCM message from=${message.from} data=${message.data}")
    }

    companion object {
        private const val TAG = "KhadamatiFCM"
    }
}
