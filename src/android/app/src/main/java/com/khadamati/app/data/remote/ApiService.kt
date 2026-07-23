package com.khadamati.app.data.remote

import com.khadamati.app.data.remote.dto.ApiResponse
import com.khadamati.app.data.remote.dto.BookingDto
import com.khadamati.app.data.remote.dto.BookingPaymentDto
import com.khadamati.app.data.remote.dto.ConfirmPaymentRequestDto
import com.khadamati.app.data.remote.dto.CraftsmanOptionDto
import com.khadamati.app.data.remote.dto.CreateBookingRequestDto
import com.khadamati.app.data.remote.dto.InitiatePaymentRequestDto
import com.khadamati.app.data.remote.dto.PagedResultDto
import com.khadamati.app.data.remote.dto.RejectBookingRequestDto
import com.khadamati.app.data.remote.dto.TimeSlotDto
import com.khadamati.app.data.remote.dto.AuthResponseDto
import com.khadamati.app.data.remote.dto.LoginRequestDto
import com.khadamati.app.data.remote.dto.RefreshTokenRequestDto
import com.khadamati.app.data.remote.dto.RegisterRequestDto
import com.khadamati.app.data.remote.dto.ServiceCategoryDto
import com.khadamati.app.data.remote.dto.ServiceDto
import com.khadamati.app.data.remote.dto.UserProfileDto
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.PATCH
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

    @POST("auth/forgot-password")
    suspend fun forgotPassword(@Body request: ForgotPasswordRequestDto): ApiResponse<MessageResponseDto>

    @POST("auth/reset-password")
    suspend fun resetPassword(@Body request: ResetPasswordRequestDto): ApiResponse<MessageResponseDto>

    @POST("auth/change-password")
    suspend fun changePassword(@Body request: ChangePasswordRequestDto): ApiResponse<MessageResponseDto>

    @GET("users/me")
    suspend fun getProfile(): ApiResponse<UserProfileDto>

    @PUT("users/me")
    suspend fun updateProfile(@Body request: Map<String, String>): ApiResponse<UserProfileDto>

    @GET("services/categories")
    suspend fun getServiceCategories(): ApiResponse<List<ServiceCategoryDto>>

    @GET("services")
    suspend fun getServices(@Query("categoryId") categoryId: String? = null): ApiResponse<List<ServiceDto>>

    @GET("bookings/craftsmen")
    suspend fun getCraftsmen(@Query("serviceId") serviceId: String): ApiResponse<List<CraftsmanOptionDto>>

    @GET("bookings/availability")
    suspend fun getAvailability(
        @Query("craftsmanId") craftsmanId: String,
        @Query("serviceId") serviceId: String,
        @Query("date") date: String,
    ): ApiResponse<List<TimeSlotDto>>

    @POST("bookings")
    suspend fun createBooking(@Body request: CreateBookingRequestDto): ApiResponse<BookingDto>

    @GET("bookings")
    suspend fun getBookings(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
    ): ApiResponse<PagedResultDto<BookingDto>>

    @GET("bookings/{id}")
    suspend fun getBooking(@retrofit2.http.Path("id") id: String): ApiResponse<BookingDto>

    @POST("bookings/{id}/confirm")
    suspend fun confirmBooking(@retrofit2.http.Path("id") id: String): ApiResponse<BookingDto>

    @POST("bookings/{id}/payment")
    suspend fun initiatePayment(
        @retrofit2.http.Path("id") id: String,
        @Body request: InitiatePaymentRequestDto,
    ): ApiResponse<BookingPaymentDto>

    @POST("bookings/{id}/payment/confirm")
    suspend fun confirmPayment(
        @retrofit2.http.Path("id") id: String,
        @Body request: ConfirmPaymentRequestDto,
    ): ApiResponse<BookingDto>

    @POST("bookings/{id}/accept")
    suspend fun acceptBooking(@retrofit2.http.Path("id") id: String): ApiResponse<BookingDto>

    @POST("bookings/{id}/reject")
    suspend fun rejectBooking(
        @retrofit2.http.Path("id") id: String,
        @Body request: RejectBookingRequestDto,
    ): ApiResponse<BookingDto>

    @POST("bookings/{id}/cancel")
    suspend fun cancelBooking(
        @retrofit2.http.Path("id") id: String,
        @Body request: com.khadamati.app.data.remote.dto.CancelBookingRequestDto,
    ): ApiResponse<BookingDto>

    @POST("bookings/{id}/complete")
    suspend fun completeBooking(@retrofit2.http.Path("id") id: String): ApiResponse<BookingDto>

    @POST("bookings/{id}/reschedule")
    suspend fun rescheduleBooking(
        @retrofit2.http.Path("id") id: String,
        @Body request: com.khadamati.app.data.remote.dto.RescheduleBookingRequestDto,
    ): ApiResponse<BookingDto>

    @POST("bookings/{id}/no-show")
    suspend fun noShowBooking(@retrofit2.http.Path("id") id: String): ApiResponse<BookingDto>

    @GET("notifications")
    suspend fun getNotifications(
        @Query("unreadOnly") unreadOnly: Boolean = false,
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
    ): ApiResponse<PagedResultDto<com.khadamati.app.data.remote.dto.NotificationDto>>

    @POST("notifications/{id}/read")
    suspend fun markNotificationRead(
        @retrofit2.http.Path("id") id: String,
    ): ApiResponse<Any?>

    @GET("subscription-plans")
    suspend fun getSubscriptionPlans(
        @Query("targetRole") targetRole: String? = null,
    ): ApiResponse<List<com.khadamati.app.data.remote.dto.SubscriptionPlanDto>>

    @GET("subscription-plans/{id}")
    suspend fun getSubscriptionPlan(
        @retrofit2.http.Path("id") id: String,
    ): ApiResponse<com.khadamati.app.data.remote.dto.SubscriptionPlanDto>

    @GET("me/subscription")
    suspend fun getCurrentSubscription(): ApiResponse<com.khadamati.app.data.remote.dto.UserSubscriptionDto?>

    @GET("me/subscriptions")
    suspend fun getSubscriptionHistory(
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 20,
    ): ApiResponse<PagedResultDto<com.khadamati.app.data.remote.dto.UserSubscriptionDto>>

    @POST("me/subscription")
    suspend fun subscribe(
        @Body request: com.khadamati.app.data.remote.dto.SubscribeRequestDto,
    ): ApiResponse<com.khadamati.app.data.remote.dto.UserSubscriptionDto>

    @POST("me/subscription/{id}/cancel")
    suspend fun cancelSubscription(
        @retrofit2.http.Path("id") id: String,
        @Body request: com.khadamati.app.data.remote.dto.CancelSubscriptionRequestDto,
    ): ApiResponse<Any?>

    @PATCH("me/subscription/auto-renew")
    suspend fun updateAutoRenew(
        @Body request: com.khadamati.app.data.remote.dto.UpdateAutoRenewRequestDto,
    ): ApiResponse<com.khadamati.app.data.remote.dto.UserSubscriptionDto>

    @POST("devices/push-token")
    suspend fun registerPushToken(
        @Body request: com.khadamati.app.data.remote.dto.RegisterPushTokenRequestDto,
    ): ApiResponse<Any?>

    @retrofit2.http.DELETE("devices/push-token")
    suspend fun unregisterPushToken(
        @Query("token") token: String,
    ): ApiResponse<Any?>

    @GET("users/me/addresses")
    suspend fun getAddresses(): ApiResponse<List<com.khadamati.app.data.remote.dto.AddressDto>>

    @POST("users/me/addresses")
    suspend fun addAddress(
        @Body request: com.khadamati.app.data.remote.dto.CreateAddressRequestDto,
    ): ApiResponse<com.khadamati.app.data.remote.dto.AddressDto>

    @GET("bookings/craftsmen/nearby")
    suspend fun getNearbyCraftsmen(
        @Query("serviceId") serviceId: String,
        @Query("latitude") latitude: Double,
        @Query("longitude") longitude: Double,
        @Query("radiusKm") radiusKm: Double = 25.0,
    ): ApiResponse<List<com.khadamati.app.data.remote.dto.CraftsmanOptionDto>>

    @GET("chat/conversations")
    suspend fun getChatConversations(): ApiResponse<List<com.khadamati.app.data.remote.dto.ChatConversationDto>>

    @GET("chat/bookings/{bookingId}")
    suspend fun getBookingChat(
        @retrofit2.http.Path("bookingId") bookingId: String,
    ): ApiResponse<com.khadamati.app.data.remote.dto.ChatConversationDto>

    @GET("chat/conversations/{id}/messages")
    suspend fun getChatMessages(
        @retrofit2.http.Path("id") conversationId: String,
        @Query("page") page: Int = 1,
        @Query("pageSize") pageSize: Int = 50,
    ): ApiResponse<PagedResultDto<com.khadamati.app.data.remote.dto.ChatMessageDto>>

    @POST("chat/conversations/{id}/messages")
    suspend fun sendChatMessage(
        @retrofit2.http.Path("id") conversationId: String,
        @Body request: com.khadamati.app.data.remote.dto.SendChatMessageRequestDto,
    ): ApiResponse<com.khadamati.app.data.remote.dto.ChatMessageDto>

    @POST("complaints")
    suspend fun createComplaint(
        @Body request: com.khadamati.app.data.remote.dto.CreateComplaintRequestDto,
    ): ApiResponse<com.khadamati.app.data.remote.dto.ComplaintDto>

    @GET("complaints/mine")
    suspend fun getMyComplaints(): ApiResponse<List<com.khadamati.app.data.remote.dto.ComplaintDto>>

    @POST("support-tickets")
    suspend fun createSupportTicket(
        @Body request: com.khadamati.app.data.remote.dto.CreateSupportTicketRequestDto,
    ): ApiResponse<com.khadamati.app.data.remote.dto.SupportTicketDto>

    @GET("support-tickets/mine")
    suspend fun getMySupportTickets(): ApiResponse<List<com.khadamati.app.data.remote.dto.SupportTicketDto>>

    @POST("bookings/{id}/review")
    suspend fun submitBookingReview(
        @retrofit2.http.Path("id") id: String,
        @Body request: com.khadamati.app.data.remote.dto.SubmitReviewRequestDto,
    ): ApiResponse<BookingDto>

    @GET("me/craftsman")
    suspend fun getCraftsmanProfile(): ApiResponse<com.khadamati.app.data.remote.dto.CraftsmanProfileDto>

    @PUT("me/craftsman")
    suspend fun updateCraftsmanProfile(
        @Body request: com.khadamati.app.data.remote.dto.UpdateCraftsmanProfileRequestDto,
    ): ApiResponse<com.khadamati.app.data.remote.dto.CraftsmanProfileDto>

    @POST("me/craftsman/services")
    suspend fun upsertCraftsmanService(
        @Body request: com.khadamati.app.data.remote.dto.UpsertCraftsmanServiceRequestDto,
    ): ApiResponse<com.khadamati.app.data.remote.dto.CraftsmanServiceItemDto>

    @GET("me/store")
    suspend fun getStoreProfile(): ApiResponse<com.khadamati.app.data.remote.dto.StoreProfileDto>

    @PUT("me/store")
    suspend fun updateStoreProfile(
        @Body request: com.khadamati.app.data.remote.dto.UpdateStoreProfileRequestDto,
    ): ApiResponse<com.khadamati.app.data.remote.dto.StoreProfileDto>

    @POST("me/store/products")
    suspend fun createStoreProduct(
        @Body request: com.khadamati.app.data.remote.dto.UpsertStoreProductRequestDto,
    ): ApiResponse<com.khadamati.app.data.remote.dto.StoreProductDto>
}
