namespace Khadamati.Application.DTOs.Craftsman;

public class CraftsmanProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Specialization { get; set; }
    public int YearsOfExperience { get; set; }
    public decimal Rating { get; set; }
    public int TotalReviews { get; set; }
    public int CompletedJobs { get; set; }
    public bool IsAvailable { get; set; }
    public decimal? ServiceRadiusKm { get; set; }
    public string? LicenseNumber { get; set; }
    public IReadOnlyList<CraftsmanServiceDto> Services { get; set; } = Array.Empty<CraftsmanServiceDto>();
    public IReadOnlyList<CraftsmanWorkingHourDto> WorkingHours { get; set; } = Array.Empty<CraftsmanWorkingHourDto>();
}

public class CraftsmanServiceDto
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public string ServiceNameEn { get; set; } = string.Empty;
    public string ServiceNameAr { get; set; } = string.Empty;
    public decimal CustomPrice { get; set; }
    public bool IsAvailable { get; set; }
}

public class CraftsmanWorkingHourDto
{
    public Guid Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class UpdateCraftsmanProfileDto
{
    public string? Specialization { get; set; }
    public int YearsOfExperience { get; set; }
    public bool IsAvailable { get; set; } = true;
    public decimal? ServiceRadiusKm { get; set; }
    public string? LicenseNumber { get; set; }
}

public class UpsertCraftsmanServiceDto
{
    public Guid ServiceId { get; set; }
    public decimal CustomPrice { get; set; }
    public bool IsAvailable { get; set; } = true;
}

public class UpsertWorkingHourDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public string StartTime { get; set; } = "09:00";
    public string EndTime { get; set; } = "18:00";
    public bool IsActive { get; set; } = true;
}
