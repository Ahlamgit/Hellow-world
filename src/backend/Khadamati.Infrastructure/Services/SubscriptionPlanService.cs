using System.Text.Json;
using AutoMapper;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Subscriptions;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;

namespace Khadamati.Infrastructure.Services;

public class SubscriptionPlanService : ISubscriptionPlanService
{
    private readonly ISubscriptionPlanRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SubscriptionPlanService(
        ISubscriptionPlanRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SubscriptionPlanDto> CreateAsync(CreateSubscriptionPlanDto dto, string? createdBy, CancellationToken cancellationToken = default)
    {
        var planCode = dto.PlanCode.ToUpperInvariant();
        if (await _repository.CodeExistsAsync(planCode, cancellationToken: cancellationToken))
            throw new ConflictException($"Plan code '{planCode}' already exists.");

        var plan = MapToEntity(dto);
        plan.PlanCode = planCode;
        plan.CreatedBy = createdBy;

        await _repository.AddAsync(plan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(plan);
    }

    public async Task<SubscriptionPlanDto> UpdateAsync(Guid id, UpdateSubscriptionPlanDto dto, string? updatedBy, CancellationToken cancellationToken = default)
    {
        var plan = await _repository.GetByIdAsync(id, includeBillingOptions: true, cancellationToken)
            ?? throw new NotFoundException("Subscription plan not found.");

        var planCode = dto.PlanCode.ToUpperInvariant();
        if (await _repository.CodeExistsAsync(planCode, id, cancellationToken))
            throw new ConflictException($"Plan code '{planCode}' already exists.");

        UpdateEntityFromDto(plan, dto);
        plan.UpdatedBy = updatedBy;

        _repository.Update(plan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(plan);
    }

    public async Task DeleteAsync(Guid id, string? deletedBy, CancellationToken cancellationToken = default)
    {
        var plan = await _repository.GetByIdAsync(id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("Subscription plan not found.");

        _repository.SoftDelete(plan, deletedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<SubscriptionPlanDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var plan = await _repository.GetByIdAsync(id, includeBillingOptions: true, cancellationToken)
            ?? throw new NotFoundException("Subscription plan not found.");
        return MapToDto(plan);
    }

    public async Task<PagedResult<SubscriptionPlanDto>> SearchAsync(SubscriptionPlanListQueryDto query, CancellationToken cancellationToken = default)
    {
        PlanStatus? status = null;
        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<PlanStatus>(query.Status, true, out var parsedStatus))
            status = parsedStatus;

        UserRole? targetRole = null;
        if (!string.IsNullOrWhiteSpace(query.TargetRole) && Enum.TryParse<UserRole>(query.TargetRole, true, out var parsedRole))
            targetRole = parsedRole;

        var (items, totalCount) = await _repository.SearchAsync(
            query.Search, status, targetRole, query.Featured, query.IncludeArchived,
            query.Page, query.PageSize, cancellationToken);

        return new PagedResult<SubscriptionPlanDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<IReadOnlyList<SubscriptionPlanDto>> GetPublicPlansAsync(string? targetRole, CancellationToken cancellationToken = default)
    {
        UserRole? role = null;
        if (!string.IsNullOrWhiteSpace(targetRole) && Enum.TryParse<UserRole>(targetRole, true, out var parsedRole))
            role = parsedRole;

        var plans = await _repository.GetPublicPlansAsync(role, cancellationToken);
        return plans.Select(MapToDto).ToList();
    }

    public async Task<SubscriptionPlanDto> CloneAsync(Guid id, CloneSubscriptionPlanDto dto, string? createdBy, CancellationToken cancellationToken = default)
    {
        var source = await _repository.GetByIdAsync(id, includeBillingOptions: true, cancellationToken)
            ?? throw new NotFoundException("Subscription plan not found.");

        var newCode = string.IsNullOrWhiteSpace(dto.NewPlanCode)
            ? await GenerateUniquePlanCodeAsync($"{source.PlanCode}_COPY", cancellationToken)
            : dto.NewPlanCode!.ToUpperInvariant();

        if (await _repository.CodeExistsAsync(newCode, cancellationToken: cancellationToken))
            throw new ConflictException($"Plan code '{newCode}' already exists.");

        var clone = new SubscriptionPlan
        {
            PlanCode = newCode,
            NameEn = source.NameEn + (dto.NameEnSuffix ?? " (Copy)"),
            NameAr = source.NameAr + (dto.NameArSuffix ?? " (نسخة)"),
            DescriptionEn = source.DescriptionEn,
            DescriptionAr = source.DescriptionAr,
            Currency = source.Currency,
            TargetRole = source.TargetRole,
            Status = PlanStatus.Inactive,
            DisplayPriority = source.DisplayPriority,
            SearchPriority = source.SearchPriority,
            IsFeatured = false,
            HomePageVisible = source.HomePageVisible,
            BannerVisible = source.BannerVisible,
            CategoryVisible = source.CategoryVisible,
            MaxCategories = source.MaxCategories,
            MaxServices = source.MaxServices,
            MaxPhotos = source.MaxPhotos,
            MaxVideos = source.MaxVideos,
            MaxAdvertisements = source.MaxAdvertisements,
            AdvertisementCredits = source.AdvertisementCredits,
            FeaturedDays = source.FeaturedDays,
            VerificationBadge = source.VerificationBadge,
            PremiumBadge = source.PremiumBadge,
            StatisticsDashboard = source.StatisticsDashboard,
            Analytics = source.Analytics,
            PriorityCustomerSupport = source.PriorityCustomerSupport,
            RenewalReminder = source.RenewalReminder,
            AutoRenewal = source.AutoRenewal,
            ExpiryNotification = source.ExpiryNotification,
            GracePeriodDays = source.GracePeriodDays,
            TrialDays = source.TrialDays,
            DiscountPercentage = source.DiscountPercentage,
            CouponSupport = source.CouponSupport,
            TaxRate = source.TaxRate,
            VatRate = source.VatRate,
            PaymentRequired = source.PaymentRequired,
            PaymentMethods = source.PaymentMethods,
            PlanColor = source.PlanColor,
            PlanIcon = source.PlanIcon,
            ClonedFromPlanId = source.Id,
            CreatedBy = createdBy,
            BillingOptions = source.BillingOptions
                .Where(b => !b.IsDeleted)
                .Select(b => new PlanBillingOption
                {
                    Cycle = b.Cycle,
                    Price = b.Price,
                    DurationDays = b.DurationDays,
                    IsActive = b.IsActive
                }).ToList()
        };

        await _repository.AddAsync(clone, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(clone);
    }

    public async Task<PlanActionResponseDto> ActivateAsync(Guid id, string? updatedBy, CancellationToken cancellationToken = default) =>
        await ChangeStatusAsync(id, PlanStatus.Active, "Plan activated successfully.", updatedBy, clearSuspended: true, clearArchived: true, cancellationToken: cancellationToken);

    public async Task<PlanActionResponseDto> DeactivateAsync(Guid id, string? updatedBy, CancellationToken cancellationToken = default) =>
        await ChangeStatusAsync(id, PlanStatus.Inactive, "Plan deactivated successfully.", updatedBy, cancellationToken: cancellationToken);

    public async Task<PlanActionResponseDto> SuspendAsync(Guid id, string? updatedBy, CancellationToken cancellationToken = default) =>
        await ChangeStatusAsync(id, PlanStatus.Suspended, "Plan suspended successfully.", updatedBy, setSuspended: true, cancellationToken: cancellationToken);

    public async Task<PlanActionResponseDto> ArchiveAsync(Guid id, string? updatedBy, CancellationToken cancellationToken = default) =>
        await ChangeStatusAsync(id, PlanStatus.Archived, "Plan archived successfully.", updatedBy, setArchived: true, cancellationToken: cancellationToken);

    private async Task<PlanActionResponseDto> ChangeStatusAsync(
        Guid id,
        PlanStatus status,
        string message,
        string? updatedBy,
        bool clearSuspended = false,
        bool clearArchived = false,
        bool setSuspended = false,
        bool setArchived = false,
        CancellationToken cancellationToken = default)
    {
        var plan = await _repository.GetByIdAsync(id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException("Subscription plan not found.");

        plan.Status = status;
        plan.UpdatedBy = updatedBy;

        if (setSuspended) plan.SuspendedAt = DateTime.UtcNow;
        if (clearSuspended) plan.SuspendedAt = null;
        if (setArchived) plan.ArchivedAt = DateTime.UtcNow;
        if (clearArchived) plan.ArchivedAt = null;

        _repository.Update(plan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PlanActionResponseDto
        {
            PlanId = plan.Id,
            Status = plan.Status.ToString(),
            Message = message
        };
    }

    private async Task<string> GenerateUniquePlanCodeAsync(string baseCode, CancellationToken cancellationToken)
    {
        var code = baseCode.ToUpperInvariant();
        var suffix = 1;
        while (await _repository.CodeExistsAsync(code, cancellationToken: cancellationToken))
        {
            code = $"{baseCode.ToUpperInvariant()}_{suffix++}";
        }
        return code;
    }

    private SubscriptionPlan MapToEntity(CreateSubscriptionPlanDto dto)
    {
        var plan = _mapper.Map<SubscriptionPlan>(dto);
        plan.Currency = PlatformConstants.DefaultCurrency;
        plan.BillingOptions = dto.BillingOptions.Select(b => new PlanBillingOption
        {
            Cycle = Enum.Parse<BillingCycle>(b.Cycle, true),
            Price = b.Price,
            DurationDays = b.DurationDays,
            IsActive = b.IsActive
        }).ToList();
        plan.PaymentMethods = SerializePaymentMethods(dto.PaymentMethods);
        return plan;
    }

    private void UpdateEntityFromDto(SubscriptionPlan plan, UpdateSubscriptionPlanDto dto)
    {
        _mapper.Map(dto, plan);
        plan.Currency = PlatformConstants.DefaultCurrency;
        plan.PlanCode = dto.PlanCode.ToUpperInvariant();
        plan.PaymentMethods = SerializePaymentMethods(dto.PaymentMethods);

        var incomingCycles = dto.BillingOptions.ToDictionary(
            b => Enum.Parse<BillingCycle>(b.Cycle, true),
            b => b);

        foreach (var existing in plan.BillingOptions.Where(b => !b.IsDeleted).ToList())
        {
            if (incomingCycles.TryGetValue(existing.Cycle, out var incoming))
            {
                existing.Price = incoming.Price;
                existing.DurationDays = incoming.DurationDays;
                existing.IsActive = incoming.IsActive;
                incomingCycles.Remove(existing.Cycle);
            }
            else
            {
                existing.IsDeleted = true;
                existing.DeletedAt = DateTime.UtcNow;
            }
        }

        foreach (var remaining in incomingCycles.Values)
        {
            plan.BillingOptions.Add(new PlanBillingOption
            {
                Cycle = Enum.Parse<BillingCycle>(remaining.Cycle, true),
                Price = remaining.Price,
                DurationDays = remaining.DurationDays,
                IsActive = remaining.IsActive
            });
        }
    }

    private SubscriptionPlanDto MapToDto(SubscriptionPlan plan)
    {
        var dto = _mapper.Map<SubscriptionPlanDto>(plan);
        dto.BillingOptions = plan.BillingOptions
            .Where(b => !b.IsDeleted)
            .OrderBy(b => b.Cycle)
            .Select(b => new PlanBillingOptionDto
            {
                Id = b.Id,
                Cycle = b.Cycle.ToString(),
                Price = b.Price,
                DurationDays = b.DurationDays,
                IsActive = b.IsActive
            }).ToList();
        dto.PaymentMethods = DeserializePaymentMethods(plan.PaymentMethods);
        return dto;
    }

    private static string? SerializePaymentMethods(List<string> methods) =>
        methods.Count == 0 ? null : JsonSerializer.Serialize(methods);

    private static List<string> DeserializePaymentMethods(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<string>();
        try { return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>(); }
        catch { return new List<string>(); }
    }
}
