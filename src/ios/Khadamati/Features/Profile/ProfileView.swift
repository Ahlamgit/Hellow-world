import SwiftUI

struct ProfileView: View {
    @EnvironmentObject private var appSession: AppSession
    @StateObject private var viewModel = ProfileViewModel()

    var body: some View {
        NavigationStack {
            ScrollView {
                VStack(spacing: AppTheme.Spacing.lg) {
                    profileHeader
                    accountSection
                    if appSession.currentUser != nil {
                        quickLinksSection
                    }
                    languageSection
                    logoutButton
                }
                .padding(AppTheme.Spacing.lg)
            }
            .background(AppTheme.Colors.background)
            .navigationTitle(L10n.Profile.title)
            .task {
                await viewModel.refreshProfile(appSession: appSession)
            }
            .refreshable {
                await viewModel.refreshProfile(appSession: appSession)
            }
        }
    }

    @ViewBuilder
    private var profileHeader: some View {
        if let user = appSession.currentUser {
            VStack(spacing: AppTheme.Spacing.md) {
                Image(systemName: "person.circle.fill")
                    .font(.system(size: 72))
                    .foregroundStyle(AppTheme.Colors.accent)

                Text(user.fullName)
                    .font(AppTheme.Typography.title())
                    .foregroundStyle(AppTheme.Colors.textPrimary)

                Text(user.email)
                    .font(AppTheme.Typography.body())
                    .foregroundStyle(AppTheme.Colors.textSecondary)

                Text(user.displayRole)
                    .font(AppTheme.Typography.caption())
                    .foregroundStyle(.white)
                    .padding(.horizontal, AppTheme.Spacing.md)
                    .padding(.vertical, AppTheme.Spacing.xs)
                    .background(Capsule().fill(AppTheme.Colors.accentSecondary))
            }
            .frame(maxWidth: .infinity)
            .cardStyle()
        } else if viewModel.isLoading {
            ProgressView(L10n.Common.loading)
                .frame(maxWidth: .infinity)
                .padding(AppTheme.Spacing.xl)
        }
    }

    private var accountSection: some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.md) {
            Text(L10n.Profile.accountInfo)
                .font(AppTheme.Typography.title())

            if let user = appSession.currentUser {
                InfoRow(label: L10n.Auth.phone, value: user.phone)
                InfoRow(label: L10n.Auth.role, value: user.displayRole)

                if let status = user.status {
                    InfoRow(label: "Status", value: status)
                }
            }

            if let error = viewModel.errorMessage {
                Text(error)
                    .font(AppTheme.Typography.caption())
                    .foregroundStyle(AppTheme.Colors.error)
            }
        }
        .cardStyle()
    }

    private var quickLinksSection: some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.md) {
            NavigationLink {
                NotificationsView()
            } label: {
                Label(L10n.Notifications.title, systemImage: "bell")
                    .font(AppTheme.Typography.body())
            }

            if SubscriptionRoleSupport.isSubscriber(appSession.currentUser?.role) {
                NavigationLink {
                    SubscriptionPlansView()
                } label: {
                    Label(L10n.Subscription.nav, systemImage: "creditcard")
                        .font(AppTheme.Typography.body())
                }

                NavigationLink {
                    MySubscriptionView()
                } label: {
                    Label(L10n.Subscription.mySubscription, systemImage: "person.text.rectangle")
                        .font(AppTheme.Typography.body())
                }
            }
        }
        .cardStyle()
    }

    private var languageSection: some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.md) {
            Text(L10n.Profile.language)
                .font(AppTheme.Typography.title())

            Picker(L10n.Profile.language, selection: Binding(
                get: { appSession.preferredLanguage },
                set: { viewModel.updateLanguage($0, appSession: appSession) }
            )) {
                Text(L10n.Profile.arabic).tag("ar")
                Text(L10n.Profile.english).tag("en")
            }
            .pickerStyle(.segmented)
        }
        .cardStyle()
    }

    private var logoutButton: some View {
        Button {
            Task { await viewModel.logout(appSession: appSession) }
        } label: {
            if viewModel.isLoggingOut {
                ProgressView()
                    .tint(.white)
            } else {
                Text(L10n.Nav.logout)
            }
        }
        .buttonStyle(PrimaryButtonStyle())
        .tint(AppTheme.Colors.error)
        .disabled(viewModel.isLoggingOut)
    }
}

private struct InfoRow: View {
    let label: String
    let value: String

    var body: some View {
        HStack {
            Text(label)
                .font(AppTheme.Typography.caption())
                .foregroundStyle(AppTheme.Colors.textSecondary)
            Spacer()
            Text(value)
                .font(AppTheme.Typography.body())
                .foregroundStyle(AppTheme.Colors.textPrimary)
        }
    }
}

#Preview {
    ProfileView()
        .environmentObject(AppSession())
}
