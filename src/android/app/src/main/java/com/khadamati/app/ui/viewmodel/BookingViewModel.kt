package com.khadamati.app.ui.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.khadamati.app.data.remote.dto.BookingDto
import com.khadamati.app.data.remote.dto.CraftsmanOptionDto
import com.khadamati.app.data.remote.dto.TimeSlotDto
import com.khadamati.app.data.repository.BookingRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class BookingUiState(
    val bookings: List<BookingDto> = emptyList(),
    val craftsmen: List<CraftsmanOptionDto> = emptyList(),
    val slots: List<TimeSlotDto> = emptyList(),
    val selectedBooking: BookingDto? = null,
    val isLoading: Boolean = false,
    val error: String? = null,
)

class BookingViewModel(private val repository: BookingRepository) : ViewModel() {
    private val _uiState = MutableStateFlow(BookingUiState())
    val uiState: StateFlow<BookingUiState> = _uiState.asStateFlow()

    fun loadBooking(bookingId: String) = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isLoading = true, error = null)
        try {
            val booking = repository.getBooking(bookingId)
            _uiState.value = _uiState.value.copy(selectedBooking = booking, isLoading = false)
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    fun loadBookings() = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isLoading = true)
        try {
            _uiState.value = _uiState.value.copy(bookings = repository.getBookings(), isLoading = false)
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    fun loadCraftsmen(serviceId: String) = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isLoading = true, error = null)
        try {
            _uiState.value = _uiState.value.copy(
                craftsmen = repository.getCraftsmen(serviceId),
                isLoading = false,
            )
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    fun loadNearbyCraftsmen(serviceId: String, latitude: Double, longitude: Double) = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isLoading = true, error = null)
        try {
            _uiState.value = _uiState.value.copy(
                craftsmen = repository.getNearbyCraftsmen(serviceId, latitude, longitude),
                isLoading = false,
            )
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    fun loadSlots(craftsmanId: String, serviceId: String, date: String) = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isLoading = true, error = null)
        try {
            _uiState.value = _uiState.value.copy(
                slots = repository.getAvailability(craftsmanId, serviceId, date),
                isLoading = false,
            )
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    fun createBooking(serviceId: String, craftsmanId: String, scheduledAt: String, onSuccess: (String) -> Unit) =
        viewModelScope.launch {
            _uiState.value = _uiState.value.copy(isLoading = true, error = null)
            try {
                val booking = repository.createAndConfirm(serviceId, craftsmanId, scheduledAt)
                _uiState.value = _uiState.value.copy(isLoading = false)
                onSuccess(booking.id)
            } catch (e: Exception) {
                _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
            }
        }

    private fun refreshAfterAction(bookingId: String) = viewModelScope.launch {
        try {
            val booking = repository.getBooking(bookingId)
            val bookings = repository.getBookings()
            _uiState.value = _uiState.value.copy(selectedBooking = booking, bookings = bookings, error = null)
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message)
        }
    }

    fun pay(bookingId: String) = viewModelScope.launch { repository.pay(bookingId); refreshAfterAction(bookingId) }
    fun accept(bookingId: String) = viewModelScope.launch { repository.accept(bookingId); refreshAfterAction(bookingId) }
    fun reject(bookingId: String, reason: String) = viewModelScope.launch { repository.reject(bookingId, reason); refreshAfterAction(bookingId) }
    fun cancel(bookingId: String, reason: String) = viewModelScope.launch { repository.cancel(bookingId, reason); refreshAfterAction(bookingId) }
    fun complete(bookingId: String) = viewModelScope.launch { repository.complete(bookingId); refreshAfterAction(bookingId) }
    fun noShow(bookingId: String) = viewModelScope.launch { repository.noShow(bookingId); refreshAfterAction(bookingId) }
    fun reschedule(bookingId: String, newScheduledAt: String, reason: String?) =
        viewModelScope.launch { repository.reschedule(bookingId, newScheduledAt, reason); refreshAfterAction(bookingId) }
    fun submitReview(bookingId: String, rating: Int, review: String?) =
        viewModelScope.launch { repository.submitReview(bookingId, rating, review); refreshAfterAction(bookingId) }
}
