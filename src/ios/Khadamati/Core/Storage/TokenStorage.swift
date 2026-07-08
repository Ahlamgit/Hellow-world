import Foundation
import Security

protocol TokenStorageProtocol: Sendable {
    func save(accessToken: String, refreshToken: String) throws
    func getAccessToken() -> String?
    func getRefreshToken() -> String?
    func clear() throws
}

enum KeychainError: LocalizedError {
    case saveFailed(OSStatus)
    case deleteFailed(OSStatus)
    case unexpectedData

    var errorDescription: String? {
        switch self {
        case .saveFailed(let status):
            return "Keychain save failed with status \(status)."
        case .deleteFailed(let status):
            return "Keychain delete failed with status \(status)."
        case .unexpectedData:
            return "Unexpected keychain data format."
        }
    }
}

final class TokenStorage: TokenStorageProtocol, @unchecked Sendable {
    static let shared = TokenStorage()

    private let service = "com.khadamati.app.tokens"
    private let accessTokenKey = "access_token"
    private let refreshTokenKey = "refresh_token"

    private init() {}

    func save(accessToken: String, refreshToken: String) throws {
        try saveItem(accessToken, account: accessTokenKey)
        try saveItem(refreshToken, account: refreshTokenKey)
    }

    func getAccessToken() -> String? {
        readItem(account: accessTokenKey)
    }

    func getRefreshToken() -> String? {
        readItem(account: refreshTokenKey)
    }

    func clear() throws {
        try deleteItem(account: accessTokenKey)
        try deleteItem(account: refreshTokenKey)
    }

    // MARK: - Keychain Helpers

    private func saveItem(_ value: String, account: String) throws {
        guard let data = value.data(using: .utf8) else {
            throw KeychainError.unexpectedData
        }

        let query: [String: Any] = [
            kSecClass as String: kSecClassGenericPassword,
            kSecAttrService as String: service,
            kSecAttrAccount as String: account
        ]

        SecItemDelete(query as CFDictionary)

        var attributes = query
        attributes[kSecValueData as String] = data
        attributes[kSecAttrAccessible as String] = kSecAttrAccessibleAfterFirstUnlockThisDeviceOnly

        let status = SecItemAdd(attributes as CFDictionary, nil)
        guard status == errSecSuccess else {
            throw KeychainError.saveFailed(status)
        }
    }

    private func readItem(account: String) -> String? {
        let query: [String: Any] = [
            kSecClass as String: kSecClassGenericPassword,
            kSecAttrService as String: service,
            kSecAttrAccount as String: account,
            kSecReturnData as String: true,
            kSecMatchLimit as String: kSecMatchLimitOne
        ]

        var result: AnyObject?
        let status = SecItemCopyMatching(query as CFDictionary, &result)

        guard status == errSecSuccess,
              let data = result as? Data,
              let string = String(data: data, encoding: .utf8) else {
            return nil
        }

        return string
    }

    private func deleteItem(account: String) throws {
        let query: [String: Any] = [
            kSecClass as String: kSecClassGenericPassword,
            kSecAttrService as String: service,
            kSecAttrAccount as String: account
        ]

        let status = SecItemDelete(query as CFDictionary)
        guard status == errSecSuccess || status == errSecItemNotFound else {
            throw KeychainError.deleteFailed(status)
        }
    }
}
