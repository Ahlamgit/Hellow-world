package com.khadamati.app.ui.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.khadamati.app.data.repository.ServicesRepository
import com.khadamati.app.domain.model.Service
import com.khadamati.app.domain.model.ServiceCategory
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.combine
import kotlinx.coroutines.flow.flatMapLatest
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.launch

data class ServicesUiState(
    val isLoading: Boolean = false,
    val errorMessage: String? = null,
    val categories: List<ServiceCategory> = emptyList(),
    val services: List<Service> = emptyList(),
    val selectedCategoryId: String? = null,
)

@OptIn(ExperimentalCoroutinesApi::class)
class ServicesViewModel(
    private val servicesRepository: ServicesRepository,
) : ViewModel() {

    private val selectedCategoryId = MutableStateFlow<String?>(null)
    private val isLoading = MutableStateFlow(false)
    private val errorMessage = MutableStateFlow<String?>(null)

    val uiState: StateFlow<ServicesUiState> = combine(
        servicesRepository.observeCategories(),
        selectedCategoryId.flatMapLatest { categoryId ->
            servicesRepository.observeServices(categoryId)
        },
        selectedCategoryId,
        isLoading,
        errorMessage,
    ) { categories, services, selectedId, loading, error ->
        ServicesUiState(
            isLoading = loading,
            errorMessage = error,
            categories = categories,
            services = services,
            selectedCategoryId = selectedId,
        )
    }.stateIn(
        scope = viewModelScope,
        started = SharingStarted.WhileSubscribed(5_000),
        initialValue = ServicesUiState(),
    )

    init {
        refresh()
    }

    fun selectCategory(categoryId: String?) {
        selectedCategoryId.value = categoryId
        refreshServices(categoryId)
    }

    fun refresh() {
        viewModelScope.launch {
            isLoading.value = true
            errorMessage.value = null
            val categoriesResult = servicesRepository.refreshCategories()
            val servicesResult = servicesRepository.refreshServices(selectedCategoryId.value)

            if (categoriesResult.isFailure && servicesResult.isFailure) {
                errorMessage.value = categoriesResult.exceptionOrNull()?.message
                    ?: servicesResult.exceptionOrNull()?.message
            }
            isLoading.value = false
        }
    }

    private fun refreshServices(categoryId: String?) {
        viewModelScope.launch {
            isLoading.value = true
            errorMessage.value = null
            servicesRepository.refreshServices(categoryId)
                .onFailure { errorMessage.value = it.message }
            isLoading.value = false
        }
    }

    fun clearError() {
        errorMessage.value = null
    }
}
