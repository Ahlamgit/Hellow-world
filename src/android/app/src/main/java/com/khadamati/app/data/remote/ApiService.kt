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
}
