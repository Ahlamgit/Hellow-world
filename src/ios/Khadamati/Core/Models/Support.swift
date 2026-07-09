import Foundation

struct Complaint: Codable, Identifiable {
    let id: UUID
    let subject: String
    let description: String
    let status: String
    let priority: String
    let createdAt: Date
}

struct SupportTicket: Codable, Identifiable {
    let id: UUID
    let ticketNumber: String
    let subject: String
    let description: String
    let status: String
    let priority: String
    let category: String
    let createdAt: Date
}

struct CreateComplaintBody: Encodable {
    let subject: String
    let description: String
    let priority: String
}

struct CreateSupportTicketBody: Encodable {
    let subject: String
    let description: String
    let category: String
    let priority: String
}

struct SubmitReviewBody: Encodable {
    let rating: Int
    let review: String?
}

struct UpdateProfileBody: Encodable {
    let firstName: String
    let lastName: String
    let preferredLanguage: String
}
