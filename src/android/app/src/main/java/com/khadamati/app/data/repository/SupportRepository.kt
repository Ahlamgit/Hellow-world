package com.khadamati.app.data.repository

import com.khadamati.app.data.remote.ApiService
import com.khadamati.app.data.remote.dto.ComplaintDto
import com.khadamati.app.data.remote.dto.CreateComplaintRequestDto
import com.khadamati.app.data.remote.dto.CreateSupportTicketRequestDto
import com.khadamati.app.data.remote.dto.SupportTicketDto

class SupportRepository(private val apiService: ApiService) {
    suspend fun getComplaints(): List<ComplaintDto> =
        apiService.getMyComplaints().data.orEmpty()

    suspend fun getTickets(): List<SupportTicketDto> =
        apiService.getMySupportTickets().data.orEmpty()

    suspend fun createComplaint(subject: String, description: String) {
        apiService.createComplaint(CreateComplaintRequestDto(subject, description))
    }

    suspend fun createTicket(subject: String, description: String, category: String) {
        apiService.createSupportTicket(CreateSupportTicketRequestDto(subject, description, category))
    }
}
