import SwiftUI

@MainActor
final class AppSession: ObservableObject {
    @Published private(set) var currentUser: User?
    @Published private(set) var isAuthenticated = false
    @Published var preferredLanguage: String {
        didSet {
            UserDefaults.standard.set(preferredLanguage, forKey: "preferredLanguage")
            applyLanguage()
        }
    }

    private let tokenStorage: TokenStorageProtocol
    private let authService: AuthServiceProtocol

    var layoutDirection: LayoutDirection {
        preferredLanguage == "ar" ? .rightToLeft : .leftToRight
    }

    var colorScheme: ColorScheme? {
        nil
    }

    init(
        tokenStorage: TokenStorageProtocol = TokenStorage.shared,
        authService: AuthServiceProtocol? = nil
    ) {
        self.tokenStorage = tokenStorage
        self.authService = authService ?? AuthService()
        self.preferredLanguage = UserDefaults.standard.string(forKey: "preferredLanguage") ?? "ar"
        applyLanguage()
        observePushTokenUpdates()
        restoreSession()
    }

    private func observePushTokenUpdates() {
        NotificationCenter.default.addObserver(
            forName: PushNotificationManager.tokenDidUpdateNotification,
            object: nil,
            queue: .main
        ) { [weak self] _ in
            guard let self, self.isAuthenticated else { return }
            Task { await PushRegistrationService.registerCurrentDevice() }
        }
    }

    func signIn(user: User) {
        currentUser = user
        isAuthenticated = true
        preferredLanguage = user.preferredLanguage
        Task { await PushRegistrationService.registerCurrentDevice() }
    }

    func signOut() {
        currentUser = nil
        isAuthenticated = false
    }

    func updateUser(_ user: User) {
        currentUser = user
    }

    func restoreSession() {
        guard tokenStorage.getAccessToken() != nil else {
            isAuthenticated = false
            return
        }

        Task {
            do {
                let user = try await authService.fetchProfile()
                signIn(user: user)
            } catch {
                try? tokenStorage.clear()
                signOut()
            }
        }
    }
}

// MARK: - Language

private extension AppSession {
    func applyLanguage() {
        UserDefaults.standard.set([preferredLanguage], forKey: "AppleLanguages")
        UserDefaults.standard.synchronize()
    }
}
