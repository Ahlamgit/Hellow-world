package com.khadamati.app.ui.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.khadamati.app.data.remote.dto.ComplaintDto
import com.khadamati.app.data.remote.dto.SupportTicketDto
import com.khadamati.app.data.repository.SupportRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class SupportUiState(
    val complaints: List<ComplaintDto> = emptyList(),
    val tickets: List<SupportTicketDto> = emptyList(),
    val isLoading: Boolean = false,
    val message: String? = null,
    val error: String? = null,
)

class SupportViewModel(private val repository: SupportRepository) : ViewModel() {
    private val _uiState = MutableStateFlow(SupportUiState())
    val uiState: StateFlow<SupportUiState> = _uiState.asStateFlow()

    fun load() = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isLoading = true, error = null)
        try {
            _uiState.value = _uiState.value.copy(
                complaints = repository.getComplaints(),
                tickets = repository.getTickets(),
                isLoading = false,
            )
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(isLoading = false, error = e.message)
        }
    }

    fun submitComplaint(subject: String, description: String) = viewModelScope.launch {
        try {
            repository.createComplaint(subject, description)
            _uiState.value = _uiState.value.copy(message = "complaint")
            load()
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message)
        }
    }

    fun submitTicket(subject: String, description: String, category: String) = viewModelScope.launch {
        try {
            repository.createTicket(subject, description, category)
            _uiState.value = _uiState.value.copy(message = "ticket")
            load()
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message)
        }
    }

    fun clearMessage() {
        _uiState.value = _uiState.value.copy(message = null, error = null)
    }
}
