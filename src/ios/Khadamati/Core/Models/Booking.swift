import Foundation

struct Booking: Codable, Identifiable {
    let id: UUID
    let bookingReference: String
    let serviceId: UUID
    let serviceName: String
    let customerId: UUID
    let customerName: String
    let craftsmanId: UUID
    let craftsmanName: String
    let status: String
    let scheduledAt: Date
    let slotEnd: Date
    let estimatedPrice: Decimal
    let payment: BookingPayment?
}

struct BookingPayment: Codable {
    let id: UUID
    let amount: Decimal
    let currency: String
    let status: String
    let paymentMethod: String
    let sessionId: String?
    let transactionReference: String?
    let checkoutUrl: String?
}

struct CraftsmanOption: Codable, Identifiable {
    let id: UUID
    let firstName: String
    let lastName: String
    let specialization: String?
    let rating: Double
    let totalReviews: Int
    let completedJobs: Int
    let price: Decimal
    let isAvailable: Bool
    let distanceKm: Double?
}

struct TimeSlot: Codable, Identifiable {
    var id: String { start.description }
    let start: Date
    let end: Date
    let isAvailable: Bool
}

struct PagedBookings: Codable {
    let items: [Booking]
    let totalCount: Int
    let page: Int
    let pageSize: Int
}

struct CreateBookingRequest: Codable {
    let serviceId: UUID
    let craftsmanId: UUID
    let scheduledAt: Date
}

struct CreateBookingBody: Codable {
    let serviceId: UUID
    let craftsmanId: UUID
    let scheduledAt: String
}
