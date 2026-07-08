using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Subscriptions;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;

namespace Khadamati.Infrastructure.Services;

public class UserSubscriptionService : IUserSubscriptionService
{
    private readonly IUserSubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserSubscriptionService(
        IUserSubscriptionRepository subscriptionRepository,
        ISubscriptionPlanRepository planRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _planRepository = planRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserSubscriptionDto?> GetCurrentAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionRepository.GetActiveByUserIdAsync(userId, cancellationToken);
        return subscription is null ? null : MapToDto(subscription);
    }

    public async Task<PagedResult<UserSubscriptionDto>> GetHistoryAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _subscriptionRepository.GetByUserIdAsync(userId, page, pageSize, cancellationToken);
        return new PagedResult<UserSubscriptionDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<UserSubscriptionDto> SubscribeAsync(Guid userId, SubscribeRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (!user.IsEmailVerified)
            throw new ValidationException(["Email verification is required before subscribing."]);

        if (user.Role is not (UserRole.Craftsman or UserRole.Store))
            throw new ValidationException(["Only craftsmen and store accounts can subscribe to plans."]);

        if (await _subscriptionRepository.HasActiveSubscriptionAsync(userId, cancellationToken))
            throw new ConflictException("You already have an active subscription.");

        var plan = await _planRepository.GetByIdAsync(request.PlanId, includeBillingOptions: true, cancellationToken)
            ?? throw new NotFoundException("Subscription plan not found.");

        if (plan.Status != PlanStatus.Active)
            throw new ValidationException(["This subscription plan is not available."]);

        if (plan.TargetRole != user.Role)
            throw new ValidationException([$"This plan is for {plan.TargetRole} accounts only."]);

        var billingOption = plan.BillingOptions.FirstOrDefault(b => b.Id == request.BillingOptionId && b.IsActive && !b.IsDeleted)
            ?? throw new ValidationException(["Invalid billing option for this plan."]);

        var subscription = BuildSubscription(user, plan, billingOption, request.AutoRenew, request.CouponCode);
        await _subscriptionRepository.AddAsync(subscription, cancellationToken);
        SyncUserSubscriptionFields(user, subscription);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _subscriptionRepository.GetByIdAsync(subscription.Id, includeDetails: true, cancellationToken);
        return MapToDto(created!);
    }

    public async Task<SubscriptionActionResponseDto> CancelAsync(
        Guid userId, Guid subscriptionId, CancelSubscriptionDto request, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("Subscription not found.");

        if (subscription.UserId != userId)
            throw new UnauthorizedException("You can only cancel your own subscription.");

        return await CancelSubscriptionInternalAsync(subscription, request.Reason, cancellationToken);
    }

    public async Task<UserSubscriptionDto> UpdateAutoRenewAsync(Guid userId, UpdateAutoRenewDto request, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionRepository.GetActiveByUserIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("No active subscription found.");

        subscription.AutoRenew = request.AutoRenew;
        subscription.UpdatedAt = DateTime.UtcNow;
        _subscriptionRepository.Update(subscription);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _subscriptionRepository.GetByIdAsync(subscription.Id, includeDetails: true, cancellationToken);
        return MapToDto(updated!);
    }

    public async Task<PagedResult<UserSubscriptionDto>> SearchAdminAsync(UserSubscriptionListQueryDto query, CancellationToken cancellationToken = default)
    {
        SubscriptionStatus? status = null;
        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<SubscriptionStatus>(query.Status, true, out var parsed))
            status = parsed;

        var (items, totalCount) = await _subscriptionRepository.SearchAsync(
            query.Search, status, query.UserId, query.PlanId, query.Page, query.PageSize, cancellationToken);

        return new PagedResult<UserSubscriptionDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        };
    }

    public async Task<UserSubscriptionDto> GetByIdAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("Subscription not found.");
        return MapToDto(subscription);
    }

    public async Task<UserSubscriptionDto> GrantAsync(
        Guid userId, AdminGrantSubscriptionDto request, string? grantedBy, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (await _subscriptionRepository.HasActiveSubscriptionAsync(userId, cancellationToken))
            throw new ConflictException("User already has an active subscription.");

        var plan = await _planRepository.GetByIdAsync(request.PlanId, includeBillingOptions: true, cancellationToken)
            ?? throw new NotFoundException("Subscription plan not found.");

        var billingOption = plan.BillingOptions.FirstOrDefault(b => b.Id == request.BillingOptionId && !b.IsDeleted)
            ?? throw new ValidationException(["Invalid billing option for this plan."]);

        var subscription = BuildSubscription(user, plan, billingOption, request.AutoRenew, couponCode: null);
        subscription.CreatedBy = grantedBy;
        await _subscriptionRepository.AddAsync(subscription, cancellationToken);
        SyncUserSubscriptionFields(user, subscription);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _subscriptionRepository.GetByIdAsync(subscription.Id, includeDetails: true, cancellationToken);
        return MapToDto(created!);
    }

    public async Task<SubscriptionActionResponseDto> CancelAdminAsync(
        Guid subscriptionId, CancelSubscriptionDto request, string? cancelledBy, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId, includeDetails: true, cancellationToken)
            ?? throw new NotFoundException("Subscription not found.");

        subscription.UpdatedBy = cancelledBy;
        return await CancelSubscriptionInternalAsync(subscription, request.Reason, cancellationToken);
    }

    private async Task<SubscriptionActionResponseDto> CancelSubscriptionInternalAsync(
        UserSubscription subscription, string? reason, CancellationToken cancellationToken)
    {
        if (subscription.Status is SubscriptionStatus.Cancelled or SubscriptionStatus.Expired)
            throw new ValidationException(["Subscription is already inactive."]);

        subscription.Status = SubscriptionStatus.Cancelled;
        subscription.CancelledAt = DateTime.UtcNow;
        subscription.CancellationReason = reason;
        subscription.UpdatedAt = DateTime.UtcNow;
        _subscriptionRepository.Update(subscription);

        var user = subscription.User ?? await _userRepository.GetByIdAsync(subscription.UserId, cancellationToken: cancellationToken);
        if (user is not null)
        {
            user.SubscriptionStatus = SubscriptionStatus.Cancelled;
            _userRepository.Update(user);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubscriptionActionResponseDto
        {
            SubscriptionId = subscription.Id,
            Status = nameof(SubscriptionStatus.Cancelled),
            Message = "Subscription cancelled successfully.",
        };
    }

    private static UserSubscription BuildSubscription(
        User user,
        SubscriptionPlan plan,
        PlanBillingOption billingOption,
        bool autoRenew,
        string? couponCode)
    {
        var now = DateTime.UtcNow;
        var status = plan.TrialDays > 0 ? SubscriptionStatus.Trial : SubscriptionStatus.Active;
        var endDate = now.AddDays(billingOption.DurationDays);

        return new UserSubscription
        {
            UserId = user.Id,
            PlanId = plan.Id,
            BillingOptionId = billingOption.Id,
            Status = status,
            StartDate = now,
            EndDate = endDate,
            AutoRenew = autoRenew || plan.AutoRenewal,
            AmountPaid = billingOption.Price,
            Currency = plan.Currency,
            CouponCode = couponCode,
        };
    }

    private static void SyncUserSubscriptionFields(User user, UserSubscription subscription)
    {
        user.SubscriptionStatus = subscription.Status;
        user.SubscriptionExpiresAt = subscription.EndDate;
    }

    private static UserSubscriptionDto MapToDto(UserSubscription subscription) => new()
    {
        Id = subscription.Id,
        UserId = subscription.UserId,
        UserEmail = subscription.User?.Email ?? string.Empty,
        UserName = subscription.User?.Profile is { } profile
            ? $"{profile.FirstName} {profile.LastName}".Trim()
            : string.Empty,
        PlanId = subscription.PlanId,
        PlanCode = subscription.Plan?.PlanCode ?? string.Empty,
        PlanNameEn = subscription.Plan?.NameEn ?? string.Empty,
        PlanNameAr = subscription.Plan?.NameAr ?? string.Empty,
        BillingOptionId = subscription.BillingOptionId,
        BillingCycle = subscription.BillingOption?.Cycle.ToString(),
        Status = subscription.Status.ToString(),
        StartDate = subscription.StartDate,
        EndDate = subscription.EndDate,
        AutoRenew = subscription.AutoRenew,
        AmountPaid = subscription.AmountPaid,
        Currency = subscription.Currency,
        CouponCode = subscription.CouponCode,
        CancelledAt = subscription.CancelledAt,
        CancellationReason = subscription.CancellationReason,
        CreatedAt = subscription.CreatedAt,
    };
}
