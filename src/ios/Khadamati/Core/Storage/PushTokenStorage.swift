import Foundation
import UIKit

final class PushTokenStorage {
    static let shared = PushTokenStorage()
    private let key = "device_push_token"

    func getOrCreateToken() -> String {
        if let existing = UserDefaults.standard.string(forKey: key), !existing.isEmpty {
            return existing
        }
        let token = "ios-\(UUID().uuidString)"
        UserDefaults.standard.set(token, forKey: key)
        return token
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
