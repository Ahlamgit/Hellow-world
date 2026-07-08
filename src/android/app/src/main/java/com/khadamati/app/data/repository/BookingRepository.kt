package com.khadamati.app.data.repository

import com.khadamati.app.data.remote.ApiService
import com.khadamati.app.data.remote.dto.BookingDto
import com.khadamati.app.data.remote.dto.CraftsmanOptionDto
import com.khadamati.app.data.remote.dto.TimeSlotDto

class BookingRepository(private val apiService: ApiService) {

    suspend fun getCraftsmen(serviceId: String): List<CraftsmanOptionDto> =
        apiService.getCraftsmen(serviceId).data.orEmpty()

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

    suspend fun pay(id: String) {
        apiService.initiatePayment(id, com.khadamati.app.data.remote.dto.InitiatePaymentRequestDto("Card"))
        apiService.confirmPayment(id, com.khadamati.app.data.remote.dto.ConfirmPaymentRequestDto("TXN-${System.currentTimeMillis()}"))
    }

    suspend fun accept(id: String) { apiService.acceptBooking(id) }
    suspend fun reject(id: String, reason: String) { apiService.rejectBooking(id, com.khadamati.app.data.remote.dto.RejectBookingRequestDto(reason)) }
}
