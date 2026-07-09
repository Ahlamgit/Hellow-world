package com.khadamati.app.data.repository

import com.khadamati.app.data.remote.ApiService
import com.khadamati.app.data.remote.dto.ChatConversationDto
import com.khadamati.app.data.remote.dto.ChatMessageDto
import com.khadamati.app.data.remote.dto.SendChatMessageRequestDto

class ChatRepository(private val apiService: ApiService) {
    suspend fun getConversations(): List<ChatConversationDto> =
        apiService.getChatConversations().data.orEmpty()

    suspend fun getBookingChat(bookingId: String): ChatConversationDto =
        apiService.getBookingChat(bookingId).data ?: error("Chat not found")

    suspend fun getMessages(conversationId: String): List<ChatMessageDto> =
        apiService.getChatMessages(conversationId).data?.items.orEmpty()

    suspend fun sendMessage(conversationId: String, body: String): ChatMessageDto =
        apiService.sendChatMessage(conversationId, SendChatMessageRequestDto(body)).data
            ?: error("Failed to send message")
}
