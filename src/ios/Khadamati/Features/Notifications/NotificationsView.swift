import SwiftUI

struct NotificationsView: View {
    @EnvironmentObject private var appSession: AppSession
    @StateObject private var viewModel = NotificationsViewModel()
    @StateObject private var bookingViewModel = BookingViewModel()
    @State private var selectedBookingId: BookingNavTarget?

    private var isArabic: Bool { appSession.preferredLanguage == "ar" }

    var body: some View {
        VStack(spacing: 0) {
            Picker("", selection: Binding(
                get: { viewModel.filter },
                set: { filter in Task { await viewModel.setFilter(filter) } }
            )) {
                Text(L10n.Notifications.all).tag(NotificationsViewModel.Filter.all)
                Text(L10n.Notifications.unread).tag(NotificationsViewModel.Filter.unread)
            }
            .pickerStyle(.segmented)
            .padding()

            Group {
                if viewModel.isLoading && viewModel.notifications.isEmpty {
                    ProgressView()
                        .frame(maxWidth: .infinity, maxHeight: .infinity)
                } else if viewModel.notifications.isEmpty {
                    ContentUnavailableView(
                        L10n.Notifications.title,
                        systemImage: "bell.slash",
                        description: Text(
                            viewModel.filter == .unread
                                ? L10n.Notifications.emptyUnread
                                : L10n.Notifications.empty
                        )
                    )
                } else {
                    List(viewModel.notifications) { notification in
                        Button {
                            Task {
                                await viewModel.markRead(notification.id)
                                if let bookingId = notification.bookingId {
                                    selectedBookingId = BookingNavTarget(id: bookingId)
                                }
                            }
                        } label: {
                            NotificationRow(notification: notification, isArabic: isArabic)
                        }
                        .buttonStyle(.plain)
                    }
                    .listStyle(.plain)
                }
            }
        }
        .navigationTitle(L10n.Notifications.title)
        .toolbar {
            if viewModel.notifications.contains(where: { !$0.isRead }) {
                ToolbarItem(placement: .topBarTrailing) {
                    Button(L10n.Notifications.markAllRead) {
                        Task { await viewModel.markAllRead() }
                    }
                    .disabled(viewModel.isLoading)
                }
            }
        }
        .navigationDestination(item: $selectedBookingId) { target in
            BookingLoaderView(bookingId: target.id, viewModel: bookingViewModel)
        }
        .task { await viewModel.load() }
        .refreshable { await viewModel.load() }
    }
}

private struct NotificationRow: View {
    let notification: AppNotification
    let isArabic: Bool

    var body: some View {
        VStack(alignment: .leading, spacing: 6) {
            HStack {
                Text(notification.title(isArabic: isArabic))
                    .font(.headline)
                    .fontWeight(notification.isRead ? .medium : .bold)
                Spacer()
                Text(notification.createdAt.formatted(date: .abbreviated, time: .shortened))
                    .font(.caption)
                    .foregroundStyle(.secondary)
            }
            Text(notification.message(isArabic: isArabic))
                .font(.subheadline)
                .foregroundStyle(.secondary)
            HStack(spacing: 8) {
                Text(notification.notificationType)
                    .font(.caption)
                    .foregroundStyle(.secondary)
                if !notification.isRead {
                    Text(L10n.Notifications.unreadBadge)
                        .font(.caption)
                        .foregroundStyle(AppTheme.Colors.primary)
                }
                if notification.bookingId != nil {
                    Text(L10n.Notifications.viewBooking)
                        .font(.caption)
                        .foregroundStyle(AppTheme.Colors.primary)
                }
            }
        }
        .padding(.vertical, 4)
        .listRowBackground(notification.isRead ? nil : AppTheme.Colors.background)
    }
}

private struct BookingNavTarget: Identifiable, Hashable {
    let id: UUID
}

private struct BookingLoaderView: View {
    let bookingId: UUID
    @ObservedObject var viewModel: BookingViewModel
    @EnvironmentObject private var appSession: AppSession
    @State private var booking: Booking?

    var body: some View {
        Group {
            if let booking {
                BookingDetailView(booking: booking, viewModel: viewModel)
            } else if viewModel.isLoading {
                ProgressView()
            } else {
                ContentUnavailableView("Booking", systemImage: "calendar.badge.exclamationmark")
            }
        }
        .task {
            booking = await viewModel.loadBooking(id: bookingId)
        }
    }
}

#Preview {
    NavigationStack {
        NotificationsView()
            .environmentObject(AppSession())
    }
}
