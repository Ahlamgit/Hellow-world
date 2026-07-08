using Khadamati.Domain.Common;

namespace Khadamati.Domain.Entities;

public class CraftsmanWorkingHour : BaseEntity
{
    public Guid CraftsmanId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsActive { get; set; } = true;

    public User Craftsman { get; set; } = null!;
}
