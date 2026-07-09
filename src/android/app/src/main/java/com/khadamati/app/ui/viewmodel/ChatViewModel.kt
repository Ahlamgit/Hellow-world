package com.khadamati.app.ui.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.khadamati.app.data.remote.dto.ChatConversationDto
import com.khadamati.app.data.remote.dto.ChatMessageDto
import com.khadamati.app.data.repository.ChatRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

data class ChatUiState(
    val conversations: List<ChatConversationDto> = emptyList(),
    val conversation: ChatConversationDto? = null,
    val messages: List<ChatMessageDto> = emptyList(),
    val draft: String = "",
    val isLoading: Boolean = false,
    val isSending: Boolean = false,
    val error: String? = null,
)

class ChatViewModel(private val repository: ChatRepository) : ViewModel() {
    private val _uiState = MutableStateFlow(ChatUiState())
    val uiState: StateFlow<ChatUiState> = _uiState.asStateFlow()

    fun loadConversations() = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isLoading = true, error = null)
        try {
            _uiState.value = _uiState.value.copy(
                conversations = repository.getConversations(),
                isLoading = false,
            )
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    fun openBookingChat(bookingId: String) = viewModelScope.launch {
        _uiState.value = _uiState.value.copy(isLoading = true, error = null)
        try {
            val conversation = repository.getBookingChat(bookingId)
            val messages = repository.getMessages(conversation.id)
            _uiState.value = _uiState.value.copy(
                conversation = conversation,
                messages = messages,
                isLoading = false,
            )
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isLoading = false)
        }
    }

    fun refreshMessages() = viewModelScope.launch {
        val conversationId = _uiState.value.conversation?.id ?: return@launch
        try {
            val messages = repository.getMessages(conversationId)
            _uiState.value = _uiState.value.copy(messages = messages)
        } catch (_: Exception) {
        }
    }

    fun updateDraft(value: String) {
        _uiState.value = _uiState.value.copy(draft = value)
    }

    fun sendMessage() = viewModelScope.launch {
        val conversationId = _uiState.value.conversation?.id ?: return@launch
        val body = _uiState.value.draft.trim()
        if (body.isEmpty()) return@launch
        _uiState.value = _uiState.value.copy(isSending = true, error = null)
        try {
            repository.sendMessage(conversationId, body)
            _uiState.value = _uiState.value.copy(draft = "", isSending = false)
            refreshMessages()
        } catch (e: Exception) {
            _uiState.value = _uiState.value.copy(error = e.message, isSending = false)
        }
    }
}
