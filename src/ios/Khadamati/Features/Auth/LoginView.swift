import SwiftUI

struct LoginView: View {
    @Binding var showRegister: Bool
    @EnvironmentObject private var appSession: AppSession
    @StateObject private var viewModel = AuthViewModel()

    var body: some View {
        ScrollView {
            VStack(spacing: AppTheme.Spacing.lg) {
                header

                VStack(spacing: AppTheme.Spacing.md) {
                    KhadamatiTextField(
                        title: L10n.Auth.email,
                        text: $viewModel.email,
                        keyboardType: .emailAddress,
                        textContentType: .emailAddress
                    )

                    KhadamatiTextField(
                        title: L10n.Auth.password,
                        text: $viewModel.password,
                        isSecure: true,
                        textContentType: .password
                    )
                }

                if let error = viewModel.errorMessage {
                    Text(error)
                        .font(AppTheme.Typography.caption())
                        .foregroundStyle(AppTheme.Colors.error)
                        .frame(maxWidth: .infinity, alignment: .leading)
                }

                Button {
                    Task { await viewModel.login(appSession: appSession) }
                } label: {
                    if viewModel.isLoading {
                        ProgressView()
                            .tint(.white)
                    } else {
                        Text(L10n.Auth.loginTitle)
                    }
                }
                .buttonStyle(PrimaryButtonStyle())
                .disabled(!viewModel.isLoginValid || viewModel.isLoading)

                HStack(spacing: AppTheme.Spacing.xs) {
                    Text(L10n.Auth.noAccount)
                        .foregroundStyle(AppTheme.Colors.textSecondary)
                    Button(L10n.Nav.register) {
                        showRegister = true
                    }
                    .fontWeight(.semibold)
                }
                .font(AppTheme.Typography.body())
            }
            .padding(AppTheme.Spacing.lg)
        }
        .background(AppTheme.Colors.background)
        .navigationTitle(L10n.Auth.loginTitle)
        .navigationBarTitleDisplayMode(.large)
    }

    private var header: some View {
        VStack(spacing: AppTheme.Spacing.sm) {
            Image(systemName: "wrench.and.screwdriver.fill")
                .font(.system(size: 48))
                .foregroundStyle(AppTheme.Colors.accent)

            Text(L10n.App.name)
                .font(AppTheme.Typography.largeTitle())
                .foregroundStyle(AppTheme.Colors.textPrimary)

            Text(L10n.App.tagline)
                .font(AppTheme.Typography.body())
                .foregroundStyle(AppTheme.Colors.textSecondary)
                .multilineTextAlignment(.center)
        }
        .padding(.vertical, AppTheme.Spacing.lg)
    }
}

#Preview {
    NavigationStack {
        LoginView(showRegister: .constant(false))
            .environmentObject(AppSession())
    }
}
