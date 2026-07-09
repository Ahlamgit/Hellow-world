import Foundation

@MainActor
final class ChatViewModel: ObservableObject {
    @Published var conversations: [ChatConversation] = []
    @Published var conversation: ChatConversation?
    @Published var messages: [ChatMessage] = []
    @Published var draft = ""
    @Published var isLoading = false
    @Published var isSending = false
    @Published var errorMessage: String?

    private let apiClient: APIClient

    init(apiClient: APIClient = .shared) {
        self.apiClient = apiClient
    }

    func loadConversations() async {
        isLoading = true
        defer { isLoading = false }
        do {
            let response: ApiResponse<[ChatConversation]> = try await apiClient.request(
                url: APIEndpoints.Chat.conversations,
                requiresAuth: true
            )
            conversations = response.data
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func openBookingChat(bookingId: UUID) async {
        isLoading = true
        defer { isLoading = false }
        do {
            let response: ApiResponse<ChatConversation> = try await apiClient.request(
                url: APIEndpoints.Chat.booking(bookingId),
                requiresAuth: true
            )
            conversation = response.data
            await refreshMessages()
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func refreshMessages() async {
        guard let conversation else { return }
        do {
            let response: ApiResponse<PagedChatMessages> = try await apiClient.request(
                url: APIEndpoints.Chat.messages(conversation.id),
                requiresAuth: true
            )
            messages = response.data.items
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func sendMessage() async {
        guard let conversation else { return }
        let body = draft.trimmingCharacters(in: .whitespacesAndNewlines)
        guard !body.isEmpty else { return }
        isSending = true
        defer { isSending = false }
        do {
            let _: ApiResponse<ChatMessage> = try await apiClient.request(
                url: APIEndpoints.Chat.sendMessage(conversation.id),
                method: .post,
                body: SendChatMessageRequest(body: body),
                requiresAuth: true
            )
            draft = ""
            await refreshMessages()
        } catch {
            errorMessage = error.localizedDescription
        }
    }
}
