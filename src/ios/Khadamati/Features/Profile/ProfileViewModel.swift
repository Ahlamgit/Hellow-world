import Foundation

@MainActor
final class ProfileViewModel: ObservableObject {
    @Published private(set) var isLoading = false
    @Published private(set) var isLoggingOut = false
    @Published var errorMessage: String?

    private let authService: AuthServiceProtocol

    init(authService: AuthServiceProtocol? = nil) {
        self.authService = authService ?? AuthService()
    }

    func refreshProfile(appSession: AppSession) async {
        isLoading = true
        errorMessage = nil
        defer { isLoading = false }

        do {
            let user = try await authService.fetchProfile()
            appSession.updateUser(user)
        } catch let error as LocalizedError {
            errorMessage = error.errorDescription ?? L10n.Common.error
        } catch {
            errorMessage = L10n.Common.error
        }
    }

    func logout(appSession: AppSession) async {
        isLoggingOut = true
        defer { isLoggingOut = false }

        do {
            try await authService.logout()
        } catch {
            // Clear local session even if revoke fails
        }

        appSession.signOut()
    }

    func updateLanguage(_ language: String, appSession: AppSession) {
        appSession.preferredLanguage = language
    }
}
