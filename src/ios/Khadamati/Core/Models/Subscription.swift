import Foundation

struct PlanBillingOption: Codable, Equatable {
    let optionId: UUID?
    let cycle: String
    let price: Decimal
    let durationDays: Int
    let isActive: Bool

    var listId: String { optionId?.uuidString ?? "\(cycle)-\(price)-\(durationDays)" }

    enum CodingKeys: String, CodingKey {
        case optionId = "id"
        case cycle, price, durationDays, isActive
    }
}

struct SubscriptionPlan: Codable, Identifiable, Equatable {
    let id: UUID
    let planCode: String
    let nameEn: String
    let nameAr: String
    let descriptionEn: String?
    let descriptionAr: String?
    let currency: String
    let targetRole: String
    let status: String
    let isFeatured: Bool
    let maxServices: Int?
    let verificationBadge: Bool
    let premiumBadge: Bool
    let trialDays: Int
    let billingOptions: [PlanBillingOption]

    func name(isArabic: Bool) -> String { isArabic ? nameAr : nameEn }
    func description(isArabic: Bool) -> String? { isArabic ? descriptionAr : descriptionEn }
}

struct UserSubscription: Codable, Identifiable, Equatable {
    let id: UUID
    let userId: UUID
    let userEmail: String
    let userName: String
    let planId: UUID
    let planCode: String
    let planNameEn: String
    let planNameAr: String
    let billingOptionId: UUID?
    let billingCycle: String?
    let status: String
    let startDate: Date
    let endDate: Date?
    let autoRenew: Bool
    let amountPaid: Decimal?
    let currency: String?
    let couponCode: String?
    let cancelledAt: Date?
    let cancellationReason: String?
    let createdAt: Date

    func planName(isArabic: Bool) -> String { isArabic ? planNameAr : planNameEn }
}

struct PagedUserSubscriptions: Codable {
    let items: [UserSubscription]
    let totalCount: Int
    let page: Int
    let pageSize: Int
}

struct SubscribeRequest: Encodable {
    let planId: UUID
    let billingOptionId: UUID
    let autoRenew: Bool
    let couponCode: String?
}

struct CancelSubscriptionRequest: Encodable {
    let reason: String?
}

struct UpdateAutoRenewRequest: Encodable {
    let autoRenew: Bool
}

enum SubscriptionRoleSupport {
    static func targetRole(for userRole: String?) -> String? {
        switch userRole {
        case "Store", "StoreOwner": return "Store"
        case "Craftsman": return "Craftsman"
        default: return nil
        }
    }

    static func isSubscriber(_ role: String?) -> Bool {
        guard let role else { return false }
        return ["Craftsman", "Store", "StoreOwner"].contains(role)
    }
}
