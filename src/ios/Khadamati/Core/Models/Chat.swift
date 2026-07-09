import Foundation

struct ChatConversation: Codable, Identifiable, Equatable {
    let id: UUID
    let bookingId: UUID
    let bookingReference: String
    let serviceName: String
    let customerId: UUID
    let customerName: String
    let craftsmanId: UUID
    let craftsmanName: String
    let lastMessageAt: Date?
    let lastMessagePreview: String?
    let unreadCount: Int
}

struct ChatMessage: Codable, Identifiable, Equatable {
    let id: UUID
    let conversationId: UUID
    let senderId: UUID
    let senderName: String
    let body: String
    let sentAt: Date
    let isRead: Bool
    let isMine: Bool
}

struct PagedChatMessages: Codable {
    let items: [ChatMessage]
    let totalCount: Int
    let page: Int
    let pageSize: Int
}

struct SendChatMessageRequest: Encodable {
    let body: String
}

struct RegisterPushTokenRequest: Encodable {
    let token: String
    let platform: String
    let deviceName: String?
}

struct CreateAddressRequest: Encodable {
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
