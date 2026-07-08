import SwiftUI

struct ServicesView: View {
    @StateObject private var viewModel = ServicesViewModel()

    var body: some View {
        NavigationStack {
            VStack(spacing: 0) {
                categoryFilter
                servicesList
            }
            .background(AppTheme.Colors.background)
            .navigationTitle(L10n.Services.title)
            .task {
                await viewModel.loadInitialData()
            }
            .refreshable {
                await viewModel.loadInitialData()
            }
        }
    }

    private var categoryFilter: some View {
        ScrollView(.horizontal, showsIndicators: false) {
            HStack(spacing: AppTheme.Spacing.sm) {
                CategoryChip(
                    title: L10n.Services.allCategories,
                    isSelected: viewModel.selectedCategoryId == nil
                ) {
                    Task { await viewModel.selectCategory(nil) }
                }

                ForEach(viewModel.categories) { category in
                    CategoryChip(
                        title: category.localizedName(),
                        isSelected: viewModel.selectedCategoryId == category.id
                    ) {
                        Task { await viewModel.selectCategory(category.id) }
                    }
                }
            }
            .padding(.horizontal, AppTheme.Spacing.lg)
            .padding(.vertical, AppTheme.Spacing.md)
        }
        .background(AppTheme.Colors.cardBackground)
    }

    @ViewBuilder
    private var servicesList: some View {
        if viewModel.isLoading && viewModel.services.isEmpty {
            Spacer()
            ProgressView(L10n.Common.loading)
            Spacer()
        } else if let error = viewModel.errorMessage, viewModel.services.isEmpty {
            Spacer()
            VStack(spacing: AppTheme.Spacing.md) {
                Text(error)
                    .font(AppTheme.Typography.caption())
                    .foregroundStyle(AppTheme.Colors.error)
                Button(L10n.Common.retry) {
                    Task { await viewModel.loadServices() }
                }
            }
            Spacer()
        } else if viewModel.services.isEmpty {
            Spacer()
            Text(L10n.Services.title)
                .foregroundStyle(AppTheme.Colors.textSecondary)
            Spacer()
        } else {
            List(viewModel.services) { service in
                NavigationLink {
                    BookingWizardView(preselectedServiceId: service.id)
                } label: {
                    ServiceRow(service: service)
                }
                .listRowSeparator(.hidden)
                    .listRowBackground(Color.clear)
                    .listRowInsets(EdgeInsets(
                        top: AppTheme.Spacing.xs,
                        leading: AppTheme.Spacing.lg,
                        bottom: AppTheme.Spacing.xs,
                        trailing: AppTheme.Spacing.lg
                    ))
            }
            .listStyle(.plain)
        }
    }
}

// MARK: - Supporting Views

private struct CategoryChip: View {
    let title: String
    let isSelected: Bool
    let action: () -> Void

    var body: some View {
        Button(action: action) {
            Text(title)
                .font(AppTheme.Typography.caption())
                .fontWeight(isSelected ? .semibold : .regular)
                .foregroundStyle(isSelected ? .white : AppTheme.Colors.textPrimary)
                .padding(.horizontal, AppTheme.Spacing.md)
                .padding(.vertical, AppTheme.Spacing.sm)
                .background(
                    Capsule()
                        .fill(isSelected ? AppTheme.Colors.accent : AppTheme.Colors.background)
                )
        }
        .buttonStyle(.plain)
    }
}

private struct ServiceRow: View {
    let service: Service

    var body: some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.sm) {
            HStack(alignment: .top) {
                VStack(alignment: .leading, spacing: AppTheme.Spacing.xs) {
                    Text(service.localizedName())
                        .font(AppTheme.Typography.headline())
                        .foregroundStyle(AppTheme.Colors.textPrimary)

                    if let description = service.localizedDescription() {
                        Text(description)
                            .font(AppTheme.Typography.caption())
                            .foregroundStyle(AppTheme.Colors.textSecondary)
                            .lineLimit(2)
                    }
                }

                Spacer()

                Image(systemName: "chevron.right")
                    .font(.caption)
                    .foregroundStyle(AppTheme.Colors.textSecondary)
            }

            HStack(spacing: AppTheme.Spacing.lg) {
                Label("\(L10n.Services.price) \(service.formattedPrice)", systemImage: "tag.fill")
                Label(
                    "\(service.estimatedDurationMinutes) \(L10n.Common.minutes)",
                    systemImage: "clock.fill"
                )
            }
            .font(AppTheme.Typography.caption())
            .foregroundStyle(AppTheme.Colors.accentSecondary)
        }
        .cardStyle()
    }
}

#Preview {
    ServicesView()
}
