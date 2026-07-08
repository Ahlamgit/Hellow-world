using Khadamati.Domain.Common;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Entities;

public class ServiceRequestStatusHistory : BaseEntity
{
    public Guid ServiceRequestId { get; set; }
    public ServiceRequestStatus? OldStatus { get; set; }
    public ServiceRequestStatus NewStatus { get; set; }
    public Guid? ChangedByUserId { get; set; }
    public string? Notes { get; set; }

    public ServiceRequest ServiceRequest { get; set; } = null!;
    public User? ChangedByUser { get; set; }
}
