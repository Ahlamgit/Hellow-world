package com.khadamati.app.data.repository

import com.khadamati.app.data.remote.ApiService
import com.khadamati.app.data.remote.dto.BookingDto
import com.khadamati.app.data.remote.dto.BookingPaymentDto
import com.khadamati.app.data.remote.dto.CraftsmanOptionDto
import com.khadamati.app.data.remote.dto.TimeSlotDto

class BookingRepository(private val apiService: ApiService) {

    suspend fun getCraftsmen(serviceId: String): List<CraftsmanOptionDto> =
        apiService.getCraftsmen(serviceId).data.orEmpty()

    suspend fun getNearbyCraftsmen(serviceId: String, latitude: Double, longitude: Double): List<CraftsmanOptionDto> =
        apiService.getNearbyCraftsmen(serviceId, latitude, longitude).data.orEmpty()

    suspend fun getAvailability(craftsmanId: String, serviceId: String, date: String): List<TimeSlotDto> =
        apiService.getAvailability(craftsmanId, serviceId, date).data.orEmpty()

    suspend fun createAndConfirm(serviceId: String, craftsmanId: String, scheduledAt: String): BookingDto {
        val created = apiService.createBooking(
            com.khadamati.app.data.remote.dto.CreateBookingRequestDto(serviceId, craftsmanId, scheduledAt)
        ).data ?: error("Failed to create booking")
        return apiService.confirmBooking(created.id).data ?: created
    }

    suspend fun getBookings(): List<BookingDto> =
        apiService.getBookings().data?.items.orEmpty()

    suspend fun getBooking(id: String): BookingDto =
        apiService.getBooking(id).data ?: error("Booking not found")

    suspend fun pay(id: String): BookingPaymentDto {
        val payment = apiService.initiatePayment(id, com.khadamati.app.data.remote.dto.InitiatePaymentRequestDto("Card")).data
            ?: error("Payment initiation failed")
        val checkoutUrl = payment.checkoutUrl
        if (!checkoutUrl.isNullOrBlank()) {
            // Hosted checkout must complete on the gateway. Completion is webhook/server-side only.
            return payment
        }
        // Development fallback: no hosted URL — caller may refresh booking status after fake checkout.
        return payment
    }

    suspend fun accept(id: String) { apiService.acceptBooking(id) }
    suspend fun reject(id: String, reason: String) { apiService.rejectBooking(id, com.khadamati.app.data.remote.dto.RejectBookingRequestDto(reason)) }
    suspend fun cancel(id: String, reason: String) { apiService.cancelBooking(id, com.khadamati.app.data.remote.dto.CancelBookingRequestDto(reason)) }
    suspend fun complete(id: String) { apiService.completeBooking(id) }
    suspend fun reschedule(id: String, newScheduledAt: String, reason: String?) {
        apiService.rescheduleBooking(id, com.khadamati.app.data.remote.dto.RescheduleBookingRequestDto(newScheduledAt, reason))
    }
    suspend fun noShow(id: String) { apiService.noShowBooking(id) }
    suspend fun submitReview(id: String, rating: Int, review: String?) {
        apiService.submitBookingReview(id, com.khadamati.app.data.remote.dto.SubmitReviewRequestDto(rating, review))
    }
}
