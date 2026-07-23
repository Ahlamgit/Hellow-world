import SwiftUI

struct BookingDetailView: View {
    @State private var currentBooking: Booking
    @ObservedObject var viewModel: BookingViewModel
    @EnvironmentObject private var appSession: AppSession

    @State private var showCancelDialog = false
    @State private var showRejectDialog = false
    @State private var showNoShowDialog = false
    @State private var showRescheduleSheet = false
    @State private var showReviewSheet = false
    @State private var reasonText = ""
    @State private var rescheduleDate = Date()
    @State private var selectedSlot: TimeSlot?
    @State private var reviewRating = 5
    @State private var reviewComment = ""

    init(booking: Booking, viewModel: BookingViewModel) {
        _currentBooking = State(initialValue: booking)
        self.viewModel = viewModel
    }

    private var isCustomer: Bool { appSession.currentUser?.role == "Customer" }
    private var isCraftsman: Bool { appSession.currentUser?.role == "Craftsman" }
    private var cancellableStatuses: Set<String> { ["Pending", "AwaitingPayment", "Confirmed", "Rescheduled"] }

    var body: some View {
        ScrollView {
            VStack(alignment: .leading, spacing: 16) {
                Text(currentBooking.serviceName).font(.title2.bold())
                Label(currentBooking.status, systemImage: "info.circle")
                Text("Craftsman: \(currentBooking.craftsmanName)")
                Text("Price: \(currentBooking.estimatedPrice) SAR")

                NavigationLink {
                    BookingChatView(bookingId: currentBooking.id)
                } label: {
                    Text(L10n.Chat.open)
                }
                .buttonStyle(.bordered)

                if currentBooking.status == "AwaitingPayment", isCustomer {
                    Button(L10n.Booking.payNow) {
                        Task {
                            await viewModel.pay(bookingId: currentBooking.id)
                            await refreshBooking()
                        }
                    }
                    .buttonStyle(.borderedProminent)
                    Button("Refresh payment status") {
                        Task { await refreshBooking() }
                    }
                    .buttonStyle(.bordered)
                }

                if currentBooking.status == "PendingCraftsmanConfirmation", isCraftsman {
                    Button(L10n.Booking.accept) {
                        Task { await viewModel.accept(bookingId: currentBooking.id); await refreshBooking() }
                    }
                    .buttonStyle(.borderedProminent)
                    Button(L10n.Booking.reject, role: .destructive) { showRejectDialog = true }
                        .buttonStyle(.bordered)
                }

                if currentBooking.status == "Confirmed", isCraftsman {
                    Button(L10n.Booking.complete) {
                        Task { await viewModel.complete(bookingId: currentBooking.id); await refreshBooking() }
                    }
                    .buttonStyle(.borderedProminent)
                    Button(L10n.Booking.noShow, role: .destructive) { showNoShowDialog = true }
                        .buttonStyle(.bordered)
                }

                if cancellableStatuses.contains(currentBooking.status), isCustomer || isCraftsman {
                    Button(L10n.Booking.cancel, role: .destructive) { showCancelDialog = true }
                        .buttonStyle(.bordered)
                }

                if currentBooking.status == "Confirmed", isCustomer || isCraftsman {
                    Button(L10n.Booking.reschedule) { showRescheduleSheet = true }
                        .buttonStyle(.bordered)
                }

                if currentBooking.status == "Completed", isCustomer, currentBooking.customerRating == nil {
                    Button(L10n.Review.leave) { showReviewSheet = true }
                        .buttonStyle(.borderedProminent)
                }

                if let rating = currentBooking.customerRating {
                    VStack(alignment: .leading, spacing: 8) {
                        Text(L10n.Review.yourReview).font(.headline)
                        HStack(spacing: 4) {
                            ForEach(1...5, id: \.self) { star in
                                Image(systemName: star <= rating ? "star.fill" : "star")
                                    .foregroundStyle(.yellow)
                            }
                        }
                        if let review = currentBooking.customerReview, !review.isEmpty {
                            Text(review)
                                .font(.subheadline)
                                .foregroundStyle(AppTheme.Colors.textSecondary)
                        }
                    }
                    .padding(.top, 8)
                }
            }
            .frame(maxWidth: .infinity, alignment: .leading)
            .padding()
        }
        .navigationTitle(currentBooking.bookingReference)
        .task { await refreshBooking() }
        .alert(L10n.Booking.cancel, isPresented: $showCancelDialog) {
            TextField(L10n.Booking.cancelReason, text: $reasonText)
            Button(L10n.Booking.confirmCancel, role: .destructive) {
                Task {
                    await viewModel.cancel(bookingId: currentBooking.id, reason: reasonText)
                    reasonText = ""
                    await refreshBooking()
                }
            }
            Button(L10n.Common.cancel, role: .cancel) {}
        }
        .alert(L10n.Booking.reject, isPresented: $showRejectDialog) {
            TextField(L10n.Booking.cancelReason, text: $reasonText)
            Button(L10n.Booking.reject, role: .destructive) {
                Task {
                    await viewModel.reject(bookingId: currentBooking.id, reason: reasonText)
                    reasonText = ""
                    await refreshBooking()
                }
            }
            Button(L10n.Common.cancel, role: .cancel) {}
        }
        .alert(L10n.Booking.noShow, isPresented: $showNoShowDialog) {
            Button(L10n.Booking.confirmNoShow, role: .destructive) {
                Task { await viewModel.noShow(bookingId: currentBooking.id); await refreshBooking() }
            }
            Button(L10n.Common.cancel, role: .cancel) {}
        } message: {
            Text(L10n.Booking.noShowHint)
        }
        .sheet(isPresented: $showReviewSheet) {
            NavigationStack {
                Form {
                    Stepper(value: $reviewRating, in: 1...5) {
                        Text("\(L10n.Review.rating): \(reviewRating)")
                    }
                    TextField(L10n.Review.comment, text: $reviewComment, axis: .vertical)
                        .lineLimit(3...6)
                }
                .navigationTitle(L10n.Review.title)
                .toolbar {
                    ToolbarItem(placement: .cancellationAction) {
                        Button(L10n.Common.cancel) { showReviewSheet = false }
                    }
                    ToolbarItem(placement: .confirmationAction) {
                        Button(L10n.Review.submit) {
                            Task {
                                await viewModel.submitReview(
                                    bookingId: currentBooking.id,
                                    rating: reviewRating,
                                    review: reviewComment.isEmpty ? nil : reviewComment
                                )
                                reviewComment = ""
                                showReviewSheet = false
                                await refreshBooking()
                            }
                        }
                    }
                }
            }
        }
        .sheet(isPresented: $showRescheduleSheet) {
            NavigationStack {
                Form {
                    DatePicker(L10n.Booking.selectDate, selection: $rescheduleDate, displayedComponents: .date)
                    Button(L10n.Booking.loadSlots) {
                        Task {
                            await viewModel.loadSlots(
                                craftsmanId: currentBooking.craftsmanId,
                                serviceId: currentBooking.serviceId,
                                date: rescheduleDate
                            )
                        }
                    }
                    ForEach(viewModel.slots) { slot in
                        Button(slot.start) { selectedSlot = slot }
                    }
                    TextField(L10n.Booking.rescheduleReason, text: $reasonText)
                }
                .navigationTitle(L10n.Booking.reschedule)
                .toolbar {
                    ToolbarItem(placement: .cancellationAction) {
                        Button(L10n.Common.cancel) { showRescheduleSheet = false }
                    }
                    ToolbarItem(placement: .confirmationAction) {
                        Button(L10n.Booking.confirmReschedule) {
                            guard let slot = selectedSlot else { return }
                            Task {
                                await viewModel.reschedule(
                                    bookingId: currentBooking.id,
                                    newScheduledAt: slot.start,
                                    reason: reasonText.isEmpty ? nil : reasonText
                                )
                                reasonText = ""
                                selectedSlot = nil
                                showRescheduleSheet = false
                                await refreshBooking()
                            }
                        }
                        .disabled(selectedSlot == nil)
                    }
                }
            }
        }
    }

    private func refreshBooking() async {
        if let refreshed = await viewModel.loadBooking(id: currentBooking.id) {
            currentBooking = refreshed
        }
    }
}
