package com.khadamati.app.ui.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.khadamati.app.data.remote.dto.SubscriptionPlanDto
import com.khadamati.app.data.remote.dto.UserSubscriptionDto
import com.khadamati.app.data.repository.SubscriptionRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class SubscriptionUiState(
    val plans: List<SubscriptionPlanDto> = emptyList(),
    val selectedPlan: SubscriptionPlanDto? = null,
    val current: UserSubscriptionDto? = null,
    val history: List<UserSubscriptionDto> = emptyList(),
    val selectedBillingOptionId: String? = null,
    val autoRenew: Boolean = true,
    val isLoading: Boolean = false,
    val isSubmitting: Boolean = false,
    val error: String? = null,
)

class SubscriptionViewModel(
    private val repository: SubscriptionRepository,
) : ViewModel() {

    private val _uiState = MutableStateFlow(SubscriptionUiState())
    val uiState: StateFlow<SubscriptionUiState> = _uiState.asStateFlow()

    fun loadPlans(targetRole: String?) = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isLoading = true, error = null)
        try {
            val current = runCatching { repository.getCurrent() }.getOrNull()
            val plans = repository.getPlans(targetRole)
            _uiState.value = _uiState.value.copy(
                plans = plans,
                current = current,
                isLoading = false,
            )
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    fun loadPlan(planId: String) = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isLoading = true, error = null)
        try {
            val plan = repository.getPlan(planId)
            val firstOption = plan.billingOptions.firstOrNull { it.isActive && it.id != null }
            _uiState.value = _uiState.value.copy(
                selectedPlan = plan,
                selectedBillingOptionId = firstOption?.id,
                isLoading = false,
            )
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    fun loadMySubscription() = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isLoading = true, error = null)
        try {
            val current = repository.getCurrent()
            val history = repository.getHistory().items
            _uiState.value = _uiState.value.copy(
                current = current,
                history = history,
                isLoading = false,
            )
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    fun selectBillingOption(id: String) {
        _uiState.value = _uiState.value.copy(selectedBillingOptionId = id)
    }

    fun setAutoRenew(enabled: Boolean) {
        _uiState.value = _uiState.value.copy(autoRenew = enabled)
    }

    fun subscribe(onSuccess: () -> Unit) = viewModelScope.launch {
        val plan = _uiState.value.selectedPlan ?: return@launch
        val billingOptionId = _uiState.value.selectedBillingOptionId ?: return@launch
        _uiState.value = _uiState.value.copy(isSubmitting = true, error = null)
        try {
            repository.subscribe(plan.id, billingOptionId, _uiState.value.autoRenew)
            _uiState.value = _uiState.value.copy(isSubmitting = false)
            onSuccess()
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isSubmitting = false)
        }
    }

    fun updateAutoRenew(enabled: Boolean) = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isSubmitting = true, error = null)
        try {
            val updated = repository.updateAutoRenew(enabled)
            _uiState.value = _uiState.value.copy(current = updated, isSubmitting = false)
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isSubmitting = false)
        }
    }

    fun cancel(reason: String?, onSuccess: () -> Unit) = viewModelScope.launch {
        val id = _uiState.value.current?.id ?: return@launch
        _uiState.value = _uiState.value.copy(isSubmitting = true, error = null)
        try {
            repository.cancel(id, reason)
            loadMySubscription()
            _uiState.value = _uiState.value.copy(isSubmitting = false)
            onSuccess()
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isSubmitting = false)
        }
    }
}

fun subscriptionTargetRole(userRole: String?): String? = when (userRole) {
    "Store", "StoreOwner" -> "Store"
    "Craftsman" -> "Craftsman"
    else -> null
}

fun isSubscriberRole(role: String?): Boolean =
    role in setOf("Craftsman", "Store", "StoreOwner")
