import SwiftUI

struct BookingWizardView: View {
    let preselectedServiceId: UUID?

    @StateObject private var servicesViewModel = ServicesViewModel()
    @StateObject private var bookingViewModel = BookingViewModel()
    @StateObject private var locationManager = LocationManager()

    @State private var activeStep = 0
    @State private var selectedService: Service?
    @State private var selectedCraftsman: CraftsmanOption?
    @State private var selectedDate = Date()
    @State private var selectedSlot: TimeSlot?
    @State private var createdBooking: Booking?
    @State private var navigateToDetail = false

    init(preselectedServiceId: UUID? = nil) {
        self.preselectedServiceId = preselectedServiceId
    }

    var body: some View {
        VStack(spacing: AppTheme.Spacing.lg) {
            stepIndicator

            if let error = bookingViewModel.errorMessage {
                Text(error)
                    .font(AppTheme.Typography.caption())
                    .foregroundStyle(AppTheme.Colors.error)
                    .frame(maxWidth: .infinity, alignment: .leading)
            }

            Group {
                switch activeStep {
                case 0:
                    serviceStep
                case 1:
                    craftsmanStep
                case 2:
                    dateTimeStep
                default:
                    confirmStep
                }
            }
            .frame(maxHeight: .infinity, alignment: .top)
        }
        .padding(AppTheme.Spacing.lg)
        .background(AppTheme.Colors.background)
        .navigationTitle(L10n.Booking.title)
        .navigationBarTitleDisplayMode(.inline)
        .task {
            await servicesViewModel.loadInitialData()
            if let preselectedServiceId,
               let service = servicesViewModel.services.first(where: { $0.id == preselectedServiceId }) {
                selectedService = service
                activeStep = 1
                await bookingViewModel.loadCraftsmen(serviceId: service.id)
            }
        }
        .navigationDestination(isPresented: $navigateToDetail) {
            if let booking = createdBooking {
                BookingDetailView(booking: booking, viewModel: bookingViewModel)
            }
        }
    }

    private var stepIndicator: some View {
        HStack(spacing: AppTheme.Spacing.xs) {
            stepChip(L10n.Booking.stepService, isActive: activeStep == 0, isComplete: activeStep > 0)
            stepChip(L10n.Booking.stepCraftsman, isActive: activeStep == 1, isComplete: activeStep > 1)
            stepChip(L10n.Booking.stepDateTime, isActive: activeStep == 2, isComplete: activeStep > 2)
            stepChip(L10n.Booking.stepConfirm, isActive: activeStep == 3, isComplete: false)
        }
    }

    private func stepChip(_ title: String, isActive: Bool, isComplete: Bool) -> some View {
        Text(title)
            .font(AppTheme.Typography.caption())
            .fontWeight(isActive ? .semibold : .regular)
            .foregroundStyle(isActive || isComplete ? AppTheme.Colors.primary : AppTheme.Colors.textSecondary)
            .frame(maxWidth: .infinity)
            .padding(.vertical, AppTheme.Spacing.sm)
            .background(
                RoundedRectangle(cornerRadius: AppTheme.Radius.sm)
                    .stroke(
                        isActive || isComplete ? AppTheme.Colors.primary : AppTheme.Colors.textSecondary.opacity(0.3),
                        lineWidth: isActive ? 2 : 1
                    )
            )
    }

    private var serviceStep: some View {
        Group {
            if servicesViewModel.isLoading && servicesViewModel.services.isEmpty {
                ProgressView(L10n.Common.loading)
            } else {
                List(servicesViewModel.services) { service in
                    Button {
                        selectedService = service
                        selectedCraftsman = nil
                        selectedSlot = nil
                        activeStep = 1
                        Task { await bookingViewModel.loadCraftsmen(serviceId: service.id) }
                    } label: {
                        VStack(alignment: .leading, spacing: AppTheme.Spacing.xs) {
                            Text(service.localizedName())
                                .font(AppTheme.Typography.headline())
                            Text(service.formattedPrice)
                                .font(AppTheme.Typography.caption())
                                .foregroundStyle(AppTheme.Colors.accent)
                        }
                    }
                    .buttonStyle(.plain)
                }
                .listStyle(.plain)
            }
        }
    }

    private var craftsmanStep: some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.md) {
            Button(L10n.Booking.findNearby) {
                locationManager.requestLocation()
            }
            .buttonStyle(.borderedProminent)
            .onChange(of: locationManager.lastLocation?.latitude) { _, _ in
                guard let service = selectedService, let coordinate = locationManager.lastLocation else { return }
                Task {
                    await bookingViewModel.loadNearbyCraftsmen(
                        serviceId: service.id,
                        latitude: coordinate.latitude,
                        longitude: coordinate.longitude
                    )
                }
            }

            if bookingViewModel.isLoading && bookingViewModel.craftsmen.isEmpty {
                ProgressView(L10n.Common.loading)
            } else if bookingViewModel.craftsmen.isEmpty {
                Text(L10n.Booking.noCraftsmen)
                    .foregroundStyle(AppTheme.Colors.textSecondary)
            } else {
                List(bookingViewModel.craftsmen) { craftsman in
                    Button {
                        selectedCraftsman = craftsman
                        selectedSlot = nil
                        activeStep = 2
                    } label: {
                        HStack {
                            VStack(alignment: .leading, spacing: 4) {
                                Text("\(craftsman.firstName) \(craftsman.lastName)")
                                    .font(AppTheme.Typography.headline())
                                if let specialization = craftsman.specialization {
                                    Text(specialization)
                                        .font(AppTheme.Typography.caption())
                                        .foregroundStyle(AppTheme.Colors.textSecondary)
                                }
                                if let distance = craftsman.distanceKm {
                                    Text(L10n.Booking.distanceKm(distance))
                                        .font(AppTheme.Typography.caption())
                                        .foregroundStyle(AppTheme.Colors.primary)
                                }
                            }
                            Spacer()
                            VStack(alignment: .trailing, spacing: 4) {
                                Text("⭐ \(String(format: "%.1f", craftsman.rating))")
                                Text("\(craftsman.price) USD")
                                    .foregroundStyle(AppTheme.Colors.accent)
                            }
                        }
                    }
                    .buttonStyle(.plain)
                }
                .listStyle(.plain)
            }

            Button(L10n.Common.back) { activeStep = 0 }
                .buttonStyle(.bordered)
        }
    }

    private var dateTimeStep: some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.md) {
            DatePicker(
                L10n.Booking.selectDate,
                selection: $selectedDate,
                in: Date()...,
                displayedComponents: .date
            )
            .datePickerStyle(.graphical)

            Button(L10n.Booking.loadSlots) {
                guard let service = selectedService, let craftsman = selectedCraftsman else { return }
                Task { await bookingViewModel.loadSlots(craftsmanId: craftsman.id, serviceId: service.id, date: selectedDate) }
            }
            .buttonStyle(.borderedProminent)
            .disabled(bookingViewModel.isLoading)

            if bookingViewModel.isLoading {
                ProgressView()
            } else if bookingViewModel.slots.isEmpty {
                Text(L10n.Booking.noSlots)
                    .foregroundStyle(AppTheme.Colors.textSecondary)
            } else {
                LazyVGrid(columns: [GridItem(.adaptive(minimum: 100))], spacing: AppTheme.Spacing.sm) {
                    ForEach(bookingViewModel.slots) { slot in
                        let isSelected = selectedSlot?.start == slot.start
                        Button {
                            selectedSlot = slot
                        } label: {
                            Text(slot.start, style: .time)
                                .frame(maxWidth: .infinity)
                                .padding(.vertical, AppTheme.Spacing.sm)
                        }
                        .buttonStyle(.bordered)
                        .tint(isSelected ? AppTheme.Colors.primary : AppTheme.Colors.textSecondary)
                        .background(isSelected ? AppTheme.Colors.primary.opacity(0.15) : Color.clear)
                        .clipShape(RoundedRectangle(cornerRadius: AppTheme.Radius.sm))
                    }
                }
            }

            HStack {
                Button(L10n.Common.back) { activeStep = 1 }
                    .buttonStyle(.bordered)
                Button(L10n.Common.next) { activeStep = 3 }
                    .buttonStyle(.borderedProminent)
                    .disabled(selectedSlot == nil)
            }
        }
    }

    private var confirmStep: some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.md) {
            VStack(alignment: .leading, spacing: AppTheme.Spacing.sm) {
                Text(L10n.Booking.summary)
                    .font(AppTheme.Typography.headline())
                if let service = selectedService {
                    Text(service.localizedName())
                }
                if let craftsman = selectedCraftsman {
                    Text("\(craftsman.firstName) \(craftsman.lastName)")
                }
                if let slot = selectedSlot {
                    Text(slot.start, style: .date)
                    Text(slot.start, style: .time)
                }
                if let craftsman = selectedCraftsman {
                    Text("\(craftsman.price) USD")
                        .fontWeight(.bold)
                }
            }
            .cardStyle()

            HStack {
                Button(L10n.Common.back) { activeStep = 2 }
                    .buttonStyle(.bordered)
                    .disabled(bookingViewModel.isLoading)

                Button(L10n.Booking.confirmAndPay) {
                    guard let service = selectedService,
                          let craftsman = selectedCraftsman,
                          let slot = selectedSlot else { return }
                    Task {
                        if let booking = await bookingViewModel.createBooking(
                            serviceId: service.id,
                            craftsmanId: craftsman.id,
                            scheduledAt: slot.start
                        ) {
                            createdBooking = booking
                            navigateToDetail = true
                        }
                    }
                }
                .buttonStyle(.borderedProminent)
                .disabled(bookingViewModel.isLoading)
            }
        }
    }
}

#Preview {
    NavigationStack {
        BookingWizardView()
    }
}
