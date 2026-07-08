using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class CraftsmanService : BaseEntity
{
    public Guid CraftsmanProfileId { get; set; }
    public Guid ServiceId { get; set; }
    public decimal CustomPrice { get; set; }
    public bool IsAvailable { get; set; } = true;

    public CraftsmanProfile CraftsmanProfile { get; set; } = null!;
    public Service Service { get; set; } = null!;
}
