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
            .navigationTitle("My Bookings")
            .navigationDestination(for: Booking.self) { booking in
                BookingDetailView(booking: booking, viewModel: viewModel)
            }
            .task { await viewModel.loadBookings() }
        }
    }
}

struct BookingDetailView: View {
    let booking: Booking
    @ObservedObject var viewModel: BookingViewModel
    @EnvironmentObject private var appSession: AppSession

    var body: some View {
        VStack(alignment: .leading, spacing: 16) {
            Text(booking.serviceName).font(.title2.bold())
            Label(booking.status, systemImage: "info.circle")
            Text("Craftsman: \(booking.craftsmanName)")
            Text("Price: \(booking.estimatedPrice) SAR")
            Spacer()
            if booking.status == "AwaitingPayment", appSession.currentUser?.role == "Customer" {
                Button("Pay Now") { Task { await viewModel.pay(bookingId: booking.id) } }
                    .buttonStyle(.borderedProminent)
            }
            if booking.status == "PendingCraftsmanConfirmation", appSession.currentUser?.role == "Craftsman" {
                HStack {
                    Button("Accept") { Task { await viewModel.accept(bookingId: booking.id) } }
                        .buttonStyle(.borderedProminent)
                    Button("Reject", role: .destructive) { Task { await viewModel.reject(bookingId: booking.id, reason: "Unavailable") } }
                }
            }
        }
        .padding()
        .navigationTitle(booking.bookingReference)
    }
}

extension Booking: Hashable {
    static func == (lhs: Booking, rhs: Booking) -> Bool { lhs.id == rhs.id }
    func hash(into hasher: inout Hasher) { hasher.combine(id) }
}
