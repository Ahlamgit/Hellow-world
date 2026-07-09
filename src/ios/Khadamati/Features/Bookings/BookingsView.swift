import SwiftUI

struct MyBookingsView: View {
    @StateObject private var viewModel = BookingViewModel()

    var body: some View {
        NavigationStack {
            Group {
                if viewModel.isLoading {
                    ProgressView()
                } else if viewModel.bookings.isEmpty {
                    ContentUnavailableView("No Bookings", systemImage: "calendar")
                } else {
                    List(viewModel.bookings) { booking in
                        NavigationLink(value: booking) {
                            VStack(alignment: .leading, spacing: 4) {
                                Text(booking.serviceName).font(.headline)
                                Text(booking.bookingReference).font(.caption).foregroundStyle(.secondary)
                                Text(booking.status).font(.caption).foregroundStyle(AppTheme.Colors.primary)
                            }
                        }
                    }
                }
            }
            .navigationTitle(L10n.Booking.myBookings)
            .toolbar {
                ToolbarItem(placement: .topBarTrailing) {
                    NavigationLink {
                        BookingWizardView()
                    } label: {
                        Image(systemName: "plus")
                    }
                    .accessibilityLabel(L10n.Booking.newBooking)
                }
            }
            .navigationDestination(for: Booking.self) { booking in
                BookingDetailView(booking: booking, viewModel: viewModel)
            }
            .task { await viewModel.loadBookings() }
        }
    }
}

extension Booking: Hashable {
    static func == (lhs: Booking, rhs: Booking) -> Bool { lhs.id == rhs.id }
    func hash(into hasher: inout Hasher) { hasher.combine(id) }
}
