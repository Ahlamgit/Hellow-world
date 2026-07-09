import SwiftUI

struct ProfileEditView: View {
    @EnvironmentObject private var appSession: AppSession
    @Environment(\.dismiss) private var dismiss
    @State private var firstName = ""
    @State private var lastName = ""
    @State private var preferredLanguage = "ar"
    @State private var isSaving = false
    @State private var errorMessage: String?

    private let authService: AuthServiceProtocol = AuthService()

    var body: some View {
        Form {
            TextField(L10n.Auth.firstName, text: $firstName)
            TextField(L10n.Auth.lastName, text: $lastName)
            Picker(L10n.Profile.language, selection: $preferredLanguage) {
                Text(L10n.Profile.arabic).tag("ar")
                Text(L10n.Profile.english).tag("en")
            }
            if let errorMessage {
                Text(errorMessage).foregroundStyle(AppTheme.Colors.error)
            }
            Button(L10n.Common.save) {
                Task { await save() }
            }
            .disabled(isSaving || firstName.isEmpty || lastName.isEmpty)
        }
        .navigationTitle(L10n.Profile.editTitle)
        .onAppear {
            if let user = appSession.currentUser {
                firstName = user.firstName
                lastName = user.lastName
                preferredLanguage = user.preferredLanguage
            }
        }
    }

    private func save() async {
        isSaving = true
        errorMessage = nil
        defer { isSaving = false }
        do {
            let user = try await authService.updateProfile(
                firstName: firstName,
                lastName: lastName,
                preferredLanguage: preferredLanguage
            )
            appSession.updateUser(user)
            dismiss()
        } catch {
            errorMessage = error.localizedDescription
        }
    }
}
