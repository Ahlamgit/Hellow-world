import Foundation

@MainActor
final class SubscriptionViewModel: ObservableObject {
    @Published var plans: [SubscriptionPlan] = []
    @Published var selectedPlan: SubscriptionPlan?
    @Published var current: UserSubscription?
    @Published var history: [UserSubscription] = []
    @Published var selectedBillingOptionId: UUID?
    @Published var autoRenew = true
    @Published var isLoading = false
    @Published var isSubmitting = false
    @Published var errorMessage: String?

    private let apiClient: APIClient

    init(apiClient: APIClient = .shared) {
        self.apiClient = apiClient
    }

    func loadPlans(targetRole: String?) async {
        isLoading = true
        errorMessage = nil
        defer { isLoading = false }

        do {
            let plansResponse: ApiResponse<[SubscriptionPlan]> = try await apiClient.request(
                url: APIEndpoints.SubscriptionPlans.list(targetRole: targetRole),
                method: .get,
                requiresAuth: false
            )
            plans = plansResponse.data
            if let currentResponse: ApiResponse<UserSubscription?> = try? await apiClient.request(
                url: APIEndpoints.MeSubscription.current,
                method: .get,
                requiresAuth: true
            ) {
                current = currentResponse.data
            }
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func loadPlan(_ planId: UUID) async {
        isLoading = true
        errorMessage = nil
        defer { isLoading = false }

        do {
            let response: ApiResponse<SubscriptionPlan> = try await apiClient.request(
                url: APIEndpoints.SubscriptionPlans.detail(planId),
                method: .get,
                requiresAuth: false
            )
            selectedPlan = response.data
            selectedBillingOptionId = response.data.billingOptions.first(where: { $0.isActive })?.optionId
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func loadMySubscription() async {
        isLoading = true
        errorMessage = nil
        defer { isLoading = false }

        do {
            async let currentResponse: ApiResponse<UserSubscription?> = apiClient.request(
                url: APIEndpoints.MeSubscription.current,
                method: .get,
                requiresAuth: true
            )
            async let historyResponse: ApiResponse<PagedUserSubscriptions> = apiClient.request(
                url: APIEndpoints.MeSubscription.history(),
                method: .get,
                requiresAuth: true
            )
            let (currentResult, historyResult) = try await (currentResponse, historyResponse)
            current = currentResult.data
            history = historyResult.data.items
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func subscribe() async -> Bool {
        guard let plan = selectedPlan, let billingOptionId = selectedBillingOptionId else { return false }
        isSubmitting = true
        errorMessage = nil
        defer { isSubmitting = false }

        do {
            let body = SubscribeRequest(
                planId: plan.id,
                billingOptionId: billingOptionId,
                autoRenew: autoRenew,
                couponCode: nil
            )
            let response: ApiResponse<UserSubscription> = try await apiClient.request(
                url: APIEndpoints.MeSubscription.subscribe,
                method: .post,
                body: body,
                requiresAuth: true
            )
            current = response.data
            return true
        } catch {
            errorMessage = error.localizedDescription
            return false
        }
    }

    func updateAutoRenew(_ enabled: Bool) async {
        isSubmitting = true
        errorMessage = nil
        defer { isSubmitting = false }

        do {
            let response: ApiResponse<UserSubscription> = try await apiClient.request(
                url: APIEndpoints.MeSubscription.autoRenew,
                method: .patch,
                body: UpdateAutoRenewRequest(autoRenew: enabled),
                requiresAuth: true
            )
            current = response.data
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func cancel(reason: String?) async {
        guard let subscription = current else { return }
        isSubmitting = true
        errorMessage = nil
        defer { isSubmitting = false }

        do {
            try await apiClient.requestVoid(
                url: APIEndpoints.MeSubscription.cancel(subscription.id),
                method: .post,
                body: CancelSubscriptionRequest(reason: reason),
                requiresAuth: true
            )
            await loadMySubscription()
        } catch {
            errorMessage = error.localizedDescription
        }
    }
}
