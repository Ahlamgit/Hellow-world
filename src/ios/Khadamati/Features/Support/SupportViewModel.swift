import Foundation

@MainActor
final class SupportViewModel: ObservableObject {
    @Published var complaints: [Complaint] = []
    @Published var tickets: [SupportTicket] = []
    @Published var isLoading = false
    @Published var message: String?
    @Published var errorMessage: String?

    private let apiClient: APIClient

    init(apiClient: APIClient = .shared) {
        self.apiClient = apiClient
    }

    func load() async {
        isLoading = true
        errorMessage = nil
        defer { isLoading = false }
        do {
            let complaintsResponse: ApiResponse<[Complaint]> = try await apiClient.request(
                url: APIEndpoints.Support.myComplaints,
                method: .get,
                requiresAuth: true
            )
            let ticketsResponse: ApiResponse<[SupportTicket]> = try await apiClient.request(
                url: APIEndpoints.Support.myTickets,
                method: .get,
                requiresAuth: true
            )
            complaints = complaintsResponse.data
            tickets = ticketsResponse.data
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func submitComplaint(subject: String, description: String) async {
        do {
            let body = CreateComplaintBody(subject: subject, description: description, priority: "Normal")
            let _: ApiResponse<Complaint> = try await apiClient.request(
                url: APIEndpoints.Support.complaints,
                method: .post,
                body: body,
                requiresAuth: true
            )
            message = L10n.Support.complaintSubmitted
            await load()
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func submitTicket(subject: String, description: String, category: String) async {
        do {
            let body = CreateSupportTicketBody(subject: subject, description: description, category: category, priority: "Normal")
            let _: ApiResponse<SupportTicket> = try await apiClient.request(
                url: APIEndpoints.Support.tickets,
                method: .post,
                body: body,
                requiresAuth: true
            )
            message = L10n.Support.ticketSubmitted
            await load()
        } catch {
            errorMessage = error.localizedDescription
        }
    }
}
