import Foundation

@MainActor
final class CraftsmanPortalViewModel: ObservableObject {
    @Published var profile: CraftsmanProfile?
    @Published var services: [Service] = []
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
            let profileResponse: ApiResponse<CraftsmanProfile> = try await apiClient.request(
                url: APIEndpoints.CraftsmanPortal.profile,
                method: .get,
                requiresAuth: true
            )
            let servicesResponse: ApiResponse<[Service]> = try await apiClient.request(
                url: APIEndpoints.Services.list(),
                method: .get,
                requiresAuth: false
            )
            profile = profileResponse.data
            services = servicesResponse.data
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func saveProfile(specialization: String, yearsOfExperience: Int, isAvailable: Bool, serviceRadiusKm: Double) async {
        isLoading = true
        errorMessage = nil
        message = nil
        defer { isLoading = false }
        do {
            let body = UpdateCraftsmanProfileBody(
                specialization: specialization.isEmpty ? nil : specialization,
                yearsOfExperience: yearsOfExperience,
                isAvailable: isAvailable,
                serviceRadiusKm: serviceRadiusKm
            )
            let response: ApiResponse<CraftsmanProfile> = try await apiClient.request(
                url: APIEndpoints.CraftsmanPortal.profile,
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

    func addService(serviceId: UUID, customPrice: Decimal) async {
        isLoading = true
        errorMessage = nil
        message = nil
        defer { isLoading = false }
        do {
            let body = UpsertCraftsmanServiceBody(serviceId: serviceId, customPrice: customPrice, isAvailable: true)
            let _: ApiResponse<CraftsmanServiceItem> = try await apiClient.request(
                url: APIEndpoints.CraftsmanPortal.services,
                method: .post,
                body: body,
                requiresAuth: true
            )
            message = L10n.Portal.serviceAdded
            await load()
        } catch {
            errorMessage = error.localizedDescription
        }
    }
}
