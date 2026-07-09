package com.khadamati.app.data.repository

import com.khadamati.app.data.remote.ApiService
import com.khadamati.app.data.remote.dto.CancelSubscriptionRequestDto
import com.khadamati.app.data.remote.dto.PagedResultDto
import com.khadamati.app.data.remote.dto.SubscribeRequestDto
import com.khadamati.app.data.remote.dto.SubscriptionPlanDto
import com.khadamati.app.data.remote.dto.UpdateAutoRenewRequestDto
import com.khadamati.app.data.remote.dto.UserSubscriptionDto

class SubscriptionRepository(private val apiService: ApiService) {

    suspend fun getPlans(targetRole: String? = null): List<SubscriptionPlanDto> =
        apiService.getSubscriptionPlans(targetRole).data.orEmpty()

    suspend fun getPlan(id: String): SubscriptionPlanDto =
        apiService.getSubscriptionPlan(id).data ?: error("Plan not found")

    suspend fun getCurrent(): UserSubscriptionDto? =
        apiService.getCurrentSubscription().data

    suspend fun getHistory(page: Int = 1, pageSize: Int = 20): PagedResultDto<UserSubscriptionDto> =
        apiService.getSubscriptionHistory(page, pageSize).data
            ?: PagedResultDto(emptyList(), 0, page, pageSize)

    suspend fun subscribe(planId: String, billingOptionId: String, autoRenew: Boolean): UserSubscriptionDto =
        apiService.subscribe(SubscribeRequestDto(planId, billingOptionId, autoRenew)).data
            ?: error("Subscription failed")

    suspend fun cancel(id: String, reason: String?) {
        apiService.cancelSubscription(id, CancelSubscriptionRequestDto(reason))
    }

    suspend fun updateAutoRenew(autoRenew: Boolean): UserSubscriptionDto =
        apiService.updateAutoRenew(UpdateAutoRenewRequestDto(autoRenew)).data
            ?: error("Failed to update auto-renew")
}
