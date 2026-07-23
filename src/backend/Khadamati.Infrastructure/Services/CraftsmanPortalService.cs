using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Craftsman;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Services;

public class CraftsmanPortalService : ICraftsmanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityRepository _identityRepository;

    public CraftsmanPortalService(IUnitOfWork unitOfWork, IIdentityRepository identityRepository)
    {
        _unitOfWork = unitOfWork;
        _identityRepository = identityRepository;
    }

    public async Task<CraftsmanProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await EnsureCraftsmanRoleAsync(userId, cancellationToken);
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);
        return await MapProfileAsync(profile, cancellationToken);
    }

    public async Task<CraftsmanProfileDto> UpdateProfileAsync(Guid userId, UpdateCraftsmanProfileDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureCraftsmanRoleAsync(userId, cancellationToken);
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);

        profile.Specialization = dto.Specialization;
        profile.YearsOfExperience = dto.YearsOfExperience;
        profile.IsAvailable = dto.IsAvailable;
        profile.ServiceRadiusKm = dto.ServiceRadiusKm;
        profile.LicenseNumber = dto.LicenseNumber;
        profile.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<CraftsmanProfile>().Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await MapProfileAsync(profile, cancellationToken);
    }

    public async Task<CraftsmanServiceDto> UpsertServiceAsync(Guid userId, UpsertCraftsmanServiceDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureCraftsmanRoleAsync(userId, cancellationToken);
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);

        var service = await _unitOfWork.Repository<Service>().GetByIdAsync(dto.ServiceId, cancellationToken)
            ?? throw new NotFoundException("Service not found.");

        var existing = await _unitOfWork.Repository<Domain.Entities.CraftsmanService>()
            .FirstOrDefaultAsync(cs => cs.CraftsmanProfileId == profile.Id && cs.ServiceId == dto.ServiceId, cancellationToken);

        if (existing is null)
        {
            existing = new Domain.Entities.CraftsmanService
            {
                CraftsmanProfileId = profile.Id,
                ServiceId = dto.ServiceId,
                CustomPrice = dto.CustomPrice > 0 ? dto.CustomPrice : service.BasePrice,
                IsAvailable = dto.IsAvailable,
            };
            await _unitOfWork.Repository<Domain.Entities.CraftsmanService>().AddAsync(existing, cancellationToken);
        }
        else
        {
            existing.CustomPrice = dto.CustomPrice > 0 ? dto.CustomPrice : service.BasePrice;
            existing.IsAvailable = dto.IsAvailable;
            existing.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Domain.Entities.CraftsmanService>().Update(existing);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapService(existing, service);
    }

    public async Task DeleteServiceAsync(Guid userId, Guid craftsmanServiceId, CancellationToken cancellationToken = default)
    {
        var profile = await GetProfileForUserAsync(userId, cancellationToken);
        var item = await _unitOfWork.Repository<Domain.Entities.CraftsmanService>().GetByIdAsync(craftsmanServiceId, cancellationToken)
            ?? throw new NotFoundException("Craftsman service not found.");
        if (item.CraftsmanProfileId != profile.Id)
            throw new ForbiddenException("Not authorized.");

        item.IsDeleted = true;
        item.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Domain.Entities.CraftsmanService>().Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<CraftsmanWorkingHourDto> UpsertWorkingHourAsync(Guid userId, UpsertWorkingHourDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureCraftsmanRoleAsync(userId, cancellationToken);
        _ = await GetOrCreateProfileAsync(userId, cancellationToken);

        if (!TimeOnly.TryParse(dto.StartTime, out var start) || !TimeOnly.TryParse(dto.EndTime, out var end))
            throw new ValidationException(["Invalid time format. Use HH:mm."]);
        if (end <= start)
            throw new ValidationException(["End time must be after start time."]);

        var existing = await _unitOfWork.Repository<CraftsmanWorkingHour>()
            .FirstOrDefaultAsync(w => w.CraftsmanId == userId && w.DayOfWeek == dto.DayOfWeek, cancellationToken);

        if (existing is null)
        {
            existing = new CraftsmanWorkingHour
            {
                CraftsmanId = userId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = start,
                EndTime = end,
                IsActive = dto.IsActive,
            };
            await _unitOfWork.Repository<CraftsmanWorkingHour>().AddAsync(existing, cancellationToken);
        }
        else
        {
            existing.StartTime = start;
            existing.EndTime = end;
            existing.IsActive = dto.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<CraftsmanWorkingHour>().Update(existing);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapWorkingHour(existing);
    }

    public async Task DeleteWorkingHourAsync(Guid userId, Guid workingHourId, CancellationToken cancellationToken = default)
    {
        var hour = await _unitOfWork.Repository<CraftsmanWorkingHour>().GetByIdAsync(workingHourId, cancellationToken)
            ?? throw new NotFoundException("Working hour not found.");
        if (hour.CraftsmanId != userId)
            throw new ForbiddenException("Not authorized.");

        hour.IsDeleted = true;
        hour.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<CraftsmanWorkingHour>().Update(hour);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCraftsmanRoleAsync(Guid userId, CancellationToken cancellationToken)
    {
        var roles = await _identityRepository.GetUserRoleNamesAsync(userId, cancellationToken);
        if (!roles.Contains(RoleNames.Craftsman, StringComparer.OrdinalIgnoreCase))
            throw new ForbiddenException("Craftsman role required.");
    }

    private async Task<CraftsmanProfile> GetOrCreateProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await _unitOfWork.Repository<CraftsmanProfile>()
            .FirstOrDefaultAsync(cp => cp.UserId == userId && !cp.IsDeleted, cancellationToken);

        if (profile is not null) return profile;

        profile = new CraftsmanProfile
        {
            UserId = userId,
            Specialization = "General",
            IsAvailable = true,
            ServiceRadiusKm = 25,
        };
        await _unitOfWork.Repository<CraftsmanProfile>().AddAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return profile;
    }

    private async Task<CraftsmanProfile> GetProfileForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<CraftsmanProfile>()
            .FirstOrDefaultAsync(cp => cp.UserId == userId && !cp.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Craftsman profile not found.");
    }

    private async Task<CraftsmanProfileDto> MapProfileAsync(CraftsmanProfile profile, CancellationToken cancellationToken)
    {
        var services = await _unitOfWork.Repository<Domain.Entities.CraftsmanService>()
            .FindAsync(cs => cs.CraftsmanProfileId == profile.Id && !cs.IsDeleted, cancellationToken);
        var serviceIds = services.Select(s => s.ServiceId).ToList();
        var catalog = serviceIds.Count > 0
            ? await _unitOfWork.Repository<Service>().FindAsync(s => serviceIds.Contains(s.Id), cancellationToken)
            : [];
        var catalogMap = catalog.ToDictionary(s => s.Id);

        var hours = await _unitOfWork.Repository<CraftsmanWorkingHour>()
            .FindAsync(w => w.CraftsmanId == profile.UserId && !w.IsDeleted, cancellationToken);

        return new CraftsmanProfileDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            Specialization = profile.Specialization,
            YearsOfExperience = profile.YearsOfExperience,
            Rating = profile.Rating,
            TotalReviews = profile.TotalReviews,
            CompletedJobs = profile.CompletedJobs,
            IsAvailable = profile.IsAvailable,
            ServiceRadiusKm = profile.ServiceRadiusKm,
            LicenseNumber = profile.LicenseNumber,
            Services = services.Select(s => MapService(s, catalogMap.GetValueOrDefault(s.ServiceId))).ToList(),
            WorkingHours = hours.Select(MapWorkingHour).OrderBy(h => h.DayOfWeek).ToList(),
        };
    }

    private static CraftsmanServiceDto MapService(Domain.Entities.CraftsmanService cs, Service? service) => new()
    {
        Id = cs.Id,
        ServiceId = cs.ServiceId,
        ServiceNameEn = service?.NameEn ?? string.Empty,
        ServiceNameAr = service?.NameAr ?? string.Empty,
        CustomPrice = cs.CustomPrice,
        IsAvailable = cs.IsAvailable,
    };

    private static CraftsmanWorkingHourDto MapWorkingHour(CraftsmanWorkingHour w) => new()
    {
        Id = w.Id,
        DayOfWeek = w.DayOfWeek,
        StartTime = w.StartTime.ToString("HH:mm"),
        EndTime = w.EndTime.ToString("HH:mm"),
        IsActive = w.IsActive,
    };
}
