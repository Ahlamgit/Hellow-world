import Foundation

struct CraftsmanProfile: Codable, Identifiable {
    let id: UUID
    let userId: UUID
    let specialization: String?
    let yearsOfExperience: Int
    let rating: Double
    let totalReviews: Int
    let completedJobs: Int
    let isAvailable: Bool
    let serviceRadiusKm: Double?
    let services: [CraftsmanServiceItem]
    let workingHours: [CraftsmanWorkingHour]
}

struct CraftsmanServiceItem: Codable, Identifiable {
    let id: UUID
    let serviceId: UUID
    let serviceNameEn: String
    let serviceNameAr: String
    let customPrice: Decimal
    let isAvailable: Bool
}

struct CraftsmanWorkingHour: Codable, Identifiable {
    let id: UUID
    let dayOfWeek: Int
    let startTime: String
    let endTime: String
    let isActive: Bool
}

struct UpdateCraftsmanProfileBody: Encodable {
    let specialization: String?
    let yearsOfExperience: Int
    let isAvailable: Bool
    let serviceRadiusKm: Double?
}

struct UpsertCraftsmanServiceBody: Encodable {
    let serviceId: UUID
    let customPrice: Decimal
    let isAvailable: Bool
}

struct StoreProfile: Codable, Identifiable {
    let id: UUID
    let userId: UUID
    let storeName: String
    let description: String?
    let rating: Double
    let totalReviews: Int
    let isOpen: Bool
    let openingTime: String?
    let closingTime: String?
    let products: [StoreProduct]
}

struct StoreProduct: Codable, Identifiable {
    let id: UUID
    let nameEn: String
    let nameAr: String
    let price: Decimal
    let stockQuantity: Int
    let isActive: Bool
}

struct UpdateStoreProfileBody: Encodable {
    let storeName: String
    let description: String?
    let isOpen: Bool
    let openingTime: String?
    let closingTime: String?
}

struct UpsertStoreProductBody: Encodable {
    let nameEn: String
    let nameAr: String
    let price: Decimal
    let stockQuantity: Int
    let isActive: Bool
}

enum PortalRoleSupport {
    static func isCraftsman(_ role: String?) -> Bool {
        role == "Craftsman"
    }

    static func isStore(_ role: String?) -> Bool {
        guard let role else { return false }
        return ["Store", "StoreOwner", "StoreEmployee"].contains(role)
    }
}
