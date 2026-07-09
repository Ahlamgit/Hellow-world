import SwiftUI

struct CraftsmanPortalView: View {
    @StateObject private var viewModel = CraftsmanPortalViewModel()
    @EnvironmentObject private var appSession: AppSession

    @State private var specialization = ""
    @State private var yearsOfExperience = 0
    @State private var serviceRadiusKm = 25.0
    @State private var isAvailable = true
    @State private var selectedServiceId: UUID?
    @State private var customPrice = ""

    private var isArabic: Bool { appSession.preferredLanguage == "ar" }

    var body: some View {
        Form {
            if let profile = viewModel.profile {
                Section(L10n.Portal.profile) {
                    Text("⭐ \(String(format: "%.1f", profile.rating)) · \(profile.completedJobs) \(L10n.Portal.jobs)")
                    TextField(L10n.Portal.specialization, text: $specialization)
                    Stepper(value: $yearsOfExperience, in: 0...50) {
                        Text("\(L10n.Portal.years): \(yearsOfExperience)")
                    }
                    Stepper(value: $serviceRadiusKm, in: 1...100, step: 1) {
                        Text("\(L10n.Portal.radius): \(Int(serviceRadiusKm)) km")
                    }
                    Toggle(L10n.Portal.available, isOn: $isAvailable)
                    Button(L10n.Common.save) {
                        Task {
                            await viewModel.saveProfile(
                                specialization: specialization,
                                yearsOfExperience: yearsOfExperience,
                                isAvailable: isAvailable,
                                serviceRadiusKm: serviceRadiusKm
                            )
                        }
                    }
                    .disabled(viewModel.isLoading)
                }

                Section(L10n.Portal.offeredServices) {
                    ForEach(profile.services) { service in
                        HStack {
                            Text(isArabic ? service.serviceNameAr : service.serviceNameEn)
                            Spacer()
                            Text("\(service.customPrice) SAR")
                        }
                    }
                }

                Section(L10n.Portal.addService) {
                    ForEach(viewModel.services) { service in
                        Button {
                            selectedServiceId = service.id
                        } label: {
                            HStack {
                                Text(service.localizedName(locale: Locale(identifier: isArabic ? "ar" : "en")))
                                Spacer()
                                if selectedServiceId == service.id {
                                    Image(systemName: "checkmark.circle.fill")
                                }
                            }
                        }
                    }
                    TextField(L10n.Portal.price, text: $customPrice)
                        .keyboardType(.decimalPad)
                    Button(L10n.Portal.addService) {
                        guard let serviceId = selectedServiceId,
                              let price = Decimal(string: customPrice) else { return }
                        Task { await viewModel.addService(serviceId: serviceId, customPrice: price) }
                    }
                    .disabled(viewModel.isLoading || selectedServiceId == nil || customPrice.isEmpty)
                }
            } else if viewModel.isLoading {
                ProgressView(L10n.Common.loading)
            }

            if let message = viewModel.message {
                Section { Text(message).foregroundStyle(AppTheme.Colors.primary) }
            }
            if let error = viewModel.errorMessage {
                Section { Text(error).foregroundStyle(AppTheme.Colors.error) }
            }
        }
        .navigationTitle(L10n.Portal.craftsmanTitle)
        .task {
            await viewModel.load()
            if let profile = viewModel.profile {
                specialization = profile.specialization ?? ""
                yearsOfExperience = profile.yearsOfExperience
                serviceRadiusKm = profile.serviceRadiusKm ?? 25
                isAvailable = profile.isAvailable
            }
        }
        .onChange(of: viewModel.profile?.id) { _, _ in
            if let profile = viewModel.profile {
                specialization = profile.specialization ?? ""
                yearsOfExperience = profile.yearsOfExperience
                serviceRadiusKm = profile.serviceRadiusKm ?? 25
                isAvailable = profile.isAvailable
            }
        }
    }
}
