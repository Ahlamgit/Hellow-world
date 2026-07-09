package com.khadamati.app.ui.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.khadamati.app.data.remote.dto.NotificationDto
import com.khadamati.app.data.repository.NotificationRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

enum class NotificationFilter { ALL, UNREAD }

data class NotificationsUiState(
    val notifications: List<NotificationDto> = emptyList(),
    val totalCount: Int = 0,
    val page: Int = 1,
    val filter: NotificationFilter = NotificationFilter.ALL,
    val isLoading: Boolean = false,
    val error: String? = null,
)

class NotificationsViewModel(
    private val repository: NotificationRepository,
) : ViewModel() {

    private val _uiState = MutableStateFlow(NotificationsUiState())
    val uiState: StateFlow<NotificationsUiState> = _uiState.asStateFlow()

    private val pageSize = 15

    fun load(page: Int = _uiState.value.page) = viewModelScope.launch {
        val filter = _uiState.value.filter
        _uiState.value = _uiState.value.copy(isLoading = true, error = null, page = page)
        try {
            val result = repository.getNotifications(
                unreadOnly = filter == NotificationFilter.UNREAD,
                page = page,
                pageSize = pageSize,
            )
            _uiState.value = _uiState.value.copy(
                notifications = result.items,
                totalCount = result.totalCount,
                isLoading = false,
            )
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    fun setFilter(filter: NotificationFilter) {
        if (_uiState.value.filter == filter) return
        _uiState.value = _uiState.value.copy(filter = filter)
        load(page = 1)
    }

    fun markRead(id: String, onDone: (() -> Unit)? = null) = viewModelScope.launch {
        try {
            repository.markRead(id)
            _uiState.value = _uiState.value.copy(
                notifications = _uiState.value.notifications.map {
                    if (it.id == id) it.copy(isRead = true) else it
                },
            )
            onDone?.invoke()
        } catch (e: Exception) {
            onDone?.invoke()
        }
    }

    fun markAllRead() = viewModelScope.launch {
        val unread = _uiState.value.notifications.filter { !it.isRead }
        if (unread.isEmpty()) return@launch
        _uiState.value = _uiState.value.copy(isLoading = true)
        try {
            unread.forEach { repository.markRead(it.id) }
            load(page = _uiState.value.page)
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    val totalPages: Int
        get() = maxOf(1, (_uiState.value.totalCount + pageSize - 1) / pageSize)
}
