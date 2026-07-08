package com.khadamati.app

import android.app.Application
import com.khadamati.app.di.AppContainer

class KhadamatiApplication : Application() {

    lateinit var container: AppContainer
        private set

    override fun onCreate() {
        super.onCreate()
        container = AppContainer(this)
    }
}
