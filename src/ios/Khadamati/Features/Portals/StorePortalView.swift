import SwiftUI

struct StorePortalView: View {
    @StateObject private var viewModel = StorePortalViewModel()
    @EnvironmentObject private var appSession: AppSession

    @State private var storeName = ""
    @State private var description = ""
    @State private var isOpen = true
    @State private var openingTime = "08:00"
    @State private var closingTime = "22:00"
    @State private var productNameEn = ""
    @State private var productNameAr = ""
    @State private var productPrice = ""
    @State private var productStock = ""

    private var isArabic: Bool { appSession.preferredLanguage == "ar" }

    var body: some View {
        Form {
            Section(L10n.Portal.storeProfile) {
                TextField(L10n.Portal.storeName, text: $storeName)
                TextField(L10n.Portal.storeDescription, text: $description, axis: .vertical)
                    .lineLimit(3...6)
                TextField(L10n.Portal.opens, text: $openingTime)
                TextField(L10n.Portal.closes, text: $closingTime)
                Toggle(L10n.Portal.storeOpen, isOn: $isOpen)
                Button(L10n.Common.save) {
                    Task {
                        await viewModel.saveProfile(
                            storeName: storeName,
                            description: description,
                            isOpen: isOpen,
                            openingTime: openingTime,
                            closingTime: closingTime
                        )
                    }
                }
                .disabled(viewModel.isLoading || storeName.isEmpty)
            }

            if let profile = viewModel.profile {
                Section(L10n.Portal.products) {
                    ForEach(profile.products) { product in
                        HStack {
                            Text(isArabic ? product.nameAr : product.nameEn)
                            Spacer()
                            Text("\(product.price) USD · \(product.stockQuantity)")
                        }
                    }
                }
            }

            Section(L10n.Portal.addProduct) {
                TextField(L10n.Portal.productNameEn, text: $productNameEn)
                TextField(L10n.Portal.productNameAr, text: $productNameAr)
                TextField(L10n.Portal.price, text: $productPrice)
                    .keyboardType(.decimalPad)
                TextField(L10n.Portal.stock, text: $productStock)
                    .keyboardType(.numberPad)
                Button(L10n.Portal.addProduct) {
                    guard let price = Decimal(string: productPrice),
                          let stock = Int(productStock) else { return }
                    Task {
                        await viewModel.addProduct(
                            nameEn: productNameEn,
                            nameAr: productNameAr,
                            price: price,
                            stockQuantity: stock
                        )
                        productNameEn = ""
                        productNameAr = ""
                        productPrice = ""
                        productStock = ""
                    }
                }
                .disabled(viewModel.isLoading || productNameEn.isEmpty)
            }

            if viewModel.isLoading && viewModel.profile == nil {
                ProgressView(L10n.Common.loading)
            }
            if let message = viewModel.message {
                Section { Text(message).foregroundStyle(AppTheme.Colors.primary) }
            }
            if let error = viewModel.errorMessage {
                Section { Text(error).foregroundStyle(AppTheme.Colors.error) }
            }
        }
        .navigationTitle(L10n.Portal.storeTitle)
        .task {
            await viewModel.load()
            if let profile = viewModel.profile {
                storeName = profile.storeName
                description = profile.description ?? ""
                isOpen = profile.isOpen
                openingTime = profile.openingTime ?? "08:00"
                closingTime = profile.closingTime ?? "22:00"
            }
        }
        .onChange(of: viewModel.profile?.id) { _, _ in
            if let profile = viewModel.profile {
                storeName = profile.storeName
                description = profile.description ?? ""
                isOpen = profile.isOpen
                openingTime = profile.openingTime ?? "08:00"
                closingTime = profile.closingTime ?? "22:00"
            }
        }
    }
}
