import Foundation
import UIKit

/// Stores the device push token. Uses a dev-prefixed simulated token until APNs/FCM is integrated.
/// Call `updateToken(_:)` when the system provides a real push registration token.
final class PushTokenStorage {
    static let shared = PushTokenStorage()
    static let devPrefix = "dev-ios-"
    private let key = "device_push_token"

    func getOrCreateToken() -> String {
        if let existing = UserDefaults.standard.string(forKey: key), !existing.isEmpty {
            return existing
        }
        let token = "\(Self.devPrefix)\(UUID().uuidString)"
        UserDefaults.standard.set(token, forKey: key)
        return token
    }

    func updateToken(_ token: String) {
        let trimmed = token.trimmingCharacters(in: .whitespacesAndNewlines)
        guard !trimmed.isEmpty else { return }
        UserDefaults.standard.set(trimmed, forKey: key)
    }

    func isSimulatedToken(_ token: String) -> Bool {
        token.lowercased().hasPrefix(Self.devPrefix)
    }
}

enum PushRegistrationService {
    static func registerCurrentDevice() async {
        let token = PushTokenStorage.shared.getOrCreateToken()
        let body = RegisterPushTokenRequest(
            token: token,
            platform: "iOS",
            deviceName: UIDevice.current.name
        )
        try? await APIClient.shared.requestVoid(
            url: APIEndpoints.Devices.pushToken,
            method: .post,
            body: body,
            requiresAuth: true
        )
    }

    static func unregisterCurrentDevice() async {
        let token = PushTokenStorage.shared.getOrCreateToken()
        var components = URLComponents(url: APIEndpoints.Devices.pushToken, resolvingAgainstBaseURL: false)!
        components.queryItems = [URLQueryItem(name: "token", value: token)]
        try? await APIClient.shared.requestVoid(url: components.url!, method: .delete, requiresAuth: true)
    }
}
