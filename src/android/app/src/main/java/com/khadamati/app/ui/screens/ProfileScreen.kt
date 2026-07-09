package com.khadamati.app.ui.screens

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.material3.Button
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import coil.compose.AsyncImage
import com.khadamati.app.R
import com.khadamati.app.ui.viewmodel.AuthViewModel

@Composable
fun ProfileScreen(
    authViewModel: AuthViewModel,
    isAuthenticated: Boolean,
    showSubscriptions: Boolean = false,
    onNavigateToLogin: () -> Unit,
    onNavigateToNotifications: () -> Unit = {},
    onNavigateToSubscriptions: () -> Unit = {},
    onNavigateToMySubscription: () -> Unit = {},
) {
    val uiState by authViewModel.uiState.collectAsStateWithLifecycle()

    LaunchedEffect(isAuthenticated) {
        if (isAuthenticated && uiState.profile == null) {
            authViewModel.loadProfile()
        }
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(24.dp),
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Top,
    ) {
        Text(
            text = stringResource(R.string.profile_title),
            style = MaterialTheme.typography.headlineMedium,
            fontWeight = FontWeight.Bold,
        )
        Spacer(modifier = Modifier.height(24.dp))

        when {
            !isAuthenticated -> {
                Text(
                    text = stringResource(R.string.profile_not_logged_in),
                    color = MaterialTheme.colorScheme.onSurfaceVariant,
                )
                Spacer(modifier = Modifier.height(16.dp))
                Button(onClick = onNavigateToLogin) {
                    Text(stringResource(R.string.nav_login))
                }
            }
            uiState.isLoading && uiState.profile == null && uiState.currentUser == null -> {
                CircularProgressIndicator()
            }
            else -> {
                val profile = uiState.profile
                val user = uiState.currentUser

                val pictureUrl = profile?.profilePictureUrl ?: user?.profilePictureUrl
                if (!pictureUrl.isNullOrBlank()) {
                    AsyncImage(
                        model = pictureUrl,
                        contentDescription = null,
                        modifier = Modifier.size(96.dp),
                        contentScale = ContentScale.Crop,
                    )
                    Spacer(modifier = Modifier.height(16.dp))
                }

                Text(
                    text = profile?.displayName ?: user?.displayName.orEmpty(),
                    style = MaterialTheme.typography.titleLarge,
                    fontWeight = FontWeight.SemiBold,
                )
                Spacer(modifier = Modifier.height(24.dp))

                ProfileField(
                    label = stringResource(R.string.profile_email),
                    value = profile?.email ?: user?.email.orEmpty(),
                )
                ProfileField(
                    label = stringResource(R.string.profile_phone),
                    value = profile?.phone ?: user?.phone.orEmpty(),
                )
                ProfileField(
                    label = stringResource(R.string.profile_role),
                    value = profile?.role ?: user?.role.orEmpty(),
                )
                ProfileField(
                    label = stringResource(R.string.profile_language),
                    value = profile?.preferredLanguage ?: user?.preferredLanguage.orEmpty(),
                )

                Spacer(modifier = Modifier.height(16.dp))
                OutlinedButton(
                    onClick = onNavigateToNotifications,
                    modifier = Modifier.fillMaxWidth(),
                ) {
                    Text(stringResource(R.string.notifications_title))
                }
                if (showSubscriptions) {
                    Spacer(modifier = Modifier.height(8.dp))
                    OutlinedButton(
                        onClick = onNavigateToSubscriptions,
                        modifier = Modifier.fillMaxWidth(),
                    ) {
                        Text(stringResource(R.string.subscription_nav))
                    }
                    Spacer(modifier = Modifier.height(8.dp))
                    OutlinedButton(
                        onClick = onNavigateToMySubscription,
                        modifier = Modifier.fillMaxWidth(),
                    ) {
                        Text(stringResource(R.string.subscription_my_subscription))
                    }
                }

                Spacer(modifier = Modifier.height(16.dp))
                OutlinedButton(
                    onClick = { authViewModel.logout(onNavigateToLogin) },
                    modifier = Modifier.fillMaxWidth(),
                ) {
                    Text(stringResource(R.string.nav_logout))
                }
            }
        }
    }
}

@Composable
private fun ProfileField(label: String, value: String) {
    Column(
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 8.dp),
    ) {
        Text(
            text = label,
            style = MaterialTheme.typography.labelMedium,
            color = MaterialTheme.colorScheme.onSurfaceVariant,
        )
        Text(
            text = value,
            style = MaterialTheme.typography.bodyLarge,
        )
    }
}
