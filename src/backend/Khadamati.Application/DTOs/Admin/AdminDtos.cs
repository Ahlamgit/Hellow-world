namespace Khadamati.Application.DTOs.Admin;

public class AdminListQueryDto
{
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }
    public string? Role { get; set; }
    public string? Type { get; set; }
    public string? Category { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsActive { get; set; }
}

public class AdminBulkActionDto
{
    public string Action { get; set; } = string.Empty;
    public List<string> Ids { get; set; } = new();
    public string? Reason { get; set; }
}

public class AdminBulkActionResultDto
{
    public int AffectedCount { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AdminRowDto
{
    public string Id { get; set; } = string.Empty;
    public Dictionary<string, string?> Columns { get; set; } = new();
}

public class AdminListResultDto
{
    public IReadOnlyList<AdminRowDto> Items { get; set; } = Array.Empty<AdminRowDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)Math.Max(PageSize, 1));
}

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalCraftsmen { get; set; }
    public int TotalStores { get; set; }
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ActiveSubscriptions { get; set; }
    public decimal TotalRevenue { get; set; }
    public int OpenComplaints { get; set; }
    public int OpenTickets { get; set; }
    public int UnreadNotifications { get; set; }
}

public class AdminSystemHealthDto
{
    public string Status { get; set; } = "Healthy";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool DatabaseConnected { get; set; }
    public long DatabaseResponseMs { get; set; }
    public string ApiVersion { get; set; } = "1.0";
    public long MemoryUsedMb { get; set; }
    public double CpuUsagePercent { get; set; }
}

public class AdminBackupDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminRestoreRequestDto
{
    public string BackupId { get; set; } = string.Empty;
    public bool Confirm { get; set; }
}

public class AdminAnalyticsDto
{
    public List<AdminChartPointDto> BookingsByMonth { get; set; } = new();
    public List<AdminChartPointDto> RevenueByMonth { get; set; } = new();
    public List<AdminChartPointDto> UsersByRole { get; set; } = new();
}

public class AdminChartPointDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class AdminReportDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
