package com.khadamati.app.di

import android.content.Context
import androidx.room.Room
import com.khadamati.app.data.local.KhadamatiDatabase
import com.khadamati.app.data.preferences.TokenManager
import com.khadamati.app.data.remote.RetrofitClient
import com.khadamati.app.data.repository.AuthRepository
import com.khadamati.app.data.repository.BookingRepository
import com.khadamati.app.data.repository.ServicesRepository
import com.khadamati.app.ui.viewmodel.AuthViewModel
import com.khadamati.app.ui.viewmodel.BookingViewModel
import com.khadamati.app.ui.viewmodel.ServicesViewModel

/**
 * Simple service-locator factory for dependency wiring.
 * Swap with Hilt/Koin when the project grows.
 */
class AppContainer(context: Context) {

    private val appContext = context.applicationContext

    val tokenManager: TokenManager by lazy { TokenManager(appContext) }

    private val retrofitClient: RetrofitClient by lazy {
        RetrofitClient(tokenManager)
    }

    val apiService by lazy { retrofitClient.apiService }

    val database: KhadamatiDatabase by lazy {
        Room.databaseBuilder(
            appContext,
            KhadamatiDatabase::class.java,
            "khadamati.db"
        ).fallbackToDestructiveMigration()
            .build()
    }

    val authRepository: AuthRepository by lazy {
        AuthRepository(
            apiService = apiService,
            userDao = database.userDao(),
            tokenManager = tokenManager,
        )
    }

    val servicesRepository: ServicesRepository by lazy {
        ServicesRepository(
            apiService = apiService,
            categoryDao = database.serviceCategoryDao(),
            serviceDao = database.serviceDao(),
        )
    }

    val bookingRepository: BookingRepository by lazy {
        BookingRepository(apiService)
    }

    fun provideAuthViewModel(): AuthViewModel =
        AuthViewModel(authRepository)

    fun provideServicesViewModel(): ServicesViewModel =
        ServicesViewModel(servicesRepository)

    fun provideBookingViewModel(): BookingViewModel =
        BookingViewModel(bookingRepository)
}
