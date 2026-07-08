using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class ServiceCategory : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? ParentCategoryId { get; set; }

    public ServiceCategory? ParentCategory { get; set; }
    public ICollection<ServiceCategory> SubCategories { get; set; } = new List<ServiceCategory>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
