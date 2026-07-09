import Foundation

@MainActor
protocol AuthServiceProtocol {
    func login(email: String, password: String) async throws -> AuthResponse
    func register(_ request: RegisterRequest) async throws -> AuthResponse
    func logout() async throws
    func fetchProfile() async throws -> User
}

@MainActor
final class AuthService: AuthServiceProtocol {
    private let apiClient: APIClient
    private let tokenStorage: TokenStorageProtocol

    init(
        apiClient: APIClient = .shared,
        tokenStorage: TokenStorageProtocol = TokenStorage.shared
    ) {
        self.apiClient = apiClient
        self.tokenStorage = tokenStorage
    }

    func login(email: String, password: String) async throws -> AuthResponse {
        let payload = LoginRequest(email: email, password: password)
        let response: ApiResponse<AuthResponse> = try await apiClient.request(
            url: APIEndpoints.Auth.login,
            method: .post,
            body: payload,
            requiresAuth: false
        )
        try persistTokens(from: response.data)
        await PushRegistrationService.registerCurrentDevice()
        return response.data
    }

    func register(_ request: RegisterRequest) async throws -> AuthResponse {
        let response: ApiResponse<AuthResponse> = try await apiClient.request(
            url: APIEndpoints.Auth.register,
            method: .post,
            body: request,
            requiresAuth: false
        )
        try persistTokens(from: response.data)
        await PushRegistrationService.registerCurrentDevice()
        return response.data
    }

    func logout() async throws {
        if let refreshToken = tokenStorage.getRefreshToken() {
            try? await apiClient.requestVoid(
                url: APIEndpoints.Auth.revoke,
                method: .post,
                body: refreshToken,
                requiresAuth: true
            )
        }
        await PushRegistrationService.unregisterCurrentDevice()
        try tokenStorage.clear()
    }

    func fetchProfile() async throws -> User {
        let response: ApiResponse<User> = try await apiClient.request(
            url: APIEndpoints.Users.me,
            method: .get,
            requiresAuth: true
        )
        return response.data
    }

    private func persistTokens(from response: AuthResponse) throws {
        try tokenStorage.save(
            accessToken: response.accessToken,
            refreshToken: response.refreshToken
        )
    }
}
