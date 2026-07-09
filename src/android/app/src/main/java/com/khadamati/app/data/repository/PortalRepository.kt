package com.khadamati.app.data.repository

import com.khadamati.app.data.remote.ApiService
import com.khadamati.app.data.remote.dto.CraftsmanProfileDto
import com.khadamati.app.data.remote.dto.CraftsmanServiceItemDto
import com.khadamati.app.data.remote.dto.ServiceDto
import com.khadamati.app.data.remote.dto.StoreProductDto
import com.khadamati.app.data.remote.dto.StoreProfileDto
import com.khadamati.app.data.remote.dto.UpdateCraftsmanProfileRequestDto
import com.khadamati.app.data.remote.dto.UpdateStoreProfileRequestDto
import com.khadamati.app.data.remote.dto.UpsertCraftsmanServiceRequestDto
import com.khadamati.app.data.remote.dto.UpsertStoreProductRequestDto

class PortalRepository(private val apiService: ApiService) {
    suspend fun getCraftsmanProfile(): CraftsmanProfileDto =
        apiService.getCraftsmanProfile().data
            ?: throw IllegalStateException("Craftsman profile not found")

    suspend fun updateCraftsmanProfile(request: UpdateCraftsmanProfileRequestDto): CraftsmanProfileDto =
        apiService.updateCraftsmanProfile(request).data
            ?: throw IllegalStateException("Failed to update craftsman profile")

    suspend fun upsertCraftsmanService(request: UpsertCraftsmanServiceRequestDto): CraftsmanServiceItemDto =
        apiService.upsertCraftsmanService(request).data
            ?: throw IllegalStateException("Failed to save craftsman service")

    suspend fun getStoreProfile(): StoreProfileDto =
        apiService.getStoreProfile().data
            ?: throw IllegalStateException("Store profile not found")

    suspend fun updateStoreProfile(request: UpdateStoreProfileRequestDto): StoreProfileDto =
        apiService.updateStoreProfile(request).data
            ?: throw IllegalStateException("Failed to update store profile")

    suspend fun createStoreProduct(request: UpsertStoreProductRequestDto): StoreProductDto =
        apiService.createStoreProduct(request).data
            ?: throw IllegalStateException("Failed to create product")

    suspend fun getServices(): List<ServiceDto> =
        apiService.getServices().data.orEmpty()
}
