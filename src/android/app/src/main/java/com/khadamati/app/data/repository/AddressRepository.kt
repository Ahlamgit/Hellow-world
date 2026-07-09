package com.khadamati.app.data.repository

import com.khadamati.app.data.remote.ApiService
import com.khadamati.app.data.remote.dto.AddressDto
import com.khadamati.app.data.remote.dto.CreateAddressRequestDto

class AddressRepository(private val apiService: ApiService) {
    suspend fun list(): List<AddressDto> = apiService.getAddresses().data.orEmpty()

    suspend fun add(request: CreateAddressRequestDto): AddressDto =
        apiService.addAddress(request).data ?: error("Failed to save address")
}
