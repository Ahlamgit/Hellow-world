package com.khadamati.app.data.remote

import com.khadamati.app.data.remote.dto.ApiResponse
import com.khadamati.app.data.remote.dto.AuthResponseDto
import com.khadamati.app.data.remote.dto.LoginRequestDto
import com.khadamati.app.data.remote.dto.RefreshTokenRequestDto
import com.khadamati.app.data.remote.dto.RegisterRequestDto
import com.khadamati.app.data.remote.dto.ServiceCategoryDto
import com.khadamati.app.data.remote.dto.ServiceDto
import com.khadamati.app.data.remote.dto.UserProfileDto
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Query

interface ApiService {

    @POST("auth/login")
    suspend fun login(@Body request: LoginRequestDto): ApiResponse<AuthResponseDto>

    @POST("auth/register")
    suspend fun register(@Body request: RegisterRequestDto): ApiResponse<AuthResponseDto>

    @POST("auth/refresh")
    suspend fun refreshToken(@Body request: RefreshTokenRequestDto): ApiResponse<AuthResponseDto>

    @POST("auth/revoke")
    suspend fun revokeToken(@Body refreshToken: String): ApiResponse<Any?>

    @GET("users/me")
    suspend fun getProfile(): ApiResponse<UserProfileDto>

    @PUT("users/me")
    suspend fun updateProfile(@Body request: Map<String, String>): ApiResponse<UserProfileDto>

    @GET("services/categories")
    suspend fun getServiceCategories(): ApiResponse<List<ServiceCategoryDto>>

    @GET("services")
    suspend fun getServices(@Query("categoryId") categoryId: String? = null): ApiResponse<List<ServiceDto>>
}
