import SwiftUI

struct RegisterView: View {
    @Binding var showRegister: Bool
    @EnvironmentObject private var appSession: AppSession
    @StateObject private var viewModel = AuthViewModel()

    private let roles = [
        ("Customer", L10n.Auth.customer),
        ("Craftsman", L10n.Auth.craftsman),
        ("Store", L10n.Auth.store)
    ]

    var body: some View {
        ScrollView {
            VStack(spacing: AppTheme.Spacing.lg) {
                VStack(spacing: AppTheme.Spacing.md) {
                    KhadamatiTextField(
                        title: L10n.Auth.firstName,
                        text: $viewModel.firstName,
                        textContentType: .givenName
                    )

                    KhadamatiTextField(
                        title: L10n.Auth.lastName,
                        text: $viewModel.lastName,
                        textContentType: .familyName
                    )

                    KhadamatiTextField(
                        title: L10n.Auth.email,
                        text: $viewModel.email,
                        keyboardType: .emailAddress,
                        textContentType: .emailAddress
                    )

                    KhadamatiTextField(
                        title: L10n.Auth.phone,
                        text: $viewModel.phone,
                        keyboardType: .phonePad,
                        textContentType: .telephoneNumber
                    )

                    KhadamatiTextField(
                        title: L10n.Auth.password,
                        text: $viewModel.password,
                        isSecure: true,
                        textContentType: .newPassword
                    )

                    VStack(alignment: .leading, spacing: AppTheme.Spacing.xs) {
                        Text(L10n.Auth.role)
                            .font(AppTheme.Typography.caption())
                            .foregroundStyle(AppTheme.Colors.textSecondary)

                        Picker(L10n.Auth.role, selection: $viewModel.role) {
                            ForEach(roles, id: \.0) { role in
                                Text(role.1).tag(role.0)
                            }
                        }
                        .pickerStyle(.segmented)
                    }

                    VStack(alignment: .leading, spacing: AppTheme.Spacing.xs) {
                        Text(L10n.Profile.language)
                            .font(AppTheme.Typography.caption())
                            .foregroundStyle(AppTheme.Colors.textSecondary)

                        Picker(L10n.Profile.language, selection: $viewModel.preferredLanguage) {
                            Text(L10n.Profile.arabic).tag("ar")
                            Text(L10n.Profile.english).tag("en")
                        }
                        .pickerStyle(.segmented)
                    }
                }

                if let error = viewModel.errorMessage {
                    Text(error)
                        .font(AppTheme.Typography.caption())
                        .foregroundStyle(AppTheme.Colors.error)
                        .frame(maxWidth: .infinity, alignment: .leading)
                }

                Button {
                    Task { await viewModel.register(appSession: appSession) }
                } label: {
                    if viewModel.isLoading {
                        ProgressView()
                            .tint(.white)
                    } else {
                        Text(L10n.Auth.registerTitle)
                    }
                }
                .buttonStyle(PrimaryButtonStyle())
                .disabled(!viewModel.isRegisterValid || viewModel.isLoading)

                HStack(spacing: AppTheme.Spacing.xs) {
                    Text(L10n.Auth.hasAccount)
                        .foregroundStyle(AppTheme.Colors.textSecondary)
                    Button(L10n.Nav.login) {
                        showRegister = false
                    }
                    .fontWeight(.semibold)
                }
                .font(AppTheme.Typography.body())
            }
            .padding(AppTheme.Spacing.lg)
        }
        .background(AppTheme.Colors.background)
        .navigationTitle(L10n.Auth.registerTitle)
        .navigationBarTitleDisplayMode(.large)
    }
}

#Preview {
    NavigationStack {
        RegisterView(showRegister: .constant(true))
            .environmentObject(AppSession())
    }
}
