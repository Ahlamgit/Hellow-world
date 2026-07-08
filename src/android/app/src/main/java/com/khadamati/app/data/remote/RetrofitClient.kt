package com.khadamati.app.data.remote

import com.khadamati.app.BuildConfig
import com.khadamati.app.data.preferences.TokenManager
import com.khadamati.app.data.remote.dto.RefreshTokenRequestDto
import com.khadamati.app.data.remote.interceptor.AuthInterceptor
import kotlinx.coroutines.runBlocking
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.util.concurrent.TimeUnit

class RetrofitClient(
  private val tokenManager: TokenManager,
) {
    private val authInterceptor = AuthInterceptor(tokenManager)

    private val refreshAuthenticator = okhttp3.Authenticator { _, response ->
        if (responseCount(response) >= 2) return@Authenticator null

        val accessToken = runBlocking { tokenManager.getAccessToken() } ?: return@Authenticator null
        val refreshToken = runBlocking { tokenManager.getRefreshToken() } ?: return@Authenticator null

        val refreshResponse = runBlocking {
            try {
                refreshApi.refreshToken(
                    RefreshTokenRequestDto(
                        accessToken = accessToken,
                        refreshToken = refreshToken,
                    )
                )
            } catch (_: Exception) {
                null
            }
        }

        val data = refreshResponse?.data ?: run {
            runBlocking { tokenManager.clearTokens() }
            return@Authenticator null
        }

        runBlocking {
            tokenManager.saveTokens(
                accessToken = data.accessToken,
                refreshToken = data.refreshToken,
                expiresAt = data.expiresAt,
            )
        }

        response.request.newBuilder()
            .header("Authorization", "Bearer ${data.accessToken}")
            .build()
    }

    private val loggingInterceptor = HttpLoggingInterceptor().apply {
        level = if (BuildConfig.DEBUG) {
            HttpLoggingInterceptor.Level.BODY
        } else {
            HttpLoggingInterceptor.Level.NONE
        }
    }

    private val okHttpClient: OkHttpClient = OkHttpClient.Builder()
        .connectTimeout(30, TimeUnit.SECONDS)
        .readTimeout(30, TimeUnit.SECONDS)
        .writeTimeout(30, TimeUnit.SECONDS)
        .addInterceptor(authInterceptor)
        .addInterceptor(loggingInterceptor)
        .authenticator(refreshAuthenticator)
        .build()

    private val retrofit: Retrofit = Retrofit.Builder()
        .baseUrl(BuildConfig.API_BASE_URL)
        .client(okHttpClient)
        .addConverterFactory(GsonConverterFactory.create())
        .build()

    val apiService: ApiService = retrofit.create(ApiService::class.java)

    private val refreshRetrofit: Retrofit = Retrofit.Builder()
        .baseUrl(BuildConfig.API_BASE_URL)
        .client(
            OkHttpClient.Builder()
                .connectTimeout(30, TimeUnit.SECONDS)
                .readTimeout(30, TimeUnit.SECONDS)
                .addInterceptor(loggingInterceptor)
                .build()
        )
        .addConverterFactory(GsonConverterFactory.create())
        .build()

    private val refreshApi: ApiService = refreshRetrofit.create(ApiService::class.java)

    private fun responseCount(response: okhttp3.Response): Int {
        var count = 1
        var prior = response.priorResponse
        while (prior != null) {
            count++
            prior = prior.priorResponse
        }
        return count
    }
}
