import Foundation
import UIKit
import UserNotifications

@MainActor
final class PushNotificationManager: NSObject {
    static let shared = PushNotificationManager()
    static let tokenDidUpdateNotification = Notification.Name("KhadamatiPushTokenDidUpdate")

    private override init() {
        super.init()
    }

    func configure() {
        UNUserNotificationCenter.current().delegate = self
        requestAuthorizationAndRegister()
    }

    func handleDeviceToken(_ deviceToken: Data) {
        let token = deviceToken.map { String(format: "%02.2hhx", $0) }.joined()
        let previous = PushTokenStorage.shared.getStoredToken()
        PushTokenStorage.shared.updateToken(token)
        if previous != token {
            NotificationCenter.default.post(name: Self.tokenDidUpdateNotification, object: nil)
        }
    }

    func handleRegistrationFailure(error: Error) {
        print("APNs registration failed: \(error.localizedDescription)")
        _ = PushTokenStorage.shared.getOrCreateToken()
        NotificationCenter.default.post(name: Self.tokenDidUpdateNotification, object: nil)
    }

    private func requestAuthorizationAndRegister() {
        UNUserNotificationCenter.current().requestAuthorization(options: [.alert, .badge, .sound]) { granted, error in
            if let error {
                print("Push authorization error: \(error.localizedDescription)")
            }
            if !granted {
                print("Push authorization denied — using dev fallback token when needed")
            }
            DispatchQueue.main.async {
                UIApplication.shared.registerForRemoteNotifications()
            }
        }
    }
}

extension PushNotificationManager: UNUserNotificationCenterDelegate {
    func userNotificationCenter(
        _ center: UNUserNotificationCenter,
        willPresent notification: UNNotification
    ) async -> UNNotificationPresentationOptions {
        [.banner, .sound, .badge]
    }
}
