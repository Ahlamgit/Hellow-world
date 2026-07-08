using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string PreferredLanguage { get; set; } = "ar";
    public string Timezone { get; set; } = "Asia/Riyadh";
    public string? NationalId { get; set; }
    public string? Nationality { get; set; }
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? AddressLine { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public User User { get; set; } = null!;
}
