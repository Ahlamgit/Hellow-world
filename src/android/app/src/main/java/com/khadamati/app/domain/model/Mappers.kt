package com.khadamati.app.domain.model

import com.khadamati.app.data.local.entity.ServiceCategoryEntity
import com.khadamati.app.data.local.entity.ServiceEntity
import com.khadamati.app.data.local.entity.UserEntity
import com.khadamati.app.data.remote.dto.AuthResponseDto
import com.khadamati.app.data.remote.dto.ServiceCategoryDto
import com.khadamati.app.data.remote.dto.ServiceDto
import com.khadamati.app.data.remote.dto.UserDto
import com.khadamati.app.data.remote.dto.UserProfileDto

fun UserDto.toDomain(): User = User(
    id = id,
    email = email,
    phone = phone,
    role = role,
    status = status,
    verificationStatus = verificationStatus,
    subscriptionStatus = subscriptionStatus,
    firstName = firstName,
    lastName = lastName,
    profilePictureUrl = profilePictureUrl,
    preferredLanguage = preferredLanguage,
)

fun AuthResponseDto.toDomain(): Pair<AuthTokens, User> {
    val tokens = AuthTokens(
        accessToken = accessToken,
        refreshToken = refreshToken,
        expiresAt = expiresAt,
    )
    return tokens to user.toDomain()
}

fun UserProfileDto.toDomain(): UserProfile = UserProfile(
    id = id,
    email = email,
    phone = phone,
    role = role,
    firstName = firstName,
    lastName = lastName,
    bio = bio,
    profilePictureUrl = profilePictureUrl,
    preferredLanguage = preferredLanguage,
)

fun ServiceCategoryDto.toDomain(): ServiceCategory = ServiceCategory(
    id = id,
    nameAr = nameAr,
    nameEn = nameEn,
    descriptionAr = descriptionAr,
    descriptionEn = descriptionEn,
    iconUrl = iconUrl,
    displayOrder = displayOrder,
)

fun ServiceDto.toDomain(): Service = Service(
    id = id,
    categoryId = categoryId,
    nameAr = nameAr,
    nameEn = nameEn,
    descriptionAr = descriptionAr,
    descriptionEn = descriptionEn,
    basePrice = basePrice,
    imageUrl = imageUrl,
    estimatedDurationMinutes = estimatedDurationMinutes,
)

fun User.toEntity(): UserEntity = UserEntity(
    id = id,
    email = email,
    phone = phone,
    role = role,
    status = status,
    verificationStatus = verificationStatus,
    subscriptionStatus = subscriptionStatus,
    firstName = firstName,
    lastName = lastName,
    profilePictureUrl = profilePictureUrl,
    preferredLanguage = preferredLanguage,
)

fun UserEntity.toDomain(): User = User(
    id = id,
    email = email,
    phone = phone,
    role = role,
    status = status,
    verificationStatus = verificationStatus,
    subscriptionStatus = subscriptionStatus,
    firstName = firstName,
    lastName = lastName,
    profilePictureUrl = profilePictureUrl,
    preferredLanguage = preferredLanguage,
)

fun ServiceCategory.toEntity(): ServiceCategoryEntity = ServiceCategoryEntity(
    id = id,
    nameAr = nameAr,
    nameEn = nameEn,
    descriptionAr = descriptionAr,
    descriptionEn = descriptionEn,
    iconUrl = iconUrl,
    displayOrder = displayOrder,
)

fun ServiceCategoryEntity.toDomain(): ServiceCategory = ServiceCategory(
    id = id,
    nameAr = nameAr,
    nameEn = nameEn,
    descriptionAr = descriptionAr,
    descriptionEn = descriptionEn,
    iconUrl = iconUrl,
    displayOrder = displayOrder,
)

fun Service.toEntity(): ServiceEntity = ServiceEntity(
    id = id,
    categoryId = categoryId,
    nameAr = nameAr,
    nameEn = nameEn,
    descriptionAr = descriptionAr,
    descriptionEn = descriptionEn,
    basePrice = basePrice,
    imageUrl = imageUrl,
    estimatedDurationMinutes = estimatedDurationMinutes,
)

fun ServiceEntity.toDomain(): Service = Service(
    id = id,
    categoryId = categoryId,
    nameAr = nameAr,
    nameEn = nameEn,
    descriptionAr = descriptionAr,
    descriptionEn = descriptionEn,
    basePrice = basePrice,
    imageUrl = imageUrl,
    estimatedDurationMinutes = estimatedDurationMinutes,
)
