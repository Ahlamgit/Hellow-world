package com.khadamati.app.data.repository

import com.khadamati.app.data.local.dao.ServiceCategoryDao
import com.khadamati.app.data.local.dao.ServiceDao
import com.khadamati.app.data.remote.ApiService
import com.khadamati.app.domain.model.Service
import com.khadamati.app.domain.model.ServiceCategory
import com.khadamati.app.domain.model.toDomain
import com.khadamati.app.domain.model.toEntity
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map

class ServicesRepository(
    private val apiService: ApiService,
    private val categoryDao: ServiceCategoryDao,
    private val serviceDao: ServiceDao,
) {

    fun observeCategories(): Flow<List<ServiceCategory>> =
        categoryDao.observeAll().map { entities -> entities.map { it.toDomain() } }

    fun observeServices(categoryId: String?): Flow<List<Service>> {
        val flow = if (categoryId.isNullOrBlank()) {
            serviceDao.observeAll()
        } else {
            serviceDao.observeByCategory(categoryId)
        }
        return flow.map { entities -> entities.map { it.toDomain() } }
    }

    suspend fun refreshCategories(): Result<List<ServiceCategory>> = runCatching {
        val response = apiService.getServiceCategories()
        val data = response.data ?: throw ApiException(response.message ?: "Failed to load categories")
        val categories = data.map { it.toDomain() }
        categoryDao.upsertAll(categories.map { it.toEntity() })
        categories
    }

    suspend fun refreshServices(categoryId: String? = null): Result<List<Service>> = runCatching {
        val response = apiService.getServices(categoryId)
        val data = response.data ?: throw ApiException(response.message ?: "Failed to load services")
        val services = data.map { it.toDomain() }
        serviceDao.upsertAll(services.map { it.toEntity() })
        services
    }
}
