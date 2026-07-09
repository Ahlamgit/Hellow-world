import Foundation

@MainActor
final class NotificationsViewModel: ObservableObject {
    enum Filter: String, CaseIterable {
        case all
        case unread
    }

    @Published var notifications: [AppNotification] = []
    @Published var totalCount = 0
    @Published var page = 1
    @Published var filter: Filter = .all
    @Published var isLoading = false
    @Published var errorMessage: String?

    private let apiClient: APIClient
    private let pageSize = 15

    init(apiClient: APIClient = .shared) {
        self.apiClient = apiClient
    }

    func load(page: Int? = nil) async {
        let targetPage = page ?? self.page
        isLoading = true
        errorMessage = nil
        defer { isLoading = false }

        do {
            let response: ApiResponse<PagedNotifications> = try await apiClient.request(
                url: APIEndpoints.Notifications.listURL(
                    unreadOnly: filter == .unread,
                    page: targetPage,
                    pageSize: pageSize
                ),
                method: .get,
                requiresAuth: true
            )
            notifications = response.data.items
            totalCount = response.data.totalCount
            self.page = targetPage
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func setFilter(_ filter: Filter) async {
        guard self.filter != filter else { return }
        self.filter = filter
        await load(page: 1)
    }

    func markRead(_ id: UUID) async {
        do {
            try await apiClient.requestVoid(
                url: APIEndpoints.Notifications.markRead(id),
                method: .post,
                requiresAuth: true
            )
            notifications = notifications.map { notification in
                guard notification.id == id else { return notification }
                return AppNotification(
                    id: notification.id,
                    titleEn: notification.titleEn,
                    titleAr: notification.titleAr,
                    messageEn: notification.messageEn,
                    messageAr: notification.messageAr,
                    notificationType: notification.notificationType,
                    referenceId: notification.referenceId,
                    isRead: true,
                    createdAt: notification.createdAt
                )
            }
        } catch {
            // Keep navigation even if mark-read fails
        }
    }

    func markAllRead() async {
        let unread = notifications.filter { !$0.isRead }
        guard !unread.isEmpty else { return }
        isLoading = true
        defer { isLoading = false }
        for notification in unread {
            await markRead(notification.id)
        }
        await load(page: page)
    }
}
