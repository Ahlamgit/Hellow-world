import Foundation

struct AppNotification: Codable, Identifiable, Equatable {
    let id: UUID
    let titleEn: String
    let titleAr: String
    let messageEn: String
    let messageAr: String
    let notificationType: String
    let referenceId: UUID?
    let isRead: Bool
    let createdAt: Date

    func title(isArabic: Bool) -> String { isArabic ? titleAr : titleEn }
    func message(isArabic: Bool) -> String { isArabic ? messageAr : messageEn }

    var bookingId: UUID? {
        guard let referenceId, notificationType.hasPrefix("Booking") else { return nil }
        return referenceId
    }
}

struct PagedNotifications: Codable {
    let items: [AppNotification]
    let totalCount: Int
    let page: Int
    let pageSize: Int
}
