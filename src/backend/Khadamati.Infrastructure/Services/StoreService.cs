using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Store;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Interfaces;

namespace Khadamati.Infrastructure.Services;

public class StoreService : IStoreService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;

    public StoreService(IUnitOfWork unitOfWork, IIdentityRepository identityRepository)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
    }

    public async Task<StoreProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await EnsureStoreRoleAsync(userId, cancellationToken);
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);
        return await MapProfileAsync(profile, cancellationToken);
    }

    public async Task<StoreProfileDto> UpdateProfileAsync(Guid userId, UpdateStoreProfileDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureStoreRoleAsync(userId, cancellationToken);
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);

        profile.StoreName = dto.StoreName.Trim();
        profile.CommercialRegistration = dto.CommercialRegistration;
        profile.Description = dto.Description;
        profile.IsOpen = dto.IsOpen;
        profile.OpeningTime = ParseTime(dto.OpeningTime);
        profile.ClosingTime = ParseTime(dto.ClosingTime);
        profile.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<StoreProfile>().Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await MapProfileAsync(profile, cancellationToken);
    }

    public async Task<StoreProductDto> UpsertProductAsync(Guid userId, UpsertStoreProductDto dto, Guid? productId = null, CancellationToken cancellationToken = default)
    {
        await EnsureStoreRoleAsync(userId, cancellationToken);
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);

        StoreProduct product;
        if (productId is null)
        {
            product = new StoreProduct { StoreProfileId = profile.Id };
            await _unitOfWork.Repository<StoreProduct>().AddAsync(product, cancellationToken);
        }
        else
        {
            product = await _unitOfWork.Repository<StoreProduct>().GetByIdAsync(productId.Value, cancellationToken)
                ?? throw new NotFoundException("Product not found.");
            if (product.StoreProfileId != profile.Id)
                throw new UnauthorizedException("Not authorized.");
        }

        product.NameAr = dto.NameAr;
        product.NameEn = dto.NameEn;
        product.DescriptionAr = dto.DescriptionAr;
        product.DescriptionEn = dto.DescriptionEn;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.Sku = dto.Sku;
        product.ImageUrl = dto.ImageUrl;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        if (productId is not null)
            _unitOfWork.Repository<StoreProduct>().Update(product);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapProduct(product);
    }

    public async Task DeleteProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
    {
        var profile = await GetProfileForUserAsync(userId, cancellationToken);
        var product = await _unitOfWork.Repository<StoreProduct>().GetByIdAsync(productId, cancellationToken)
            ?? throw new NotFoundException("Product not found.");
        if (product.StoreProfileId != profile.Id)
            throw new UnauthorizedException("Not authorized.");

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<StoreProduct>().Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureStoreRoleAsync(Guid userId, CancellationToken cancellationToken)
    {
        var roles = await _identityRepository.GetUserRoleNamesAsync(userId, cancellationToken);
        if (!roles.Any(r => r.Equals(RoleNames.StoreOwner, StringComparison.OrdinalIgnoreCase)
                         || r.Equals(RoleNames.StoreEmployee, StringComparison.OrdinalIgnoreCase)))
            throw new UnauthorizedException("Store role required.");
    }

    private async Task<StoreProfile> GetOrCreateProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await _unitOfWork.Repository<StoreProfile>()
            .FirstOrDefaultAsync(sp => sp.UserId == userId && !sp.IsDeleted, cancellationToken);

        if (profile is not null) return profile;

        var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        profile = new StoreProfile
        {
            UserId = userId,
            StoreName = $"{user.Profile?.FirstName} Store".Trim(),
            IsOpen = true,
            OpeningTime = new TimeOnly(9, 0),
            ClosingTime = new TimeOnly(22, 0),
        };
        await _unitOfWork.Repository<StoreProfile>().AddAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return profile;
    }

    private async Task<StoreProfile> GetProfileForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<StoreProfile>()
            .FirstOrDefaultAsync(sp => sp.UserId == userId && !sp.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Store profile not found.");
    }

    private async Task<StoreProfileDto> MapProfileAsync(StoreProfile profile, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Repository<StoreProduct>()
            .FindAsync(p => p.StoreProfileId == profile.Id && !p.IsDeleted, cancellationToken);

        return new StoreProfileDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            StoreName = profile.StoreName,
            CommercialRegistration = profile.CommercialRegistration,
            Description = profile.Description,
            Rating = profile.Rating,
            TotalReviews = profile.TotalReviews,
            IsOpen = profile.IsOpen,
            OpeningTime = profile.OpeningTime?.ToString("HH:mm"),
            ClosingTime = profile.ClosingTime?.ToString("HH:mm"),
            Products = products.Select(MapProduct).ToList(),
        };
    }

    private static StoreProductDto MapProduct(StoreProduct p) => new()
    {
        Id = p.Id,
        NameAr = p.NameAr,
        NameEn = p.NameEn,
        DescriptionAr = p.DescriptionAr,
        DescriptionEn = p.DescriptionEn,
        Price = p.Price,
        StockQuantity = p.StockQuantity,
        Sku = p.Sku,
        ImageUrl = p.ImageUrl,
        IsActive = p.IsActive,
    };

    private static TimeOnly? ParseTime(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : TimeOnly.TryParse(value, out var t) ? t : null;
}
