namespace Khadamati.Application.DTOs.Services;

public record ServiceCategoryDto(
    Guid Id,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    string? IconUrl,
    int DisplayOrder,
    IReadOnlyList<ServiceCategoryDto>? SubCategories);

public record ServiceDto(
    Guid Id,
    Guid CategoryId,
    string NameAr,
    string NameEn,
    string? DescriptionAr,
    string? DescriptionEn,
    decimal BasePrice,
    string? ImageUrl,
    int EstimatedDurationMinutes);

public record CreateServiceRequestDto(
    Guid ServiceId,
    Guid? AddressId,
    string? Description,
    DateTime? ScheduledAt);

public record ServiceRequestDto(
    Guid Id,
    Guid ServiceId,
    string ServiceName,
    string Status,
    string? Description,
    DateTime? ScheduledAt,
    DateTime? CompletedAt,
    decimal EstimatedPrice,
    decimal? FinalPrice,
    string? CraftsmanName,
    int? CustomerRating);
