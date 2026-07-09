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

    fun pay(bookingId: String) = viewModelScope.launch { repository.pay(bookingId); loadBookings() }
    fun accept(bookingId: String) = viewModelScope.launch { repository.accept(bookingId); loadBookings() }
    fun reject(bookingId: String, reason: String) = viewModelScope.launch { repository.reject(bookingId, reason); loadBookings() }
    fun cancel(bookingId: String, reason: String) = viewModelScope.launch { repository.cancel(bookingId, reason); loadBookings() }
    fun complete(bookingId: String) = viewModelScope.launch { repository.complete(bookingId); loadBookings() }
    fun noShow(bookingId: String) = viewModelScope.launch { repository.noShow(bookingId); loadBookings() }
}
