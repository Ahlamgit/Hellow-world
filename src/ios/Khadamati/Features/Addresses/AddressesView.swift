import SwiftUI
import CoreLocation

struct AddressesView: View {
    @StateObject private var locationManager = LocationManager()
    @State private var addresses: [Address] = []
    @State private var label = "Home"
    @State private var street = ""
    @State private var city = ""
    @State private var country = "SA"
    @State private var isLoading = false
    @State private var isSaving = false

    var body: some View {
        Form {
            Section(L10n.Addresses.add) {
                TextField(L10n.Addresses.label, text: $label)
                TextField(L10n.Addresses.street, text: $street)
                TextField(L10n.Addresses.city, text: $city)
                TextField(L10n.Addresses.country, text: $country)
                Button(L10n.Addresses.useLocation) {
                    locationManager.requestLocation()
                }
                if let coordinate = locationManager.lastLocation {
                    Text(L10n.Addresses.coordinates(lat: coordinate.latitude, lng: coordinate.longitude))
                        .font(.caption)
                        .foregroundStyle(.secondary)
                }
                Button(L10n.Common.save) {
                    Task { await saveAddress() }
                }
                .disabled(isSaving || street.isEmpty || city.isEmpty)
            }

            Section(L10n.Addresses.title) {
                if isLoading {
                    ProgressView()
                } else if addresses.isEmpty {
                    Text(L10n.Addresses.empty)
                } else {
                    ForEach(addresses) { address in
                        VStack(alignment: .leading) {
                            Text(address.label).font(.headline)
                            Text("\(address.street), \(address.city)")
                            if let lat = address.latitude, let lng = address.longitude {
                                Text(L10n.Addresses.coordinates(lat: NSDecimalNumber(decimal: lat).doubleValue, lng: NSDecimalNumber(decimal: lng).doubleValue))
                                    .font(.caption)
                                    .foregroundStyle(.secondary)
                            }
                        }
                    }
                }
            }
        }
        .navigationTitle(L10n.Addresses.title)
        .task { await loadAddresses() }
    }

    private func loadAddresses() async {
        isLoading = true
        defer { isLoading = false }
        do {
            let response: ApiResponse<[Address]> = try await APIClient.shared.request(
                url: APIEndpoints.Users.addresses,
                requiresAuth: true
            )
            addresses = response.data
        } catch {}
    }

    private func saveAddress() async {
        isSaving = true
        defer { isSaving = false }
        let lat = locationManager.lastLocation.map { Decimal($0.latitude) }
        let lng = locationManager.lastLocation.map { Decimal($0.longitude) }
        let body = CreateAddressRequest(
            label: label,
            street: street,
            city: city,
            district: nil,
            postalCode: nil,
            country: country,
            latitude: lat,
            longitude: lng,
            isDefault: addresses.isEmpty
        )
        do {
            let _: ApiResponse<Address> = try await APIClient.shared.request(
                url: APIEndpoints.Users.addresses,
                method: .post,
                body: body,
                requiresAuth: true
            )
            street = ""
            city = ""
            await loadAddresses()
        } catch {}
    }
}
