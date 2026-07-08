import Foundation

struct AuthResponse: Codable {
    let accessToken: String
    let refreshToken: String
    let expiresAt: Date
    let user: User
}

struct LoginRequest: Encodable {
    let email: String
    let password: String
}

struct RegisterRequest: Encodable {
    let email: String
    let phone: String
    let password: String
    let firstName: String
    let lastName: String
    let role: String
    let preferredLanguage: String
}

struct RefreshTokenRequest: Encodable {
    let accessToken: String
    let refreshToken: String
}
