package com.khadamati.app.ui.navigation

import androidx.compose.foundation.layout.padding
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Home
import androidx.compose.material.icons.filled.Person
import androidx.compose.material.icons.filled.Build
import androidx.compose.material3.Icon
import androidx.compose.material3.NavigationBar
import androidx.compose.material3.NavigationBarItem
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavGraph.Companion.findStartDestination
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.currentBackStackEntryAsState
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.khadamati.app.R
import com.khadamati.app.di.AppContainer
import com.khadamati.app.ui.screens.AddressesScreen
import com.khadamati.app.ui.screens.BookingChatScreen
import com.khadamati.app.ui.screens.ChatListScreen
import com.khadamati.app.ui.screens.HomeScreen
import com.khadamati.app.ui.screens.LoginScreen
import com.khadamati.app.ui.screens.BookingDetailScreen
import com.khadamati.app.ui.screens.BookingWizardScreen
import com.khadamati.app.ui.screens.MyBookingsScreen
import com.khadamati.app.ui.screens.MySubscriptionScreen
import com.khadamati.app.ui.screens.NotificationsScreen
import com.khadamati.app.ui.screens.ProfileScreen
import com.khadamati.app.ui.screens.ForgotPasswordScreen
import com.khadamati.app.ui.screens.ResetPasswordScreen
import com.khadamati.app.ui.screens.ChangePasswordScreen
import com.khadamati.app.ui.screens.ServicesScreen
import com.khadamati.app.ui.screens.SplashScreen
import com.khadamati.app.ui.screens.SupportScreen
import com.khadamati.app.ui.screens.ProfileEditScreen
import com.khadamati.app.ui.screens.CraftsmanPortalScreen
import com.khadamati.app.ui.screens.StorePortalScreen
import com.khadamati.app.ui.screens.SubscriptionPlansScreen
import com.khadamati.app.ui.viewmodel.AddressViewModel
import com.khadamati.app.ui.viewmodel.AuthViewModel
import com.khadamati.app.ui.viewmodel.BookingViewModel
import com.khadamati.app.ui.viewmodel.ChatViewModel
import com.khadamati.app.ui.viewmodel.NotificationsViewModel
import com.khadamati.app.ui.viewmodel.ServicesViewModel
import com.khadamati.app.ui.viewmodel.SupportViewModel
import com.khadamati.app.ui.viewmodel.CraftsmanPortalViewModel
import com.khadamati.app.ui.viewmodel.StorePortalViewModel
import com.khadamati.app.ui.viewmodel.isCraftsmanRole
import com.khadamati.app.ui.viewmodel.isStoreRole
import com.khadamati.app.ui.viewmodel.isSubscriberRole
import com.khadamati.app.ui.viewmodel.subscriptionTargetRole

@Composable
fun KhadamatiNavGraph(container: AppContainer) {
    val navController = rememberNavController()
    val authViewModel: AuthViewModel = viewModel(factory = AppViewModelFactory(container) { container.provideAuthViewModel() })
    val servicesViewModel: ServicesViewModel = viewModel(factory = AppViewModelFactory(container) { container.provideServicesViewModel() })
    val bookingViewModel: BookingViewModel = viewModel(factory = AppViewModelFactory(container) { container.provideBookingViewModel() })
    val notificationsViewModel: NotificationsViewModel = viewModel(factory = AppViewModelFactory(container) { container.provideNotificationsViewModel() })
    val subscriptionViewModel: SubscriptionViewModel = viewModel(factory = AppViewModelFactory(container) { container.provideSubscriptionViewModel() })
    val addressViewModel: AddressViewModel = viewModel(factory = AppViewModelFactory(container) { container.provideAddressViewModel() })
    val chatViewModel: ChatViewModel = viewModel(factory = AppViewModelFactory(container) { container.provideChatViewModel() })
    val supportViewModel: SupportViewModel = viewModel(factory = AppViewModelFactory(container) { container.provideSupportViewModel() })
    val craftsmanPortalViewModel: CraftsmanPortalViewModel = viewModel(factory = AppViewModelFactory(container) { container.provideCraftsmanPortalViewModel() })
    val storePortalViewModel: StorePortalViewModel = viewModel(factory = AppViewModelFactory(container) { container.provideStorePortalViewModel() })

    val navBackStackEntry by navController.currentBackStackEntryAsState()
    val currentRoute = navBackStackEntry?.destination?.route

    val bottomNavRoutes = remember { setOf(Routes.HOME, Routes.SERVICES, Routes.BOOKINGS, Routes.PROFILE) }
    val showBottomBar = currentRoute in bottomNavRoutes

    Scaffold(
        bottomBar = {
            if (showBottomBar) {
                KhadamatiBottomBar(
                    currentRoute = currentRoute,
                    onNavigate = { route ->
                        navController.navigate(route) {
                            popUpTo(navController.graph.findStartDestination().id) {
                                saveState = true
                            }
                            launchSingleTop = true
                            restoreState = true
                        }
                    },
                )
            }
        },
    ) { innerPadding ->
        NavHost(
            navController = navController,
            startDestination = Routes.SPLASH,
            modifier = Modifier.padding(innerPadding),
        ) {
            composable(Routes.SPLASH) {
                SplashScreen(
                    authViewModel = authViewModel,
                    onNavigateToHome = {
                        navController.navigate(Routes.HOME) {
                            popUpTo(Routes.SPLASH) { inclusive = true }
                        }
                    },
                    onNavigateToLogin = {
                        navController.navigate(Routes.LOGIN) {
                            popUpTo(Routes.SPLASH) { inclusive = true }
                        }
                    },
                )
            }

            composable(Routes.LOGIN) {
                LoginScreen(
                    authViewModel = authViewModel,
                    onNavigateToRegister = { navController.navigate(Routes.REGISTER) },
                    onNavigateToForgotPassword = { navController.navigate(Routes.FORGOT_PASSWORD) },
                    onNavigateToHome = {
                        navController.navigate(Routes.HOME) {
                            popUpTo(Routes.LOGIN) { inclusive = true }
                        }
                    },
                )
            }

            composable(Routes.FORGOT_PASSWORD) {
                ForgotPasswordScreen(
                    authViewModel = authViewModel,
                    onNavigateBack = { navController.popBackStack() },
                    onNavigateToReset = { navController.navigate(Routes.RESET_PASSWORD) },
                )
            }

            composable(Routes.RESET_PASSWORD) {
                ResetPasswordScreen(
                    authViewModel = authViewModel,
                    onNavigateBack = { navController.popBackStack() },
                )
            }

            composable(Routes.CHANGE_PASSWORD) {
                ChangePasswordScreen(
                    authViewModel = authViewModel,
                    onNavigateBack = { navController.popBackStack() },
                )
            }

            composable(Routes.REGISTER) {
                RegisterScreen(
                    authViewModel = authViewModel,
                    onNavigateToLogin = { navController.popBackStack() },
                    onNavigateToHome = {
                        navController.navigate(Routes.HOME) {
                            popUpTo(Routes.REGISTER) { inclusive = true }
                        }
                    },
                )
            }

            composable(Routes.HOME) {
                val authState by authViewModel.uiState.collectAsStateWithLifecycle()
                HomeScreen(
                    isAuthenticated = authState.isAuthenticated,
                    onBrowseServices = { navController.navigate(Routes.SERVICES) },
                    onGetStarted = {
                        if (authState.isAuthenticated) {
                            navController.navigate(Routes.SERVICES)
                        } else {
                            navController.navigate(Routes.LOGIN)
                        }
                    },
                )
            }

            composable(Routes.SERVICES) {
                ServicesScreen(
                    viewModel = servicesViewModel,
                    onServiceClick = { serviceId ->
                        navController.navigate("booking-wizard/$serviceId")
                    },
                )
            }

            composable(Routes.BOOKING_WIZARD) {
                BookingWizardScreen(
                    servicesViewModel = servicesViewModel,
                    bookingViewModel = bookingViewModel,
                    locationProvider = container.locationProvider,
                    preselectedServiceId = null,
                    onNavigateBack = { navController.popBackStack() },
                    onBookingCreated = { bookingId ->
                        navController.navigate("booking/$bookingId") {
                            popUpTo(Routes.BOOKINGS)
                        }
                    },
                )
            }

            composable(
                route = Routes.BOOKING_WIZARD_WITH_SERVICE,
                arguments = listOf(navArgument("serviceId") { type = NavType.StringType }),
            ) { backStack ->
                val serviceId = backStack.arguments?.getString("serviceId") ?: return@composable
                BookingWizardScreen(
                    servicesViewModel = servicesViewModel,
                    bookingViewModel = bookingViewModel,
                    locationProvider = container.locationProvider,
                    preselectedServiceId = serviceId,
                    onNavigateBack = { navController.popBackStack() },
                    onBookingCreated = { bookingId ->
                        navController.navigate("booking/$bookingId") {
                            popUpTo(Routes.SERVICES)
                        }
                    },
                )
            }

            composable(Routes.BOOKINGS) {
                MyBookingsScreen(
                    viewModel = bookingViewModel,
                    onBookingClick = { id -> navController.navigate("booking/$id") },
                    onNewBooking = { navController.navigate(Routes.BOOKING_WIZARD) },
                )
            }

            composable(
                route = Routes.BOOKING_DETAIL,
                arguments = listOf(navArgument("bookingId") { type = NavType.StringType }),
            ) { backStack ->
                val bookingId = backStack.arguments?.getString("bookingId") ?: return@composable
                val authState by authViewModel.uiState.collectAsStateWithLifecycle()
                BookingDetailScreen(
                    bookingId = bookingId,
                    viewModel = bookingViewModel,
                    userRole = authState.currentUser?.role ?: "Customer",
                    onPay = { navController.navigate(Routes.BOOKINGS) },
                    onOpenChat = { id -> navController.navigate("booking/$id/chat") },
                )
            }

            composable(Routes.PROFILE) {
                val authState by authViewModel.uiState.collectAsStateWithLifecycle()
                val userRole = authState.profile?.role ?: authState.currentUser?.role
                val isArabic = (authState.profile?.preferredLanguage ?: authState.currentUser?.preferredLanguage) == "ar"
                ProfileScreen(
                    authViewModel = authViewModel,
                    isAuthenticated = authState.isAuthenticated,
                    showSubscriptions = isSubscriberRole(userRole),
                    showCraftsmanPortal = isCraftsmanRole(userRole),
                    showStorePortal = isStoreRole(userRole),
                    onNavigateToLogin = {
                        navController.navigate(Routes.LOGIN) {
                            popUpTo(Routes.HOME)
                        }
                    },
                    onNavigateToNotifications = { navController.navigate(Routes.NOTIFICATIONS) },
                    onNavigateToSubscriptions = { navController.navigate(Routes.SUBSCRIPTION_PLANS) },
                    onNavigateToMySubscription = { navController.navigate(Routes.MY_SUBSCRIPTION) },
                    onNavigateToAddresses = { navController.navigate(Routes.ADDRESSES) },
                    onNavigateToChats = { navController.navigate(Routes.CHAT_LIST) },
                    onNavigateToSupport = { navController.navigate(Routes.SUPPORT) },
                    onNavigateToProfileEdit = { navController.navigate(Routes.PROFILE_EDIT) },
                    onNavigateToChangePassword = { navController.navigate(Routes.CHANGE_PASSWORD) },
                    onNavigateToCraftsmanPortal = { navController.navigate(Routes.CRAFTSMAN_PORTAL) },
                    onNavigateToStorePortal = { navController.navigate(Routes.STORE_PORTAL) },
                )
            }

            composable(Routes.SUPPORT) {
                SupportScreen(
                    viewModel = supportViewModel,
                    onNavigateBack = { navController.popBackStack() },
                )
            }

            composable(Routes.PROFILE_EDIT) {
                ProfileEditScreen(
                    authViewModel = authViewModel,
                    onNavigateBack = { navController.popBackStack() },
                )
            }

            composable(Routes.CRAFTSMAN_PORTAL) {
                val authState by authViewModel.uiState.collectAsStateWithLifecycle()
                val isArabic = (authState.profile?.preferredLanguage ?: authState.currentUser?.preferredLanguage) == "ar"
                CraftsmanPortalScreen(
                    viewModel = craftsmanPortalViewModel,
                    isArabic = isArabic,
                    onNavigateBack = { navController.popBackStack() },
                )
            }

            composable(Routes.STORE_PORTAL) {
                val authState by authViewModel.uiState.collectAsStateWithLifecycle()
                val isArabic = (authState.profile?.preferredLanguage ?: authState.currentUser?.preferredLanguage) == "ar"
                StorePortalScreen(
                    viewModel = storePortalViewModel,
                    isArabic = isArabic,
                    onNavigateBack = { navController.popBackStack() },
                )
            }

            composable(Routes.NOTIFICATIONS) {
                val authState by authViewModel.uiState.collectAsStateWithLifecycle()
                val isArabic = (authState.profile?.preferredLanguage ?: authState.currentUser?.preferredLanguage) == "ar"
                NotificationsScreen(
                    viewModel = notificationsViewModel,
                    isArabic = isArabic,
                    onNavigateBack = { navController.popBackStack() },
                    onNavigateToBooking = { bookingId ->
                        navController.navigate("booking/$bookingId")
                    },
                )
            }

            composable(Routes.SUBSCRIPTION_PLANS) {
                val authState by authViewModel.uiState.collectAsStateWithLifecycle()
                val userRole = authState.profile?.role ?: authState.currentUser?.role
                val isArabic = (authState.profile?.preferredLanguage ?: authState.currentUser?.preferredLanguage) == "ar"
                SubscriptionPlansScreen(
                    viewModel = subscriptionViewModel,
                    targetRole = subscriptionTargetRole(userRole),
                    isArabic = isArabic,
                    onNavigateBack = { navController.popBackStack() },
                    onNavigateToSubscribe = { planId -> navController.navigate("subscriptions/$planId") },
                    onNavigateToMySubscription = { navController.navigate(Routes.MY_SUBSCRIPTION) },
                )
            }

            composable(
                route = Routes.SUBSCRIBE,
                arguments = listOf(navArgument("planId") { type = NavType.StringType }),
            ) { backStack ->
                val planId = backStack.arguments?.getString("planId") ?: return@composable
                val authState by authViewModel.uiState.collectAsStateWithLifecycle()
                val isArabic = (authState.profile?.preferredLanguage ?: authState.currentUser?.preferredLanguage) == "ar"
                SubscribeScreen(
                    planId = planId,
                    viewModel = subscriptionViewModel,
                    isArabic = isArabic,
                    onNavigateBack = { navController.popBackStack() },
                    onSubscribed = {
                        navController.navigate(Routes.MY_SUBSCRIPTION) {
                            popUpTo(Routes.SUBSCRIPTION_PLANS)
                        }
                    },
                )
            }

            composable(Routes.ADDRESSES) {
                AddressesScreen(
                    viewModel = addressViewModel,
                    locationProvider = container.locationProvider,
                    onNavigateBack = { navController.popBackStack() },
                )
            }

            composable(Routes.CHAT_LIST) {
                ChatListScreen(
                    viewModel = chatViewModel,
                    onNavigateBack = { navController.popBackStack() },
                    onOpenChat = { bookingId -> navController.navigate("booking/$bookingId/chat") },
                )
            }

            composable(
                route = "booking/{bookingId}/chat",
                arguments = listOf(navArgument("bookingId") { type = NavType.StringType }),
            ) { backStack ->
                val bookingId = backStack.arguments?.getString("bookingId") ?: return@composable
                BookingChatScreen(
                    bookingId = bookingId,
                    viewModel = chatViewModel,
                    onNavigateBack = { navController.popBackStack() },
                )
            }

            composable(Routes.MY_SUBSCRIPTION) {
                val authState by authViewModel.uiState.collectAsStateWithLifecycle()
                val isArabic = (authState.profile?.preferredLanguage ?: authState.currentUser?.preferredLanguage) == "ar"
                MySubscriptionScreen(
                    viewModel = subscriptionViewModel,
                    isArabic = isArabic,
                    onNavigateBack = { navController.popBackStack() },
                    onBrowsePlans = { navController.navigate(Routes.SUBSCRIPTION_PLANS) },
                )
            }
        }
    }
}

@Composable
private fun KhadamatiBottomBar(
    currentRoute: String?,
    onNavigate: (String) -> Unit,
) {
    NavigationBar {
        NavigationBarItem(
            selected = currentRoute == Routes.HOME,
            onClick = { onNavigate(Routes.HOME) },
            icon = { Icon(Icons.Default.Home, contentDescription = null) },
            label = { Text(stringResource(R.string.nav_home)) },
        )
        NavigationBarItem(
            selected = currentRoute == Routes.SERVICES,
            onClick = { onNavigate(Routes.SERVICES) },
            icon = { Icon(Icons.Default.Build, contentDescription = null) },
            label = { Text(stringResource(R.string.nav_services)) },
        )
        NavigationBarItem(
            selected = currentRoute == Routes.BOOKINGS,
            onClick = { onNavigate(Routes.BOOKINGS) },
            icon = { Icon(Icons.Default.Build, contentDescription = null) },
            label = { Text(stringResource(R.string.nav_bookings)) },
        )
        NavigationBarItem(
            selected = currentRoute == Routes.PROFILE,
            onClick = { onNavigate(Routes.PROFILE) },
            icon = { Icon(Icons.Default.Person, contentDescription = null) },
            label = { Text(stringResource(R.string.nav_profile)) },
        )
    }
}
