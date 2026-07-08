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
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.currentBackStackEntryAsState
import androidx.navigation.compose.rememberNavController
import com.khadamati.app.R
import com.khadamati.app.di.AppContainer
import com.khadamati.app.ui.screens.HomeScreen
import com.khadamati.app.ui.screens.LoginScreen
import com.khadamati.app.ui.screens.ProfileScreen
import com.khadamati.app.ui.screens.RegisterScreen
import com.khadamati.app.ui.screens.ServicesScreen
import com.khadamati.app.ui.screens.SplashScreen
import com.khadamati.app.ui.viewmodel.AuthViewModel
import com.khadamati.app.ui.viewmodel.ServicesViewModel

@Composable
fun KhadamatiNavGraph(container: AppContainer) {
    val navController = rememberNavController()
    val authViewModel: AuthViewModel = viewModel(factory = AppViewModelFactory(container) { container.provideAuthViewModel() })
    val servicesViewModel: ServicesViewModel = viewModel(factory = AppViewModelFactory(container) { container.provideServicesViewModel() })

    val navBackStackEntry by navController.currentBackStackEntryAsState()
    val currentRoute = navBackStackEntry?.destination?.route

    val bottomNavRoutes = remember { setOf(Routes.HOME, Routes.SERVICES, Routes.PROFILE) }
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
                    onNavigateToHome = {
                        navController.navigate(Routes.HOME) {
                            popUpTo(Routes.LOGIN) { inclusive = true }
                        }
                    },
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
                ServicesScreen(viewModel = servicesViewModel)
            }

            composable(Routes.PROFILE) {
                val authState by authViewModel.uiState.collectAsStateWithLifecycle()
                ProfileScreen(
                    authViewModel = authViewModel,
                    isAuthenticated = authState.isAuthenticated,
                    onNavigateToLogin = {
                        navController.navigate(Routes.LOGIN) {
                            popUpTo(Routes.HOME)
                        }
                    },
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
            selected = currentRoute == Routes.PROFILE,
            onClick = { onNavigate(Routes.PROFILE) },
            icon = { Icon(Icons.Default.Person, contentDescription = null) },
            label = { Text(stringResource(R.string.nav_profile)) },
        )
    }
}
