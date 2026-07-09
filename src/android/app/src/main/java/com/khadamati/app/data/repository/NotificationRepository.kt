package com.khadamati.app.data.repository

import com.khadamati.app.data.remote.ApiService
import com.khadamati.app.data.remote.dto.NotificationDto
import com.khadamati.app.data.remote.dto.PagedResultDto

class NotificationRepository(private val apiService: ApiService) {

    suspend fun getNotifications(
        unreadOnly: Boolean = false,
        page: Int = 1,
        pageSize: Int = 15,
    ): PagedResultDto<NotificationDto> =
        apiService.getNotifications(unreadOnly, page, pageSize).data
            ?: PagedResultDto(emptyList(), 0, page, pageSize)

    suspend fun markRead(id: String) {
        apiService.markNotificationRead(id)
    }
}
