package com.khadamati.app.di

import android.content.Context
import androidx.room.Room
import com.khadamati.app.data.local.KhadamatiDatabase
import com.khadamati.app.data.preferences.PushTokenManager
import com.khadamati.app.data.preferences.TokenManager
import com.khadamati.app.data.remote.RetrofitClient
import com.khadamati.app.data.repository.AddressRepository
import com.khadamati.app.data.repository.AuthRepository
import com.khadamati.app.data.repository.BookingRepository
import com.khadamati.app.data.repository.ChatRepository
import com.khadamati.app.data.repository.NotificationRepository
import com.khadamati.app.data.repository.PortalRepository
import com.khadamati.app.data.repository.SupportRepository
import com.khadamati.app.data.repository.ServicesRepository
import com.khadamati.app.data.repository.SubscriptionRepository
import com.khadamati.app.location.LocationProvider
import com.khadamati.app.ui.viewmodel.AddressViewModel
import com.khadamati.app.ui.viewmodel.AuthViewModel
import com.khadamati.app.ui.viewmodel.BookingViewModel
import com.khadamati.app.ui.viewmodel.ChatViewModel
import com.khadamati.app.ui.viewmodel.NotificationsViewModel
import com.khadamati.app.ui.viewmodel.CraftsmanPortalViewModel
import com.khadamati.app.ui.viewmodel.StorePortalViewModel
import com.khadamati.app.ui.viewmodel.SupportViewModel
import com.khadamati.app.ui.viewmodel.SubscriptionViewModel

/**
 * Simple service-locator factory for dependency wiring.
 * Swap with Hilt/Koin when the project grows.
 */
class AppContainer(context: Context) {

    private val appContext = context.applicationContext

    val tokenManager: TokenManager by lazy { TokenManager(appContext) }
    val pushTokenManager: PushTokenManager by lazy { PushTokenManager(appContext) }
    val locationProvider: LocationProvider by lazy { LocationProvider(appContext) }

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

    val pushRepository: PushRepository by lazy {
        PushRepository(apiService, pushTokenManager)
    }

    val authRepository: AuthRepository by lazy {
        AuthRepository(
            apiService = apiService,
            userDao = database.userDao(),
            tokenManager = tokenManager,
            pushRepository = pushRepository,
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

    val notificationRepository: NotificationRepository by lazy {
        NotificationRepository(apiService)
    }

    val subscriptionRepository: SubscriptionRepository by lazy {
        SubscriptionRepository(apiService)
    }

    val addressRepository: AddressRepository by lazy {
        AddressRepository(apiService)
    }

    val chatRepository: ChatRepository by lazy {
        ChatRepository(apiService)
    }

    val supportRepository: SupportRepository by lazy {
        SupportRepository(apiService)
    }

    val portalRepository: PortalRepository by lazy {
        PortalRepository(apiService)
    }

    fun provideAuthViewModel(): AuthViewModel =
        AuthViewModel(authRepository)

    fun provideServicesViewModel(): ServicesViewModel =
        ServicesViewModel(servicesRepository)

    fun provideBookingViewModel(): BookingViewModel =
        BookingViewModel(bookingRepository)

    fun provideNotificationsViewModel(): NotificationsViewModel =
        NotificationsViewModel(notificationRepository)

    fun provideSubscriptionViewModel(): SubscriptionViewModel =
        SubscriptionViewModel(subscriptionRepository)

    fun provideAddressViewModel(): AddressViewModel =
        AddressViewModel(addressRepository)

    fun provideChatViewModel(): ChatViewModel =
        ChatViewModel(chatRepository)

    fun provideSupportViewModel(): SupportViewModel =
        SupportViewModel(supportRepository)

    fun provideCraftsmanPortalViewModel(): CraftsmanPortalViewModel =
        CraftsmanPortalViewModel(portalRepository)

    fun provideStorePortalViewModel(): StorePortalViewModel =
        StorePortalViewModel(portalRepository)
}
