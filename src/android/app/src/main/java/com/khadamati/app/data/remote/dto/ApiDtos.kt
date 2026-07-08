package com.khadamati.app.data.remote.dto

import com.google.gson.annotations.SerializedName

data class ApiResponse<T>(
    val success: Boolean,
    val message: String?,
    val data: T?,
    val errors: List<String>?,
)

data class LoginRequestDto(
    val email: String,
    val password: String,
)

data class RegisterRequestDto(
    val email: String,
    val phone: String,
    val password: String,
    val firstName: String,
    val lastName: String,
    val role: String,
    @SerializedName("preferredLanguage")
    val preferredLanguage: String = "ar",
)

data class RefreshTokenRequestDto(
    val accessToken: String,
    val refreshToken: String,
)

data class AuthResponseDto(
    val accessToken: String,
    val refreshToken: String,
    val expiresAt: String,
    val user: UserDto,
)

data class UserDto(
    val id: String,
    val email: String,
    val phone: String,
    val role: String,
    val status: String,
    val verificationStatus: String,
    val subscriptionStatus: String,
    val firstName: String,
    val lastName: String,
    val profilePictureUrl: String?,
    val preferredLanguage: String,
)

data class UserProfileDto(
    val id: String,
    val email: String,
    val phone: String,
    val role: String,
    val firstName: String,
    val lastName: String,
    val bio: String?,
    val profilePictureUrl: String?,
    val preferredLanguage: String,
)

data class ServiceCategoryDto(
    val id: String,
    val nameAr: String,
    val nameEn: String,
    val descriptionAr: String?,
    val descriptionEn: String?,
    val iconUrl: String?,
    val displayOrder: Int,
)

data class ServiceDto(
    val id: String,
    val categoryId: String,
    val nameAr: String,
    val nameEn: String,
    val descriptionAr: String?,
    val descriptionEn: String?,
    val basePrice: Double,
    val imageUrl: String?,
    val estimatedDurationMinutes: Int,
)
