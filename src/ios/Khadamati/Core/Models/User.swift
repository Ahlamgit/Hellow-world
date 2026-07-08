import Foundation

struct User: Codable, Identifiable, Equatable {
    let id: UUID
    let email: String
    let phone: String
    let role: String
    let status: String?
    let verificationStatus: String?
    let subscriptionStatus: String?
    let firstName: String
    let lastName: String
    let profilePictureUrl: String?
    let preferredLanguage: String
    let bio: String?
    let addresses: [Address]?

    var fullName: String {
        "\(firstName) \(lastName)".trimmingCharacters(in: .whitespaces)
    }

    var displayRole: String {
        switch role.lowercased() {
        case "customer": return L10n.Auth.customer
        case "craftsman": return L10n.Auth.craftsman
        case "store": return L10n.Auth.store
        default: return role
        }
    }
}

struct Address: Codable, Identifiable, Equatable {
    let id: UUID
    let label: String
    let street: String
    let city: String
    let district: String?
    let postalCode: String?
    let country: String
    let latitude: Decimal?
    let longitude: Decimal?
    let isDefault: Bool
}
