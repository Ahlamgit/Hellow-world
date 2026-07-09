import Foundation

@MainActor
final class StorePortalViewModel: ObservableObject {
    @Published var profile: StoreProfile?
    @Published var isLoading = false
    @Published var message: String?
    @Published var errorMessage: String?

    private let apiClient: APIClient

    init(apiClient: APIClient = .shared) {
        self.apiClient = apiClient
    }

    func load() async {
        isLoading = true
        errorMessage = nil
        message = nil
        defer { isLoading = false }
        do {
            let response: ApiResponse<StoreProfile> = try await apiClient.request(
                url: APIEndpoints.StorePortal.profile,
                method: .get,
                requiresAuth: true
            )
            profile = response.data
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func saveProfile(storeName: String, description: String, isOpen: Bool, openingTime: String, closingTime: String) async {
        isLoading = true
        errorMessage = nil
        message = nil
        defer { isLoading = false }
        do {
            let body = UpdateStoreProfileBody(
                storeName: storeName,
                description: description.isEmpty ? nil : description,
                isOpen: isOpen,
                openingTime: openingTime.isEmpty ? nil : openingTime,
                closingTime: closingTime.isEmpty ? nil : closingTime
            )
            let response: ApiResponse<StoreProfile> = try await apiClient.request(
                url: APIEndpoints.StorePortal.profile,
                method: .put,
                body: body,
                requiresAuth: true
            )
            profile = response.data
            message = L10n.Portal.saved
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func addProduct(nameEn: String, nameAr: String, price: Decimal, stockQuantity: Int) async {
        guard !nameEn.isEmpty else { return }
        isLoading = true
        errorMessage = nil
        message = nil
        defer { isLoading = false }
        do {
            let body = UpsertStoreProductBody(
                nameEn: nameEn,
                nameAr: nameAr.isEmpty ? nameEn : nameAr,
                price: price,
                stockQuantity: stockQuantity,
                isActive: true
            )
            let _: ApiResponse<StoreProduct> = try await apiClient.request(
                url: APIEndpoints.StorePortal.products,
                method: .post,
                body: body,
                requiresAuth: true
            )
            message = L10n.Portal.productAdded
            await load()
        } catch {
            errorMessage = error.localizedDescription
        }
    }
}
