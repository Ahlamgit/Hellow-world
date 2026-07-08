using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string PreferredLanguage { get; set; } = "ar";
    public string? NationalId { get; set; }

    public User User { get; set; } = null!;
}
