package com.khadamati.app

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.Surface
import androidx.compose.ui.Modifier
import com.khadamati.app.push.PushRegistrationManager
import com.khadamati.app.ui.navigation.KhadamatiNavGraph
import com.khadamati.app.ui.theme.KhadamatiTheme

class MainActivity : ComponentActivity() {

    private val container by lazy {
        (application as KhadamatiApplication).container
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        PushRegistrationManager.requestNotificationPermissionIfNeeded(this)

        setContent {
            KhadamatiTheme {
                Surface(modifier = Modifier.fillMaxSize()) {
                    KhadamatiNavGraph(container = container)
                }
            }
        }
    }

    override fun onRequestPermissionsResult(
        requestCode: Int,
        permissions: Array<out String>,
        grantResults: IntArray,
    ) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        if (requestCode == PushRegistrationManager.NOTIFICATION_PERMISSION_REQUEST_CODE) {
            val granted = grantResults.isNotEmpty() &&
                grantResults[0] == android.content.pm.PackageManager.PERMISSION_GRANTED
            PushRegistrationManager.onNotificationPermissionResult(application as KhadamatiApplication, granted)
        }
    }
}
