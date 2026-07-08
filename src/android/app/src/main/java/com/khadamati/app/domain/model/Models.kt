package com.khadamati.app.domain.model

data class User(
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
) {
    val displayName: String
        get() = "$firstName $lastName".trim()
}

data class AuthTokens(
    val accessToken: String,
    val refreshToken: String,
    val expiresAt: String,
)

data class ServiceCategory(
    val id: String,
    val nameAr: String,
    val nameEn: String,
    val descriptionAr: String?,
    val descriptionEn: String?,
    val iconUrl: String?,
    val displayOrder: Int,
) {
    fun localizedName(isArabic: Boolean): String =
        if (isArabic) nameAr else nameEn

    fun localizedDescription(isArabic: Boolean): String? =
        if (isArabic) descriptionAr else descriptionEn
}

data class Service(
    val id: String,
    val categoryId: String,
    val nameAr: String,
    val nameEn: String,
    val descriptionAr: String?,
    val descriptionEn: String?,
    val basePrice: Double,
    val imageUrl: String?,
    val estimatedDurationMinutes: Int,
) {
    fun localizedName(isArabic: Boolean): String =
        if (isArabic) nameAr else nameEn

    fun localizedDescription(isArabic: Boolean): String? =
        if (isArabic) descriptionAr else descriptionEn
}

data class UserProfile(
    val id: String,
    val email: String,
    val phone: String,
    val role: String,
    val firstName: String,
    val lastName: String,
    val bio: String?,
    val profilePictureUrl: String?,
    val preferredLanguage: String,
) {
    val displayName: String
        get() = "$firstName $lastName".trim()
}
