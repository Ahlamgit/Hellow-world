package com.khadamati.app.ui.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.khadamati.app.data.remote.dto.CraftsmanProfileDto
import com.khadamati.app.data.remote.dto.ServiceDto
import com.khadamati.app.data.remote.dto.StoreProfileDto
import com.khadamati.app.data.remote.dto.UpdateCraftsmanProfileRequestDto
import com.khadamati.app.data.remote.dto.UpdateStoreProfileRequestDto
import com.khadamati.app.data.remote.dto.UpsertCraftsmanServiceRequestDto
import com.khadamati.app.data.remote.dto.UpsertStoreProductRequestDto
import com.khadamati.app.data.repository.PortalRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch

data class CraftsmanPortalUiState(
    val profile: CraftsmanProfileDto? = null,
    val services: List<ServiceDto> = emptyList(),
    val isLoading: Boolean = false,
    val message: String? = null,
    val errorMessage: String? = null,
)

data class StorePortalUiState(
    val profile: StoreProfileDto? = null,
    val isLoading: Boolean = false,
    val message: String? = null,
    val errorMessage: String? = null,
)

class CraftsmanPortalViewModel(
    private val repository: PortalRepository,
) : ViewModel() {
    private val _uiState = MutableStateFlow(CraftsmanPortalUiState())
    val uiState: StateFlow<CraftsmanPortalUiState> = _uiState.asStateFlow()

    fun load() {
        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, errorMessage = null, message = null) }
            try {
                val profile = repository.getCraftsmanProfile()
                val services = repository.getServices()
                _uiState.update { it.copy(profile = profile, services = services, isLoading = false) }
            } catch (e: Exception) {
                _uiState.update { it.copy(isLoading = false, errorMessage = e.message) }
            }
        }
    }

    fun saveProfile(
        specialization: String,
        yearsOfExperience: Int,
        isAvailable: Boolean,
        serviceRadiusKm: Double,
    ) {
        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, errorMessage = null, message = null) }
            try {
                val profile = repository.updateCraftsmanProfile(
                    UpdateCraftsmanProfileRequestDto(
                        specialization = specialization.ifBlank { null },
                        yearsOfExperience = yearsOfExperience,
                        isAvailable = isAvailable,
                        serviceRadiusKm = serviceRadiusKm,
                    ),
                )
                _uiState.update { it.copy(profile = profile, isLoading = false, message = "saved") }
            } catch (e: Exception) {
                _uiState.update { it.copy(isLoading = false, errorMessage = e.message) }
            }
        }
    }

    fun addService(serviceId: String, customPrice: Double) {
        if (serviceId.isBlank()) return
        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, errorMessage = null, message = null) }
            try {
                repository.upsertCraftsmanService(
                    UpsertCraftsmanServiceRequestDto(serviceId = serviceId, customPrice = customPrice),
                )
                val profile = repository.getCraftsmanProfile()
                _uiState.update { it.copy(profile = profile, isLoading = false, message = "service_added") }
            } catch (e: Exception) {
                _uiState.update { it.copy(isLoading = false, errorMessage = e.message) }
            }
        }
    }
}

class StorePortalViewModel(
    private val repository: PortalRepository,
) : ViewModel() {
    private val _uiState = MutableStateFlow(StorePortalUiState())
    val uiState: StateFlow<StorePortalUiState> = _uiState.asStateFlow()

    fun load() {
        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, errorMessage = null, message = null) }
            try {
                val profile = repository.getStoreProfile()
                _uiState.update { it.copy(profile = profile, isLoading = false) }
            } catch (e: Exception) {
                _uiState.update { it.copy(isLoading = false, errorMessage = e.message) }
            }
        }
    }

    fun saveProfile(
        storeName: String,
        description: String,
        isOpen: Boolean,
        openingTime: String,
        closingTime: String,
    ) {
        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, errorMessage = null, message = null) }
            try {
                val profile = repository.updateStoreProfile(
                    UpdateStoreProfileRequestDto(
                        storeName = storeName,
                        description = description.ifBlank { null },
                        isOpen = isOpen,
                        openingTime = openingTime.ifBlank { null },
                        closingTime = closingTime.ifBlank { null },
                    ),
                )
                _uiState.update { it.copy(profile = profile, isLoading = false, message = "saved") }
            } catch (e: Exception) {
                _uiState.update { it.copy(isLoading = false, errorMessage = e.message) }
            }
        }
    }

    fun addProduct(nameEn: String, nameAr: String, price: Double, stockQuantity: Int) {
        if (nameEn.isBlank()) return
        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, errorMessage = null, message = null) }
            try {
                repository.createStoreProduct(
                    UpsertStoreProductRequestDto(
                        nameEn = nameEn,
                        nameAr = nameAr.ifBlank { nameEn },
                        price = price,
                        stockQuantity = stockQuantity,
                    ),
                )
                val profile = repository.getStoreProfile()
                _uiState.update { it.copy(profile = profile, isLoading = false, message = "product_added") }
            } catch (e: Exception) {
                _uiState.update { it.copy(isLoading = false, errorMessage = e.message) }
            }
        }
    }
}

fun isCraftsmanRole(role: String?): Boolean = role == "Craftsman"

fun isStoreRole(role: String?): Boolean =
    role in setOf("Store", "StoreOwner", "StoreEmployee")
