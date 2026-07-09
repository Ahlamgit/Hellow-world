package com.khadamati.app.ui.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.khadamati.app.data.remote.dto.AddressDto
import com.khadamati.app.data.remote.dto.CreateAddressRequestDto
import com.khadamati.app.data.repository.AddressRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class AddressUiState(
    val addresses: List<AddressDto> = emptyList(),
    val isLoading: Boolean = false,
    val isSaving: Boolean = false,
    val error: String? = null,
)

class AddressViewModel(private val repository: AddressRepository) : ViewModel() {
    private val _uiState = MutableStateFlow(AddressUiState())
    val uiState: StateFlow<AddressUiState> = _uiState.asStateFlow()

    fun load() = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isLoading = true, error = null)
        try {
            _uiState.value = _uiState.value.copy(addresses = repository.list(), isLoading = false)
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    fun addAddress(
        label: String,
        street: String,
        city: String,
        country: String,
        latitude: Double?,
        longitude: Double?,
        onSuccess: () -> Unit,
    ) = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isSaving = true, error = null)
        try {
            repository.add(
                CreateAddressRequestDto(
                    label = label,
                    street = street,
                    city = city,
                    country = country,
                    latitude = latitude,
                    longitude = longitude,
                    isDefault = _uiState.value.addresses.isEmpty(),
                ),
            )
            load()
            _uiState.value = _uiState.value.copy(isSaving = false)
            onSuccess()
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isSaving = false)
        }
    }
}
