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

data class ForgotPasswordRequestDto(val email: String)

data class ResetPasswordRequestDto(
    val token: String,
    val newPassword: String,
    val confirmPassword: String,
)

data class MessageResponseDto(val message: String? = null)

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

data class PagedResultDto<T>(
    val items: List<T>,
    val totalCount: Int,
    val page: Int,
    val pageSize: Int,
)

data class CraftsmanOptionDto(
    val id: String,
    val firstName: String,
    val lastName: String,
    val specialization: String?,
    val rating: Double,
    val totalReviews: Int,
    val completedJobs: Int,
    val price: Double,
    val isAvailable: Boolean,
    val distanceKm: Double? = null,
)

data class TimeSlotDto(
    val start: String,
    val end: String,
    val isAvailable: Boolean,
)

data class CreateBookingRequestDto(
    val serviceId: String,
    val craftsmanId: String,
    val scheduledAt: String,
    val addressId: String? = null,
    val description: String? = null,
)

data class BookingDto(
    val id: String,
    val bookingReference: String,
    val serviceId: String,
    val serviceName: String,
    val customerId: String,
    val customerName: String,
    val craftsmanId: String,
    val craftsmanName: String,
    val status: String,
    val scheduledAt: String,
    val slotEnd: String,
    val estimatedPrice: Double,
    val payment: BookingPaymentDto?,
)

data class BookingPaymentDto(
    val id: String,
    val amount: Double,
    val currency: String,
    val status: String,
    val paymentMethod: String,
    val sessionId: String? = null,
    val checkoutUrl: String? = null,
)

data class CancelBookingRequestDto(val reason: String)
data class RescheduleBookingRequestDto(val newScheduledAt: String, val reason: String? = null)

data class ConfirmPaymentRequestDto(val transactionReference: String)
data class InitiatePaymentRequestDto(val paymentMethod: String)
data class RejectBookingRequestDto(val reason: String)

data class NotificationDto(
    val id: String,
    val titleEn: String,
    val titleAr: String,
    val messageEn: String,
    val messageAr: String,
    val notificationType: String,
    val referenceId: String? = null,
    val isRead: Boolean,
    val createdAt: String,
)

data class PlanBillingOptionDto(
    val id: String? = null,
    val cycle: String,
    val price: Double,
    val durationDays: Int,
    val isActive: Boolean,
)

data class SubscriptionPlanDto(
    val id: String,
    val planCode: String,
    val nameEn: String,
    val nameAr: String,
    val descriptionEn: String? = null,
    val descriptionAr: String? = null,
    val currency: String,
    val targetRole: String,
    val status: String,
    val isFeatured: Boolean,
    val maxServices: Int? = null,
    val verificationBadge: Boolean,
    val premiumBadge: Boolean,
    val trialDays: Int,
    val billingOptions: List<PlanBillingOptionDto>,
)

data class UserSubscriptionDto(
    val id: String,
    val userId: String,
    val userEmail: String,
    val userName: String,
    val planId: String,
    val planCode: String,
    val planNameEn: String,
    val planNameAr: String,
    val billingOptionId: String? = null,
    val billingCycle: String? = null,
    val status: String,
    val startDate: String,
    val endDate: String? = null,
    val autoRenew: Boolean,
    val amountPaid: Double? = null,
    val currency: String? = null,
    val couponCode: String? = null,
    val cancelledAt: String? = null,
    val cancellationReason: String? = null,
    val createdAt: String,
)

data class SubscribeRequestDto(
    val planId: String,
    val billingOptionId: String,
    val autoRenew: Boolean = true,
    val couponCode: String? = null,
)

data class CancelSubscriptionRequestDto(val reason: String? = null)
data class UpdateAutoRenewRequestDto(val autoRenew: Boolean)

data class AddressDto(
    val id: String,
    val label: String,
    val street: String,
    val city: String,
    val district: String? = null,
    val postalCode: String? = null,
    val country: String,
    val latitude: Double? = null,
    val longitude: Double? = null,
    val isDefault: Boolean,
)

data class CreateAddressRequestDto(
    val label: String,
    val street: String,
    val city: String,
    val district: String? = null,
    val postalCode: String? = null,
    val country: String,
    val latitude: Double? = null,
    val longitude: Double? = null,
    val isDefault: Boolean = false,
)

data class RegisterPushTokenRequestDto(
    val token: String,
    val platform: String,
    val deviceName: String? = null,
)

data class ChatConversationDto(
    val id: String,
    val bookingId: String,
    val bookingReference: String,
    val serviceName: String,
    val customerId: String,
    val customerName: String,
    val craftsmanId: String,
    val craftsmanName: String,
    val lastMessageAt: String? = null,
    val lastMessagePreview: String? = null,
    val unreadCount: Int = 0,
)

data class ChatMessageDto(
    val id: String,
    val conversationId: String,
    val senderId: String,
    val senderName: String,
    val body: String,
    val sentAt: String,
    val isRead: Boolean,
    val isMine: Boolean,
)

data class SendChatMessageRequestDto(val body: String)
