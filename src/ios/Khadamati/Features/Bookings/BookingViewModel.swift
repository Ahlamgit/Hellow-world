import Foundation

@MainActor
final class BookingViewModel: ObservableObject {
    @Published var bookings: [Booking] = []
    @Published var craftsmen: [CraftsmanOption] = []
    @Published var slots: [TimeSlot] = []
    @Published var isLoading = false
    @Published var errorMessage: String?

    private let apiClient: APIClient

    init(apiClient: APIClient = .shared) {
        self.apiClient = apiClient
    }

    func loadBookings() async {
        isLoading = true
        defer { isLoading = false }
        do {
            let response: ApiResponse<PagedBookings> = try await apiClient.request(
                url: APIEndpoints.Bookings.list,
                method: .get,
                requiresAuth: true
            )
            bookings = response.data.items
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func loadCraftsmen(serviceId: UUID) async {
        do {
            let response: ApiResponse<[CraftsmanOption]> = try await apiClient.request(
                url: APIEndpoints.Bookings.craftsmen(serviceId: serviceId),
                method: .get,
                requiresAuth: false
            )
            craftsmen = response.data
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func loadSlots(craftsmanId: UUID, serviceId: UUID, date: Date) async {
        let formatter = ISO8601DateFormatter()
        formatter.formatOptions = [.withFullDate]
        do {
            let response: ApiResponse<[TimeSlot]> = try await apiClient.request(
                url: APIEndpoints.Bookings.availability(craftsmanId: craftsmanId, serviceId: serviceId, date: formatter.string(from: date)),
                method: .get,
                requiresAuth: false
            )
            slots = response.data.filter { $0.isAvailable }
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func pay(bookingId: UUID) async {
        do {
            let _: ApiResponse<BookingPayment> = try await apiClient.request(
                url: APIEndpoints.Bookings.payment(bookingId),
                method: .post,
                body: ["paymentMethod": "Card"],
                requiresAuth: true
            )
            let _: ApiResponse<Booking> = try await apiClient.request(
                url: APIEndpoints.Bookings.confirmPayment(bookingId),
                method: .post,
                body: ["transactionReference": "TXN-\(Int(Date().timeIntervalSince1970))"],
                requiresAuth: true
            )
            await loadBookings()
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func accept(bookingId: UUID) async {
        try? await apiClient.requestVoid(url: APIEndpoints.Bookings.accept(bookingId), method: .post, requiresAuth: true)
        await loadBookings()
    }

    func reject(bookingId: UUID, reason: String) async {
        try? await apiClient.requestVoid(url: APIEndpoints.Bookings.reject(bookingId), method: .post, body: ["reason": reason], requiresAuth: true)
        await loadBookings()
    }
}
