package com.khadamati.app.data.repository

import com.khadamati.app.data.local.dao.UserDao
import com.khadamati.app.data.preferences.TokenManager
import com.khadamati.app.data.remote.ApiService
import com.khadamati.app.data.remote.dto.LoginRequestDto
import com.khadamati.app.data.repository.PushRepository
import com.khadamati.app.domain.model.AuthTokens
import com.khadamati.app.domain.model.User
import com.khadamati.app.domain.model.UserProfile
import com.khadamati.app.domain.model.toDomain
import com.khadamati.app.domain.model.toEntity
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map

class AuthRepository(
    private val apiService: ApiService,
    private val userDao: UserDao,
    private val tokenManager: TokenManager,
    private val pushRepository: PushRepository? = null,
) {

    val isLoggedIn: Flow<Boolean> = tokenManager.isLoggedIn

    fun observeCurrentUser(): Flow<User?> =
        userDao.observeCurrentUser().map { it?.toDomain() }

    suspend fun login(email: String, password: String): Result<User> = runCatching {
        val response = apiService.login(LoginRequestDto(email, password))
        val data = response.data ?: throw ApiException(response.message ?: "Login failed")
        val (tokens, user) = data.toDomain()
        persistSession(tokens, user)
        user
    }

    suspend fun register(
        email: String,
        phone: String,
        password: String,
        firstName: String,
        lastName: String,
        role: String,
        preferredLanguage: String,
    ): Result<User> = runCatching {
        val response = apiService.register(
            RegisterRequestDto(
                email = email,
                phone = phone,
                password = password,
                firstName = firstName,
                lastName = lastName,
                role = role,
                preferredLanguage = preferredLanguage,
            )
        )
        val data = response.data ?: throw ApiException(response.message ?: "Registration failed")
        val (tokens, user) = data.toDomain()
        persistSession(tokens, user)
        user
    }

    suspend fun getProfile(): Result<UserProfile> = runCatching {
        val response = apiService.getProfile()
        val data = response.data ?: throw ApiException(response.message ?: "Failed to load profile")
        data.toDomain()
    }

    suspend fun logout(): Result<Unit> = runCatching {
        val refreshToken = tokenManager.getRefreshToken()
        if (!refreshToken.isNullOrBlank()) {
            runCatching { apiService.revokeToken(refreshToken) }
        }
        clearSession()
    }

    suspend fun hasValidSession(): Boolean =
        !tokenManager.getAccessToken().isNullOrBlank()

    suspend fun forgotPassword(email: String): Result<String> = runCatching {
        val response = apiService.forgotPassword(com.khadamati.app.data.remote.dto.ForgotPasswordRequestDto(email))
        response.message ?: response.data?.message ?: "If an account exists, a reset link has been sent."
    }

    suspend fun resetPassword(token: String, newPassword: String, confirmPassword: String): Result<String> = runCatching {
        val response = apiService.resetPassword(
            com.khadamati.app.data.remote.dto.ResetPasswordRequestDto(token, newPassword, confirmPassword),
        )
        response.message ?: response.data?.message ?: "Password reset successfully."
    }

    suspend fun updateProfile(firstName: String, lastName: String, preferredLanguage: String): Result<UserProfile> = runCatching {
        val response = apiService.updateProfile(
            mapOf(
                "firstName" to firstName,
                "lastName" to lastName,
                "preferredLanguage" to preferredLanguage,
            ),
        )
        val data = response.data ?: throw ApiException(response.message ?: "Failed to update profile")
        data.toDomain()
    }

    private suspend fun persistSession(tokens: AuthTokens, user: User) {
        tokenManager.saveTokens(
            accessToken = tokens.accessToken,
            refreshToken = tokens.refreshToken,
            expiresAt = tokens.expiresAt,
        )
        userDao.upsert(user.toEntity())
        runCatching { pushRepository?.registerCurrentDevice() }
    }

    private suspend fun clearSession() {
        runCatching { pushRepository?.unregisterCurrentDevice() }
        tokenManager.clearTokens()
        userDao.clear()
    }
}

class ApiException(message: String) : Exception(message)
