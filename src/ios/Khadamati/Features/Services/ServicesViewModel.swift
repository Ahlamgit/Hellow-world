import Foundation

@MainActor
final class ServicesViewModel: ObservableObject {
    @Published private(set) var categories: [ServiceCategory] = []
    @Published private(set) var services: [Service] = []
    @Published var selectedCategoryId: UUID?
    @Published private(set) var isLoading = false
    @Published var errorMessage: String?

    private let apiClient: APIClient

    init(apiClient: APIClient = .shared) {
        self.apiClient = apiClient
    }

    func loadInitialData() async {
        await loadCategories()
        await loadServices()
    }

    func loadCategories() async {
        do {
            let response: ApiResponse<[ServiceCategory]> = try await apiClient.request(
                url: APIEndpoints.Services.categories,
                method: .get,
                requiresAuth: false
            )
            categories = response.data.sorted { $0.displayOrder < $1.displayOrder }
        } catch let error as LocalizedError {
            errorMessage = error.errorDescription ?? L10n.Common.error
        } catch {
            errorMessage = L10n.Common.error
        }
    }

    func loadServices() async {
        isLoading = true
        errorMessage = nil
        defer { isLoading = false }

        do {
            let response: ApiResponse<[Service]> = try await apiClient.request(
                url: APIEndpoints.Services.list(categoryId: selectedCategoryId),
                method: .get,
                requiresAuth: false
            )
            services = response.data
        } catch let error as LocalizedError {
            errorMessage = error.errorDescription ?? L10n.Common.error
        } catch {
            errorMessage = L10n.Common.error
        }
    }

    func selectCategory(_ categoryId: UUID?) async {
        selectedCategoryId = categoryId
        await loadServices()
    }
}
