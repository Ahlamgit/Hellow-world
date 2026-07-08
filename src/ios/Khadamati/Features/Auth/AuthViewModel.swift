import Foundation

@MainActor
final class AuthViewModel: ObservableObject {
    @Published var email = ""
    @Published var password = ""
    @Published var phone = ""
    @Published var firstName = ""
    @Published var lastName = ""
    @Published var role = "Customer"
    @Published var preferredLanguage = "ar"
    @Published var isLoading = false
    @Published var errorMessage: String?

    private let authService: AuthServiceProtocol

    init(authService: AuthServiceProtocol? = nil) {
        self.authService = authService ?? AuthService()
    }

    var isLoginValid: Bool {
        !email.trimmingCharacters(in: .whitespaces).isEmpty &&
        password.count >= 6
    }

    var isRegisterValid: Bool {
        isLoginValid &&
        !phone.trimmingCharacters(in: .whitespaces).isEmpty &&
        !firstName.trimmingCharacters(in: .whitespaces).isEmpty &&
        !lastName.trimmingCharacters(in: .whitespaces).isEmpty
    }

    func login(appSession: AppSession) async {
        guard isLoginValid else { return }

        isLoading = true
        errorMessage = nil
        defer { isLoading = false }

        do {
            let response = try await authService.login(
                email: email.trimmingCharacters(in: .whitespaces),
                password: password
            )
            appSession.signIn(user: response.user)
        } catch let error as LocalizedError {
            errorMessage = error.errorDescription ?? L10n.Common.error
        } catch {
            errorMessage = L10n.Common.error
        }
    }

    func register(appSession: AppSession) async {
        guard isRegisterValid else { return }

        isLoading = true
        errorMessage = nil
        defer { isLoading = false }

        let request = RegisterRequest(
            email: email.trimmingCharacters(in: .whitespaces),
            phone: phone.trimmingCharacters(in: .whitespaces),
            password: password,
            firstName: firstName.trimmingCharacters(in: .whitespaces),
            lastName: lastName.trimmingCharacters(in: .whitespaces),
            role: role,
            preferredLanguage: preferredLanguage
        )

        do {
            let response = try await authService.register(request)
            appSession.signIn(user: response.user)
        } catch let error as LocalizedError {
            errorMessage = error.errorDescription ?? L10n.Common.error
        } catch {
            errorMessage = L10n.Common.error
        }
    }
}
