package com.khadamati.app.ui.screens

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.Button
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Scaffold
import androidx.compose.material3.SnackbarHost
import androidx.compose.material3.SnackbarHostState
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import com.khadamati.app.R
import com.khadamati.app.ui.viewmodel.AuthViewModel

@Composable
fun ForgotPasswordScreen(
    authViewModel: AuthViewModel,
    onNavigateBack: () -> Unit,
    onNavigateToReset: () -> Unit,
) {
    val uiState by authViewModel.uiState.collectAsStateWithLifecycle()
    val snackbarHostState = remember { SnackbarHostState() }
    var email by remember { mutableStateOf("") }
    var successMessage by remember { mutableStateOf<String?>(null) }

    LaunchedEffect(uiState.errorMessage) {
        uiState.errorMessage?.let {
            snackbarHostState.showSnackbar(it)
            authViewModel.clearError()
        }
    }

    Scaffold(snackbarHost = { SnackbarHost(snackbarHostState) }) { padding ->
        Column(
            modifier = Modifier.fillMaxSize().padding(padding).padding(24.dp).verticalScroll(rememberScrollState()),
            horizontalAlignment = Alignment.CenterHorizontally,
            verticalArrangement = Arrangement.Center,
        ) {
            Text(stringResource(R.string.auth_forgot_password_title), style = MaterialTheme.typography.headlineMedium)
            Spacer(modifier = Modifier.height(12.dp))
            Text(stringResource(R.string.auth_forgot_password_hint))
            Spacer(modifier = Modifier.height(24.dp))
            OutlinedTextField(
                value = email,
                onValueChange = { email = it },
                label = { Text(stringResource(R.string.auth_email)) },
                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                modifier = Modifier.fillMaxWidth(),
                singleLine = true,
            )
            successMessage?.let {
                Spacer(modifier = Modifier.height(12.dp))
                Text(it, color = MaterialTheme.colorScheme.primary)
            }
            Spacer(modifier = Modifier.height(24.dp))
            Button(
                onClick = {
                    authViewModel.forgotPassword(email.trim()) { message ->
                        successMessage = message
                    }
                },
                enabled = !uiState.isLoading && email.isNotBlank(),
                modifier = Modifier.fillMaxWidth(),
            ) {
                if (uiState.isLoading) CircularProgressIndicator() else Text(stringResource(R.string.auth_send_reset_link))
            }
            TextButton(onClick = onNavigateToReset) { Text(stringResource(R.string.auth_reset_password_title)) }
            TextButton(onClick = onNavigateBack) { Text(stringResource(R.string.common_back)) }
        }
    }
}

@Composable
fun ResetPasswordScreen(
    authViewModel: AuthViewModel,
    initialToken: String = "",
    onNavigateBack: () -> Unit,
) {
    val uiState by authViewModel.uiState.collectAsStateWithLifecycle()
    val snackbarHostState = remember { SnackbarHostState() }
    var token by remember { mutableStateOf(initialToken) }
    var newPassword by remember { mutableStateOf("") }
    var confirmPassword by remember { mutableStateOf("") }
    var resetSuccess by remember { mutableStateOf(false) }

    LaunchedEffect(uiState.errorMessage) {
        uiState.errorMessage?.let {
            snackbarHostState.showSnackbar(it)
            authViewModel.clearError()
        }
    }

    Scaffold(snackbarHost = { SnackbarHost(snackbarHostState) }) { padding ->
        Column(
            modifier = Modifier.fillMaxSize().padding(padding).padding(24.dp).verticalScroll(rememberScrollState()),
            horizontalAlignment = Alignment.CenterHorizontally,
            verticalArrangement = Arrangement.Center,
        ) {
            Text(stringResource(R.string.auth_reset_password_title), style = MaterialTheme.typography.headlineMedium)
            Spacer(modifier = Modifier.height(24.dp))
            OutlinedTextField(
                value = token,
                onValueChange = { token = it },
                label = { Text(stringResource(R.string.auth_reset_token)) },
                modifier = Modifier.fillMaxWidth(),
            )
            Spacer(modifier = Modifier.height(12.dp))
            OutlinedTextField(
                value = newPassword,
                onValueChange = { newPassword = it },
                label = { Text(stringResource(R.string.auth_new_password)) },
                visualTransformation = PasswordVisualTransformation(),
                modifier = Modifier.fillMaxWidth(),
            )
            Spacer(modifier = Modifier.height(12.dp))
            OutlinedTextField(
                value = confirmPassword,
                onValueChange = { confirmPassword = it },
                label = { Text(stringResource(R.string.auth_confirm_password)) },
                visualTransformation = PasswordVisualTransformation(),
                modifier = Modifier.fillMaxWidth(),
            )
            if (resetSuccess) {
                Spacer(modifier = Modifier.height(12.dp))
                Text(stringResource(R.string.auth_reset_success), color = MaterialTheme.colorScheme.primary)
            }
            Spacer(modifier = Modifier.height(24.dp))
            Button(
                onClick = {
                    if (newPassword != confirmPassword) return@Button
                    authViewModel.resetPassword(token, newPassword, confirmPassword) {
                        resetSuccess = true
                    }
                },
                enabled = !uiState.isLoading && token.isNotBlank() && newPassword.length >= 6 && confirmPassword.isNotBlank(),
                modifier = Modifier.fillMaxWidth(),
            ) {
                if (uiState.isLoading) CircularProgressIndicator() else Text(stringResource(R.string.auth_reset_password))
            }
            if (newPassword.isNotBlank() && confirmPassword.isNotBlank() && newPassword != confirmPassword) {
                Text(stringResource(R.string.auth_password_mismatch), color = MaterialTheme.colorScheme.error)
            }
            TextButton(onClick = onNavigateBack) { Text(stringResource(R.string.common_back)) }
        }
    }
}

@Composable
fun ChangePasswordScreen(
    authViewModel: AuthViewModel,
    onNavigateBack: () -> Unit,
) {
    val uiState by authViewModel.uiState.collectAsStateWithLifecycle()
    val snackbarHostState = remember { SnackbarHostState() }
    var currentPassword by remember { mutableStateOf("") }
    var newPassword by remember { mutableStateOf("") }
    var confirmPassword by remember { mutableStateOf("") }
    var successMessage by remember { mutableStateOf<String?>(null) }

    LaunchedEffect(uiState.errorMessage) {
        uiState.errorMessage?.let {
            snackbarHostState.showSnackbar(it)
            authViewModel.clearError()
        }
    }

    Scaffold(snackbarHost = { SnackbarHost(snackbarHostState) }) { padding ->
        Column(
            modifier = Modifier.fillMaxSize().padding(padding).padding(24.dp).verticalScroll(rememberScrollState()),
            horizontalAlignment = Alignment.CenterHorizontally,
            verticalArrangement = Arrangement.Center,
        ) {
            Text(stringResource(R.string.auth_change_password_title), style = MaterialTheme.typography.headlineMedium)
            Spacer(modifier = Modifier.height(12.dp))
            Text(stringResource(R.string.auth_change_password_hint))
            Spacer(modifier = Modifier.height(24.dp))
            OutlinedTextField(
                value = currentPassword,
                onValueChange = { currentPassword = it },
                label = { Text(stringResource(R.string.auth_current_password)) },
                visualTransformation = PasswordVisualTransformation(),
                modifier = Modifier.fillMaxWidth(),
            )
            Spacer(modifier = Modifier.height(12.dp))
            OutlinedTextField(
                value = newPassword,
                onValueChange = { newPassword = it },
                label = { Text(stringResource(R.string.auth_new_password)) },
                visualTransformation = PasswordVisualTransformation(),
                modifier = Modifier.fillMaxWidth(),
            )
            Spacer(modifier = Modifier.height(12.dp))
            OutlinedTextField(
                value = confirmPassword,
                onValueChange = { confirmPassword = it },
                label = { Text(stringResource(R.string.auth_confirm_password)) },
                visualTransformation = PasswordVisualTransformation(),
                modifier = Modifier.fillMaxWidth(),
            )
            successMessage?.let {
                Spacer(modifier = Modifier.height(12.dp))
                Text(it, color = MaterialTheme.colorScheme.primary)
            }
            Spacer(modifier = Modifier.height(24.dp))
            Button(
                onClick = {
                    if (newPassword != confirmPassword) return@Button
                    authViewModel.changePassword(currentPassword, newPassword, confirmPassword) { message ->
                        successMessage = message
                        currentPassword = ""
                        newPassword = ""
                        confirmPassword = ""
                    }
                },
                enabled = !uiState.isLoading && currentPassword.isNotBlank() && newPassword.length >= 8 && confirmPassword.isNotBlank(),
                modifier = Modifier.fillMaxWidth(),
            ) {
                if (uiState.isLoading) CircularProgressIndicator() else Text(stringResource(R.string.auth_change_password))
            }
            if (newPassword.isNotBlank() && confirmPassword.isNotBlank() && newPassword != confirmPassword) {
                Text(stringResource(R.string.auth_password_mismatch), color = MaterialTheme.colorScheme.error)
            }
            TextButton(onClick = onNavigateBack) { Text(stringResource(R.string.common_back)) }
        }
    }
}
