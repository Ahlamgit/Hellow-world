import SwiftUI

struct SubscriptionPlansView: View {
    @EnvironmentObject private var appSession: AppSession
    @StateObject private var viewModel = SubscriptionViewModel()

    private var isArabic: Bool { appSession.preferredLanguage == "ar" }
    private var targetRole: String? {
        SubscriptionRoleSupport.targetRole(for: appSession.currentUser?.role)
    }

    var body: some View {
        Group {
            if viewModel.isLoading && viewModel.plans.isEmpty {
                ProgressView()
            } else if viewModel.plans.isEmpty {
                ContentUnavailableView(
                    L10n.Subscription.plansTitle,
                    systemImage: "creditcard",
                    description: Text(L10n.Subscription.noPlans)
                )
            } else {
                ScrollView {
                    VStack(spacing: AppTheme.Spacing.md) {
                        if let current = viewModel.current {
                            activePlanBanner(current)
                        }
                        ForEach(viewModel.plans) { plan in
                            PlanCard(
                                plan: plan,
                                isArabic: isArabic,
                                hasActiveSubscription: viewModel.current != nil
                            )
                        }
                    }
                    .padding(AppTheme.Spacing.lg)
                }
            }
        }
        .navigationTitle(L10n.Subscription.plansTitle)
        .task { await viewModel.loadPlans(targetRole: targetRole) }
        .refreshable { await viewModel.loadPlans(targetRole: targetRole) }
    }

    @ViewBuilder
    private func activePlanBanner(_ current: UserSubscription) -> some View {
        HStack {
            Text(L10n.Subscription.activePlan(
                plan: current.planName(isArabic: isArabic),
                status: current.status
            ))
            .font(AppTheme.Typography.body())
            Spacer()
            NavigationLink(L10n.Subscription.manage) {
                MySubscriptionView()
            }
            .font(AppTheme.Typography.caption())
        }
        .cardStyle()
    }
}

struct SubscribePlanView: View {
    let planId: UUID
    @StateObject private var viewModel = SubscriptionViewModel()
    @EnvironmentObject private var appSession: AppSession
    @Environment(\.dismiss) private var dismiss

    private var isArabic: Bool { appSession.preferredLanguage == "ar" }

    var body: some View {
        Group {
            if viewModel.isLoading || viewModel.selectedPlan == nil {
                ProgressView()
            } else if let plan = viewModel.selectedPlan {
                ScrollView {
                    VStack(alignment: .leading, spacing: AppTheme.Spacing.md) {
                        Text(plan.name(isArabic: isArabic))
                            .font(AppTheme.Typography.title())
                        if let description = plan.description(isArabic: isArabic), !description.isEmpty {
                            Text(description)
                                .font(AppTheme.Typography.body())
                                .foregroundStyle(AppTheme.Colors.textSecondary)
                        }

                        Text(L10n.Subscription.billingCycle)
                            .font(AppTheme.Typography.title())

                        let activeOptions = plan.billingOptions.filter { $0.isActive && $0.optionId != nil }
                        ForEach(activeOptions, id: \.listId) { option in
                            Button {
                                viewModel.selectedBillingOptionId = option.optionId
                            } label: {
                                HStack {
                                    Text("\(cycleLabel(option.cycle)) — \(option.price) \(plan.currency)")
                                    Spacer()
                                    if viewModel.selectedBillingOptionId == option.optionId {
                                        Image(systemName: "checkmark.circle.fill")
                                    }
                                }
                            }
                            .buttonStyle(.bordered)
                            .tint(viewModel.selectedBillingOptionId == option.optionId ? AppTheme.Colors.primary : .secondary)
                        }

                        Toggle(L10n.Subscription.autoRenew, isOn: $viewModel.autoRenew)

                        HStack {
                            Button(L10n.Common.back) { dismiss() }
                                .buttonStyle(.bordered)
                            Button(L10n.Subscription.confirmSubscribe) {
                                Task {
                                    if await viewModel.subscribe() {
                                        dismiss()
                                    }
                                }
                            }
                            .buttonStyle(PrimaryButtonStyle())
                            .disabled(viewModel.isSubmitting || viewModel.selectedBillingOptionId == nil)
                        }
                    }
                    .padding(AppTheme.Spacing.lg)
                }
            }
        }
        .navigationTitle(L10n.Subscription.subscribeTitle)
        .task { await viewModel.loadPlan(planId) }
    }

    private func cycleLabel(_ cycle: String) -> String {
        switch cycle {
        case "Monthly": return L10n.Subscription.cycleMonthly
        case "Quarterly": return L10n.Subscription.cycleQuarterly
        case "SemiAnnual": return L10n.Subscription.cycleSemiAnnual
        case "Annual": return L10n.Subscription.cycleAnnual
        case "Lifetime": return L10n.Subscription.cycleLifetime
        default: return cycle
        }
    }
}

struct MySubscriptionView: View {
    @StateObject private var viewModel = SubscriptionViewModel()
    @EnvironmentObject private var appSession: AppSession
    @State private var showCancelDialog = false
    @State private var cancelReason = ""

    private var isArabic: Bool { appSession.preferredLanguage == "ar" }

    var body: some View {
        Group {
            if viewModel.isLoading && viewModel.current == nil {
                ProgressView()
            } else if viewModel.current == nil {
                VStack(spacing: AppTheme.Spacing.lg) {
                    Text(L10n.Subscription.noActive)
                        .font(AppTheme.Typography.body())
                    NavigationLink(L10n.Subscription.browsePlans) {
                        SubscriptionPlansView()
                    }
                    .buttonStyle(PrimaryButtonStyle())
                }
                .padding(AppTheme.Spacing.lg)
            } else if let subscription = viewModel.current {
                ScrollView {
                    VStack(spacing: AppTheme.Spacing.md) {
                        currentCard(subscription)
                        if !viewModel.history.isEmpty {
                            Text(L10n.Subscription.history)
                                .font(AppTheme.Typography.title())
                                .frame(maxWidth: .infinity, alignment: .leading)
                            ForEach(viewModel.history) { item in
                                historyCard(item)
                            }
                        }
                    }
                    .padding(AppTheme.Spacing.lg)
                }
            }
        }
        .navigationTitle(L10n.Subscription.mySubscription)
        .task { await viewModel.loadMySubscription() }
        .refreshable { await viewModel.loadMySubscription() }
        .alert(L10n.Subscription.cancelTitle, isPresented: $showCancelDialog) {
            TextField(L10n.Subscription.cancelReason, text: $cancelReason)
            Button(L10n.Common.cancel, role: .cancel) {}
            Button(L10n.Subscription.confirmCancel, role: .destructive) {
                Task {
                    await viewModel.cancel(reason: cancelReason.isEmpty ? nil : cancelReason)
                    cancelReason = ""
                }
            }
        } message: {
            Text(L10n.Subscription.cancelHint)
        }
    }

    @ViewBuilder
    private func currentCard(_ subscription: UserSubscription) -> some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.sm) {
            HStack {
                Text(subscription.planName(isArabic: isArabic))
                    .font(AppTheme.Typography.title())
                Spacer()
                if let amount = subscription.amountPaid {
                    Text("\(amount) \(subscription.currency ?? "USD")")
                        .font(AppTheme.Typography.title())
                }
            }
            Text(subscription.status)
                .font(AppTheme.Typography.caption())
                .foregroundStyle(AppTheme.Colors.primary)
            Text("\(L10n.Subscription.startDate): \(subscription.startDate.formatted(date: .abbreviated, time: .omitted))")
                .font(AppTheme.Typography.caption())
            if let endDate = subscription.endDate {
                Text("\(L10n.Subscription.endDate): \(endDate.formatted(date: .abbreviated, time: .omitted))")
                    .font(AppTheme.Typography.caption())
            }
            if let cycle = subscription.billingCycle {
                Text("\(L10n.Subscription.billingCycle): \(cycle)")
                    .font(AppTheme.Typography.caption())
            }
            Toggle(
                L10n.Subscription.autoRenew,
                isOn: Binding(
                    get: { subscription.autoRenew },
                    set: { enabled in Task { await viewModel.updateAutoRenew(enabled) } }
                )
            )
            .disabled(viewModel.isSubmitting)
            Button(L10n.Subscription.cancel, role: .destructive) {
                showCancelDialog = true
            }
            .buttonStyle(.bordered)
            .disabled(viewModel.isSubmitting)
        }
        .cardStyle()
    }

    @ViewBuilder
    private func historyCard(_ item: UserSubscription) -> some View {
        HStack {
            VStack(alignment: .leading) {
                Text(item.planName(isArabic: isArabic))
                    .font(AppTheme.Typography.body())
                Text("\(item.startDate.formatted(date: .abbreviated, time: .omitted)) — \(item.status)")
                    .font(AppTheme.Typography.caption())
                    .foregroundStyle(AppTheme.Colors.textSecondary)
            }
            Spacer()
            if let amount = item.amountPaid {
                Text("\(amount) \(item.currency ?? "USD")")
            }
        }
        .cardStyle()
    }
}

private struct PlanCard: View {
    let plan: SubscriptionPlan
    let isArabic: Bool
    let hasActiveSubscription: Bool

    var body: some View {
        VStack(alignment: .leading, spacing: AppTheme.Spacing.sm) {
            HStack {
                Text(plan.name(isArabic: isArabic))
                    .font(AppTheme.Typography.title())
                Spacer()
                if plan.isFeatured {
                    Text(L10n.Subscription.featured)
                        .font(AppTheme.Typography.caption())
                        .foregroundStyle(AppTheme.Colors.primary)
                }
            }
            if let description = plan.description(isArabic: isArabic), !description.isEmpty {
                Text(description)
                    .font(AppTheme.Typography.body())
                    .foregroundStyle(AppTheme.Colors.textSecondary)
            }
            ForEach(plan.billingOptions.filter(\.isActive), id: \.listId) { option in
                Text("\(option.cycle): \(option.price) \(plan.currency)")
                    .font(AppTheme.Typography.caption())
            }
            if let maxServices = plan.maxServices {
                Text(L10n.Subscription.maxServices(count: maxServices))
                    .font(AppTheme.Typography.caption())
                    .foregroundStyle(AppTheme.Colors.textSecondary)
            }
            if hasActiveSubscription {
                Text(L10n.Subscription.alreadySubscribed)
                    .font(AppTheme.Typography.body())
                    .foregroundStyle(.secondary)
            } else {
                NavigationLink(L10n.Subscription.choosePlan) {
                    SubscribePlanView(planId: plan.id)
                }
                .buttonStyle(PrimaryButtonStyle())
            }
        }
        .cardStyle()
    }
}

#Preview {
    NavigationStack {
        SubscriptionPlansView()
            .environmentObject(AppSession())
    }
}
