import SwiftUI

struct HomeView: View {
    @StateObject private var viewModel = HomeViewModel()

    var body: some View {
        NavigationStack {
            ScrollView {
                VStack(alignment: .leading, spacing: AppTheme.Spacing.lg) {
                    heroSection
                    howItWorksSection
                    categoriesSection
                }
                .padding(AppTheme.Spacing.lg)
            }
            .background(AppTheme.Colors.background)
            .navigationTitle(L10n.App.name)
            .task {
                await viewModel.loadCategories()
            }
            .refreshable {
                await viewModel.loadCategories()
            }
        }
    }

    private var heroSection: some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.md) {
            Text(L10n.Home.hero)
                .font(AppTheme.Typography.largeTitle())
                .foregroundStyle(AppTheme.Colors.textPrimary)

            Text(L10n.Home.subtitle)
                .font(AppTheme.Typography.body())
                .foregroundStyle(AppTheme.Colors.textSecondary)

            NavigationLink {
                ServicesView()
            } label: {
                Text(L10n.Home.browseServices)
                    .frame(maxWidth: .infinity)
            }
            .buttonStyle(PrimaryButtonStyle())
        }
        .cardStyle()
    }

    private var howItWorksSection: some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.md) {
            Text(L10n.Home.howItWorks)
                .font(AppTheme.Typography.title())

            StepRow(number: 1, text: L10n.Home.step1)
            StepRow(number: 2, text: L10n.Home.step2)
            StepRow(number: 3, text: L10n.Home.step3)
        }
        .cardStyle()
    }

    @ViewBuilder
    private var categoriesSection: some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.md) {
            Text(L10n.Home.categories)
                .font(AppTheme.Typography.title())

            if viewModel.isLoading && viewModel.categories.isEmpty {
                ProgressView(L10n.Common.loading)
                    .frame(maxWidth: .infinity)
            } else if let error = viewModel.errorMessage, viewModel.categories.isEmpty {
                ErrorBanner(message: error) {
                    Task { await viewModel.loadCategories() }
                }
            } else {
                LazyVGrid(
                    columns: [GridItem(.flexible()), GridItem(.flexible())],
                    spacing: AppTheme.Spacing.md
                ) {
                    ForEach(viewModel.categories) { category in
                        CategoryCard(category: category)
                    }
                }
            }
        }
    }
}

// MARK: - Supporting Views

private struct StepRow: View {
    let number: Int
    let text: String

    var body: some View {
        HStack(spacing: AppTheme.Spacing.md) {
            Text("\(number)")
                .font(.headline)
                .foregroundStyle(.white)
                .frame(width: 32, height: 32)
                .background(Circle().fill(AppTheme.Colors.accent))

            Text(text)
                .font(AppTheme.Typography.body())
                .foregroundStyle(AppTheme.Colors.textPrimary)

            Spacer()
        }
    }
}

private struct CategoryCard: View {
    let category: ServiceCategory

    var body: some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.sm) {
            Image(systemName: "square.grid.2x2.fill")
                .font(.title2)
                .foregroundStyle(AppTheme.Colors.accentSecondary)

            Text(category.localizedName())
                .font(AppTheme.Typography.headline())
                .foregroundStyle(AppTheme.Colors.textPrimary)
                .lineLimit(2)

            if let description = category.localizedDescription() {
                Text(description)
                    .font(AppTheme.Typography.caption())
                    .foregroundStyle(AppTheme.Colors.textSecondary)
                    .lineLimit(2)
            }
        }
        .frame(maxWidth: .infinity, alignment: .leading)
        .padding(AppTheme.Spacing.md)
        .background(AppTheme.Colors.cardBackground)
        .clipShape(RoundedRectangle(cornerRadius: AppTheme.Radius.lg))
    }
}

private struct ErrorBanner: View {
    let message: String
    let retry: () -> Void

    var body: some View {
        VStack(spacing: AppTheme.Spacing.sm) {
            Text(message)
                .font(AppTheme.Typography.caption())
                .foregroundStyle(AppTheme.Colors.error)

            Button(L10n.Common.retry, action: retry)
                .font(AppTheme.Typography.caption())
        }
        .frame(maxWidth: .infinity)
        .padding(AppTheme.Spacing.md)
    }
}

#Preview {
    HomeView()
}
