package com.khadamati.app

import android.app.Application
import com.khadamati.app.di.AppContainer
import com.khadamati.app.push.PushRegistrationManager

class KhadamatiApplication : Application() {

    lateinit var container: AppContainer
        private set

    override fun onCreate() {
        super.onCreate()
        container = AppContainer(this)
        PushRegistrationManager.initialize(this)
    }
}
