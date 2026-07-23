import SwiftUI

struct ForgotPasswordView: View {
    @StateObject private var viewModel = PasswordRecoveryViewModel()
    @Environment(\.dismiss) private var dismiss

    var body: some View {
        ScrollView {
            VStack(spacing: AppTheme.Spacing.lg) {
                Text(L10n.Auth.forgotPasswordTitle)
                    .font(AppTheme.Typography.largeTitle())
                Text(L10n.Auth.forgotPasswordHint)
                    .font(AppTheme.Typography.body())
                    .foregroundStyle(AppTheme.Colors.textSecondary)

                KhadamatiTextField(title: L10n.Auth.email, text: $viewModel.email, keyboardType: .emailAddress)

                if let success = viewModel.successMessage {
                    Text(success).foregroundStyle(AppTheme.Colors.primary)
                }
                if let error = viewModel.errorMessage {
                    Text(error).foregroundStyle(AppTheme.Colors.error)
                }

                Button {
                    Task { await viewModel.forgotPassword() }
                } label: {
                    if viewModel.isLoading { ProgressView().tint(.white) }
                    else { Text(L10n.Auth.sendResetLink) }
                }
                .buttonStyle(PrimaryButtonStyle())
                .disabled(viewModel.isLoading || viewModel.email.isEmpty)
            }
            .padding(AppTheme.Spacing.lg)
        }
        .navigationTitle(L10n.Auth.forgotPasswordTitle)
    }
}

struct ResetPasswordView: View {
    @StateObject private var viewModel = PasswordRecoveryViewModel()
    @Environment(\.dismiss) private var dismiss

    var body: some View {
        ScrollView {
            VStack(spacing: AppTheme.Spacing.lg) {
                Text(L10n.Auth.resetPasswordTitle)
                    .font(AppTheme.Typography.largeTitle())

                KhadamatiTextField(title: L10n.Auth.resetToken, text: $viewModel.token)
                KhadamatiTextField(title: L10n.Auth.newPassword, text: $viewModel.newPassword, isSecure: true)
                KhadamatiTextField(title: L10n.Auth.confirmPassword, text: $viewModel.confirmPassword, isSecure: true)

                if viewModel.resetSuccess {
                    Text(L10n.Auth.resetSuccess).foregroundStyle(AppTheme.Colors.primary)
                }
                if let error = viewModel.errorMessage {
                    Text(error).foregroundStyle(AppTheme.Colors.error)
                }

                Button {
                    Task { await viewModel.resetPassword() }
                } label: {
                    if viewModel.isLoading { ProgressView().tint(.white) }
                    else { Text(L10n.Auth.resetPassword) }
                }
                .buttonStyle(PrimaryButtonStyle())
                .disabled(viewModel.isLoading || !viewModel.isResetValid)
            }
            .padding(AppTheme.Spacing.lg)
        }
        .navigationTitle(L10n.Auth.resetPasswordTitle)
    }
}

@MainActor
final class PasswordRecoveryViewModel: ObservableObject {
    @Published var email = ""
    @Published var token = ""
    @Published var newPassword = ""
    @Published var confirmPassword = ""
    @Published var isLoading = false
    @Published var errorMessage: String?
    @Published var successMessage: String?
    @Published var resetSuccess = false

    private let authService: AuthServiceProtocol

    init(authService: AuthServiceProtocol? = nil) {
        self.authService = authService ?? AuthService()
    }

    var isResetValid: Bool {
        !token.isEmpty && newPassword.count >= 6 && newPassword == confirmPassword
    }

    func forgotPassword() async {
        isLoading = true
        errorMessage = nil
        successMessage = nil
        defer { isLoading = false }
        do {
            successMessage = try await authService.forgotPassword(email: email.trimmingCharacters(in: .whitespaces))
        } catch {
            errorMessage = error.localizedDescription
        }
    }

    func resetPassword() async {
        guard newPassword == confirmPassword else {
            errorMessage = L10n.Auth.passwordMismatch
            return
        }
        isLoading = true
        errorMessage = nil
        defer { isLoading = false }
        do {
            _ = try await authService.resetPassword(token: token, newPassword: newPassword, confirmPassword: confirmPassword)
            resetSuccess = true
        } catch {
            errorMessage = error.localizedDescription
        }
    }
}

@MainActor
final class ChangePasswordViewModel: ObservableObject {
    @Published var currentPassword = ""
    @Published var newPassword = ""
    @Published var confirmPassword = ""
    @Published var isLoading = false
    @Published var errorMessage: String?
    @Published var successMessage: String?

    private let authService: AuthServiceProtocol

    init(authService: AuthServiceProtocol? = nil) {
        self.authService = authService ?? AuthService()
    }

    var isValid: Bool {
        !currentPassword.isEmpty && newPassword.count >= 8 && newPassword == confirmPassword
    }

    func changePassword() async {
        guard newPassword == confirmPassword else {
            errorMessage = L10n.Auth.passwordMismatch
            return
        }
        isLoading = true
        errorMessage = nil
        successMessage = nil
        defer { isLoading = false }
        do {
            successMessage = try await authService.changePassword(
                currentPassword: currentPassword,
                newPassword: newPassword,
                confirmPassword: confirmPassword
            )
            currentPassword = ""
            newPassword = ""
            confirmPassword = ""
        } catch {
            errorMessage = error.localizedDescription
        }
    }
}

struct ChangePasswordView: View {
    @StateObject private var viewModel = ChangePasswordViewModel()

    var body: some View {
        ScrollView {
            VStack(spacing: AppTheme.Spacing.lg) {
                Text(L10n.Auth.changePasswordTitle)
                    .font(AppTheme.Typography.largeTitle())

                Text(L10n.Auth.changePasswordHint)
                    .font(AppTheme.Typography.body())
                    .foregroundStyle(AppTheme.Colors.textSecondary)

                KhadamatiTextField(title: L10n.Auth.currentPassword, text: $viewModel.currentPassword, isSecure: true)
                KhadamatiTextField(title: L10n.Auth.newPassword, text: $viewModel.newPassword, isSecure: true)
                KhadamatiTextField(title: L10n.Auth.confirmPassword, text: $viewModel.confirmPassword, isSecure: true)

                if let success = viewModel.successMessage {
                    Text(success).foregroundStyle(AppTheme.Colors.primary)
                }
                if let error = viewModel.errorMessage {
                    Text(error).foregroundStyle(AppTheme.Colors.error)
                }

                Button {
                    Task { await viewModel.changePassword() }
                } label: {
                    if viewModel.isLoading { ProgressView().tint(.white) }
                    else { Text(L10n.Auth.changePassword) }
                }
                .buttonStyle(PrimaryButtonStyle())
                .disabled(viewModel.isLoading || !viewModel.isValid)
            }
            .padding(AppTheme.Spacing.lg)
        }
        .navigationTitle(L10n.Auth.changePasswordTitle)
    }
}
