using System.Diagnostics;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Admin;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Common;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Entities.Identity;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;
    private readonly IAdminExportService _export;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;

    private static readonly Dictionary<string, string[]> ModuleColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["users"] = ["email", "phone", "role", "status", "createdAt"],
        ["customers"] = ["email", "phone", "name", "status", "createdAt"],
        ["craftsmen"] = ["email", "name", "specialization", "rating", "status"],
        ["stores"] = ["email", "name", "status", "createdAt"],
        ["categories"] = ["nameEn", "nameAr", "displayOrder", "isActive"],
        ["services"] = ["nameEn", "nameAr", "basePrice", "isActive"],
        ["bookings"] = ["reference", "service", "customer", "status", "scheduledAt"],
        ["subscriptions"] = ["planCode", "nameEn", "price", "status", "targetRole"],
        ["advertisements"] = ["titleEn", "placement", "startDate", "isActive"],
        ["coupons"] = ["code", "discount", "usedCount", "validTo", "isActive"],
        ["notifications"] = ["titleEn", "type", "isRead", "createdAt"],
        ["payments"] = ["amount", "currency", "status", "method", "paidAt"],
        ["reports"] = ["name", "type", "status", "generatedAt"],
        ["complaints"] = ["subject", "status", "priority", "createdAt"],
        ["support-tickets"] = ["ticketNumber", "subject", "status", "priority"],
        ["cities"] = ["nameEn", "nameAr", "code", "isActive"],
        ["regions"] = ["nameEn", "nameAr", "code", "isActive"],
        ["settings"] = ["key", "value", "category"],
        ["roles"] = ["role", "description"],
        ["permissions"] = ["code", "module", "nameEn"],
        ["audit-logs"] = ["table", "action", "userEmail", "createdAt"],
        ["activity-logs"] = ["action", "module", "details", "createdAt"],
        ["backup"] = ["name", "status", "sizeBytes", "createdAt"],
        ["restore"] = ["name", "status", "completedAt", "createdAt"],
    };

    public AdminService(ApplicationDbContext context, IAdminExportService export, IUnitOfWork unitOfWork, IPermissionService permissionService)
    {
        _context = context;
        _export = export;
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var users = await _context.Users.CountAsync(cancellationToken);
        return new AdminDashboardDto
        {
            TotalUsers = users,
            TotalCustomers = await _context.Users.CountAsync(u => u.Role == UserRole.Customer, cancellationToken),
            TotalCraftsmen = await _context.Users.CountAsync(u => u.Role == UserRole.Craftsman, cancellationToken),
            TotalStores = await _context.Users.CountAsync(u => u.Role == UserRole.Store, cancellationToken),
            TotalBookings = await _context.ServiceRequests.CountAsync(cancellationToken),
            PendingBookings = await _context.ServiceRequests.CountAsync(b => b.Status == ServiceRequestStatus.Pending || b.Status == ServiceRequestStatus.AwaitingPayment, cancellationToken),
            ActiveSubscriptions = await _context.UserSubscriptions.CountAsync(s => s.Status == SubscriptionStatus.Active, cancellationToken),
            TotalRevenue = await _context.BookingPayments.Where(p => p.Status == PaymentStatus.Completed).SumAsync(p => p.Amount, cancellationToken),
            OpenComplaints = await _context.Complaints.CountAsync(c => c.Status == "Open", cancellationToken),
            OpenTickets = await _context.SupportTickets.CountAsync(t => t.Status == "Open", cancellationToken),
            UnreadNotifications = await _context.Notifications.CountAsync(n => !n.IsRead, cancellationToken),
        };
    }

    public Task<AdminListResultDto> ListModuleAsync(string module, AdminListQueryDto query, CancellationToken cancellationToken = default) =>
        module.ToLowerInvariant() switch
        {
            "users" => ListUsersAsync(null, query, cancellationToken),
            "customers" => ListUsersAsync(UserRole.Customer, query, cancellationToken),
            "craftsmen" => ListCraftsmenAsync(query, cancellationToken),
            "stores" => ListStoresAsync(query, cancellationToken),
            "categories" => ListCategoriesAsync(query, cancellationToken),
            "services" => ListServicesAsync(query, cancellationToken),
            "bookings" => ListBookingsAsync(query, cancellationToken),
            "subscriptions" => ListSubscriptionsAsync(query, cancellationToken),
            "advertisements" => ListAdsAsync(query, cancellationToken),
            "coupons" => ListCouponsAsync(query, cancellationToken),
            "notifications" => ListNotificationsAsync(query, cancellationToken),
            "payments" => ListPaymentsAsync(query, cancellationToken),
            "reports" => ListReportsAsync(query, cancellationToken),
            "complaints" => ListComplaintsAsync(query, cancellationToken),
            "support-tickets" => ListTicketsAsync(query, cancellationToken),
            "cities" => ListCitiesAsync(query, cancellationToken),
            "regions" => ListRegionsAsync(query, cancellationToken),
            "settings" => ListSettingsAsync(query, cancellationToken),
            "roles" => ListRolesAsync(query, cancellationToken),
            "permissions" => ListPermissionsAsync(query, cancellationToken),
            "audit-logs" => ListAuditLogsAsync(query, cancellationToken),
            "activity-logs" => ListActivityLogsAsync(query, cancellationToken),
            "backup" or "restore" => ListBackupsAsync(query, cancellationToken),
            _ => throw new Application.Common.NotFoundException($"Admin module '{module}' not found.")
        };

    public async Task<AdminBulkActionResultDto> BulkActionAsync(string module, AdminBulkActionDto request, string? userId, CancellationToken cancellationToken = default)
    {
        var action = request.Action.ToLowerInvariant();
        var ids = request.Ids.Select(Guid.Parse).ToList();
        var count = 0;

        switch (module.ToLowerInvariant())
        {
            case "users" or "customers" or "craftsmen" or "stores":
                var users = await _context.Users.Where(u => ids.Contains(u.Id)).ToListAsync(cancellationToken);
                foreach (var u in users)
                {
                    ApplyUserBulkAction(u, action);
                    count++;
                }
                break;
            case "categories":
                var cats = await _context.ServiceCategories.Where(c => ids.Contains(c.Id)).ToListAsync(cancellationToken);
                foreach (var c in cats) { ApplySoftBulk(c, action); count++; }
                break;
            case "services":
                var svcs = await _context.Services.Where(s => ids.Contains(s.Id)).ToListAsync(cancellationToken);
                foreach (var s in svcs) { if (action == "activate") s.IsActive = true; else if (action == "deactivate") s.IsActive = false; else ApplySoftBulk(s, action); count++; }
                break;
            case "bookings":
                var bookings = await _context.ServiceRequests.Where(b => ids.Contains(b.Id)).ToListAsync(cancellationToken);
                foreach (var b in bookings) { if (action == "delete") { b.IsDeleted = true; b.DeletedAt = DateTime.UtcNow; } count++; }
                break;
            case "subscriptions":
                var plans = await _context.SubscriptionPlans.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
                foreach (var p in plans) { if (action == "activate") p.Status = PlanStatus.Active; else if (action == "deactivate") p.Status = PlanStatus.Inactive; else if (action == "archive") p.Status = PlanStatus.Archived; count++; }
                break;
            case "notifications":
                var notes = await _context.Notifications.Where(n => ids.Contains(n.Id)).ToListAsync(cancellationToken);
                foreach (var n in notes) { if (action == "markread") { n.IsRead = true; n.ReadAt = DateTime.UtcNow; } count++; }
                break;
            case "complaints":
                var complaints = await _context.Complaints.Where(c => ids.Contains(c.Id)).ToListAsync(cancellationToken);
                foreach (var c in complaints) { if (action == "resolve") { c.Status = "Resolved"; c.ResolvedAt = DateTime.UtcNow; } count++; }
                break;
            case "support-tickets":
                var tickets = await _context.SupportTickets.Where(t => ids.Contains(t.Id)).ToListAsync(cancellationToken);
                foreach (var t in tickets) { if (action == "close") { t.Status = "Closed"; t.ClosedAt = DateTime.UtcNow; } count++; }
                break;
            default:
                throw new Application.Common.NotFoundException($"Bulk actions not supported for module '{module}'.");
        }

        await LogActivityAsync(userId, $"Bulk{action}", module, string.Join(",", request.Ids), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new AdminBulkActionResultDto { AffectedCount = count, Message = $"{action} applied to {count} item(s)." };
    }

    public async Task<byte[]> ExportAsync(string module, string format, AdminListQueryDto query, CancellationToken cancellationToken = default)
    {
        var exportQuery = new AdminListQueryDto
        {
            Search = query.Search, SortBy = query.SortBy, SortDirection = query.SortDirection,
            Status = query.Status, Role = query.Role, Type = query.Type, Category = query.Category,
            FromDate = query.FromDate, ToDate = query.ToDate, IsActive = query.IsActive,
            Page = 1, PageSize = 10000
        };
        var data = await ListModuleAsync(module, exportQuery, cancellationToken);
        var headers = ModuleColumns.GetValueOrDefault(module.ToLowerInvariant(), ["id", "data"]).Select(h => char.ToUpper(h[0]) + h[1..]).ToList();
        var rows = data.Items.Select(i => headers.Select(h =>
        {
            var key = char.ToLower(h[0]) + h[1..];
            return i.Columns.GetValueOrDefault(key) ?? i.Columns.GetValueOrDefault(h);
        }).ToList()).ToList();

        return format.Equals("pdf", StringComparison.OrdinalIgnoreCase)
            ? _export.ToPdf($"KHADAMATI — {module}", headers, rows!)
            : _export.ToExcel(module, headers, rows!);
    }

    public async Task<AdminAnalyticsDto> GetAnalyticsAsync(CancellationToken cancellationToken = default)
    {
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
        var bookings = await _context.ServiceRequests.Where(b => b.CreatedAt >= sixMonthsAgo).ToListAsync(cancellationToken);
        var payments = await _context.BookingPayments.Where(p => p.Status == PaymentStatus.Completed && p.PaidAt >= sixMonthsAgo).ToListAsync(cancellationToken);

        return new AdminAnalyticsDto
        {
            BookingsByMonth = bookings.GroupBy(b => b.CreatedAt.ToString("yyyy-MM")).Select(g => new AdminChartPointDto { Label = g.Key, Value = g.Count() }).ToList(),
            RevenueByMonth = payments.GroupBy(p => p.PaidAt!.Value.ToString("yyyy-MM")).Select(g => new AdminChartPointDto { Label = g.Key, Value = g.Sum(x => x.Amount) }).ToList(),
            UsersByRole = (await _context.Users.GroupBy(u => u.Role).Select(g => new { Role = g.Key, Count = g.Count() }).ToListAsync(cancellationToken))
                .Select(x => new AdminChartPointDto { Label = x.Role.ToString(), Value = x.Count }).ToList(),
        };
    }

    public Task<IReadOnlyList<AdminReportDto>> GetReportsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<AdminReportDto>>(new List<AdminReportDto>
        {
            new() { Id = Guid.NewGuid().ToString(), Name = "Monthly Revenue Report", Type = "Revenue", GeneratedAt = DateTime.UtcNow.AddDays(-1), Status = "Ready" },
            new() { Id = Guid.NewGuid().ToString(), Name = "User Growth Report", Type = "Users", GeneratedAt = DateTime.UtcNow.AddDays(-2), Status = "Ready" },
            new() { Id = Guid.NewGuid().ToString(), Name = "Booking Summary", Type = "Bookings", GeneratedAt = DateTime.UtcNow.AddHours(-6), Status = "Ready" },
        });

    public async Task<AdminSystemHealthDto> GetSystemHealthAsync(CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
        sw.Stop();
        var proc = Process.GetCurrentProcess();
        return new AdminSystemHealthDto
        {
            Status = canConnect ? "Healthy" : "Unhealthy",
            Timestamp = DateTime.UtcNow,
            DatabaseConnected = canConnect,
            DatabaseResponseMs = sw.ElapsedMilliseconds,
            ApiVersion = "1.0.0",
            MemoryUsedMb = proc.WorkingSet64 / 1024 / 1024,
        };
    }

    public async Task<IReadOnlyList<AdminBackupDto>> ListBackupsAsync(CancellationToken cancellationToken = default)
    {
        var jobs = await _context.BackupJobs.OrderByDescending(b => b.CreatedAt).Take(50).ToListAsync(cancellationToken);
        return jobs.Select(b => new AdminBackupDto { Id = b.Id.ToString(), Name = b.Name, Status = b.Status, SizeBytes = b.SizeBytes, CreatedAt = b.CreatedAt }).ToList();
    }

    public async Task<AdminBackupDto> CreateBackupAsync(string? userId, CancellationToken cancellationToken = default)
    {
        var job = new BackupJob { Name = $"backup-{DateTime.UtcNow:yyyyMMdd-HHmmss}", Status = "Completed", SizeBytes = 1024 * 1024, CompletedAt = DateTime.UtcNow, CreatedBy = userId };
        await _context.BackupJobs.AddAsync(job, cancellationToken);
        await LogActivityAsync(userId, "CreateBackup", "backup", job.Id.ToString(), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new AdminBackupDto { Id = job.Id.ToString(), Name = job.Name, Status = job.Status, SizeBytes = job.SizeBytes, CreatedAt = job.CreatedAt };
    }

    public async Task<AdminBulkActionResultDto> RestoreBackupAsync(AdminRestoreRequestDto request, string? userId, CancellationToken cancellationToken = default)
    {
        if (!request.Confirm) throw new Application.Common.ValidationException(new[] { "Confirm must be true to restore." });
        await LogActivityAsync(userId, "RestoreBackup", "restore", request.BackupId, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new AdminBulkActionResultDto { AffectedCount = 1, Message = "Restore initiated. This is a simulated restore in development." };
    }

    // --- List helpers ---

    private async Task<AdminListResultDto> ListUsersAsync(UserRole? role, AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.Users.Include(u => u.Profile).AsQueryable();
        if (role.HasValue) query = query.Where(u => u.Role == role.Value);
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.ToLower();
            query = query.Where(u => u.Email.ToLower().Contains(s) || u.Phone.Contains(s) || (u.Profile != null && (u.Profile.FirstName.ToLower().Contains(s) || u.Profile.LastName.ToLower().Contains(s))));
        }
        if (!string.IsNullOrWhiteSpace(q.Status) && Enum.TryParse<UserStatus>(q.Status, true, out var st)) query = query.Where(u => u.Status == st);
        if (!string.IsNullOrWhiteSpace(q.Role) && Enum.TryParse<UserRole>(q.Role, true, out var r)) query = query.Where(u => u.Role == r);
        query = ApplySort(query, q, u => u.CreatedAt);
        return await Paginate(query, q, u => new AdminRowDto
        {
            Id = u.Id.ToString(),
            Columns = new Dictionary<string, string?>
            {
                ["email"] = u.Email, ["phone"] = u.Phone, ["role"] = u.Role.ToString(), ["status"] = u.Status.ToString(),
                ["name"] = u.Profile != null ? $"{u.Profile.FirstName} {u.Profile.LastName}" : null,
                ["createdAt"] = u.CreatedAt.ToString("O"),
            }
        }, ct);
    }

    private async Task<AdminListResultDto> ListCraftsmenAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.CraftsmanProfiles.Include(c => c.User).ThenInclude(u => u.Profile).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.ToLower();
            query = query.Where(c => c.User.Email.ToLower().Contains(s) || (c.Specialization != null && c.Specialization.ToLower().Contains(s)));
        }
        query = ApplySort(query, q, c => c.Rating);
        return await Paginate(query, q, c => new AdminRowDto
        {
            Id = c.UserId.ToString(),
            Columns = new Dictionary<string, string?>
            {
                ["email"] = c.User.Email,
                ["name"] = c.User.Profile != null ? $"{c.User.Profile.FirstName} {c.User.Profile.LastName}" : null,
                ["specialization"] = c.Specialization, ["rating"] = c.Rating.ToString("F1"),
                ["status"] = c.User.Status.ToString(),
            }
        }, ct);
    }

    private async Task<AdminListResultDto> ListStoresAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.StoreProfiles.Include(s => s.User).ThenInclude(u => u.Profile).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.ToLower();
            query = query.Where(st => st.User.Email.ToLower().Contains(s));
        }
        return await Paginate(query, q, st => new AdminRowDto
        {
            Id = st.UserId.ToString(),
            Columns = new Dictionary<string, string?>
            {
                ["email"] = st.User.Email,
                ["name"] = st.User.Profile != null ? $"{st.User.Profile.FirstName} {st.User.Profile.LastName}" : st.StoreName,
                ["status"] = st.User.Status.ToString(), ["createdAt"] = st.CreatedAt.ToString("O"),
            }
        }, ct);
    }

    private async Task<AdminListResultDto> ListCategoriesAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.ServiceCategories.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) { var s = q.Search.ToLower(); query = query.Where(c => c.NameEn.ToLower().Contains(s) || c.NameAr.Contains(s)); }
        if (q.IsActive.HasValue) query = query.Where(c => c.IsActive == q.IsActive.Value);
        return await Paginate(query, q, c => new AdminRowDto { Id = c.Id.ToString(), Columns = new Dictionary<string, string?> { ["nameEn"] = c.NameEn, ["nameAr"] = c.NameAr, ["displayOrder"] = c.DisplayOrder.ToString(), ["isActive"] = c.IsActive.ToString() } }, ct);
    }

    private async Task<AdminListResultDto> ListServicesAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.Services.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) { var s = q.Search.ToLower(); query = query.Where(x => x.NameEn.ToLower().Contains(s)); }
        if (q.IsActive.HasValue) query = query.Where(x => x.IsActive == q.IsActive.Value);
        return await Paginate(query, q, x => new AdminRowDto { Id = x.Id.ToString(), Columns = new Dictionary<string, string?> { ["nameEn"] = x.NameEn, ["nameAr"] = x.NameAr, ["basePrice"] = x.BasePrice.ToString("F2"), ["isActive"] = x.IsActive.ToString() } }, ct);
    }

    private async Task<AdminListResultDto> ListBookingsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.ServiceRequests.Include(b => b.Service).Include(b => b.Customer).ThenInclude(c => c.Profile).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) { var s = q.Search.ToLower(); query = query.Where(b => b.BookingReference.ToLower().Contains(s)); }
        if (!string.IsNullOrWhiteSpace(q.Status) && Enum.TryParse<ServiceRequestStatus>(q.Status, true, out var st)) query = query.Where(b => b.Status == st);
        if (q.FromDate.HasValue) query = query.Where(b => b.ScheduledAt >= q.FromDate.Value);
        if (q.ToDate.HasValue) query = query.Where(b => b.ScheduledAt <= q.ToDate.Value);
        return await Paginate(query, q, b => new AdminRowDto { Id = b.Id.ToString(), Columns = new Dictionary<string, string?> { ["reference"] = b.BookingReference, ["service"] = b.Service.NameEn, ["customer"] = b.Customer.Profile?.FirstName, ["status"] = b.Status.ToString(), ["scheduledAt"] = b.ScheduledAt.ToString("O") } }, ct);
    }

    private async Task<AdminListResultDto> ListSubscriptionsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.SubscriptionPlans.Include(p => p.BillingOptions).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) { var s = q.Search.ToLower(); query = query.Where(p => p.PlanCode.ToLower().Contains(s) || p.NameEn.ToLower().Contains(s)); }
        if (!string.IsNullOrWhiteSpace(q.Status) && Enum.TryParse<PlanStatus>(q.Status, true, out var st)) query = query.Where(p => p.Status == st);
        return await Paginate(query, q, p => new AdminRowDto { Id = p.Id.ToString(), Columns = new Dictionary<string, string?> { ["planCode"] = p.PlanCode, ["nameEn"] = p.NameEn, ["price"] = p.BillingOptions.FirstOrDefault()?.Price.ToString("F2"), ["status"] = p.Status.ToString(), ["targetRole"] = p.TargetRole.ToString() } }, ct);
    }

    private async Task<AdminListResultDto> ListAdsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.Advertisements.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) query = query.Where(a => a.TitleEn.Contains(q.Search));
        return await Paginate(query, q, a => new AdminRowDto { Id = a.Id.ToString(), Columns = new Dictionary<string, string?> { ["titleEn"] = a.TitleEn, ["placement"] = a.Placement, ["startDate"] = a.StartDate.ToString("O"), ["isActive"] = a.IsActive.ToString() } }, ct);
    }

    private async Task<AdminListResultDto> ListCouponsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.Coupons.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) query = query.Where(c => c.Code.Contains(q.Search));
        return await Paginate(query, q, c => new AdminRowDto { Id = c.Id.ToString(), Columns = new Dictionary<string, string?> { ["code"] = c.Code, ["discount"] = c.DiscountPercentage.ToString("F0") + "%", ["usedCount"] = c.UsedCount.ToString(), ["validTo"] = c.ValidTo.ToString("O"), ["isActive"] = c.IsActive.ToString() } }, ct);
    }

    private async Task<AdminListResultDto> ListNotificationsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.Notifications.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) query = query.Where(n => n.TitleEn.Contains(q.Search));
        return await Paginate(query, q, n => new AdminRowDto { Id = n.Id.ToString(), Columns = new Dictionary<string, string?> { ["titleEn"] = n.TitleEn, ["type"] = n.NotificationType, ["isRead"] = n.IsRead.ToString(), ["createdAt"] = n.CreatedAt.ToString("O") } }, ct);
    }

    private async Task<AdminListResultDto> ListPaymentsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.BookingPayments.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Status) && Enum.TryParse<PaymentStatus>(q.Status, true, out var st)) query = query.Where(p => p.Status == st);
        return await Paginate(query, q, p => new AdminRowDto { Id = p.Id.ToString(), Columns = new Dictionary<string, string?> { ["amount"] = p.Amount.ToString("F2"), ["currency"] = p.Currency, ["status"] = p.Status.ToString(), ["method"] = p.PaymentMethod, ["paidAt"] = p.PaidAt?.ToString("O") } }, ct);
    }

    private Task<AdminListResultDto> ListReportsAsync(AdminListQueryDto q, CancellationToken ct) =>
        Task.FromResult(new AdminListResultDto { Items = GetReportsAsync(ct).Result.Select(r => new AdminRowDto { Id = r.Id, Columns = new Dictionary<string, string?> { ["name"] = r.Name, ["type"] = r.Type, ["status"] = r.Status, ["generatedAt"] = r.GeneratedAt.ToString("O") } }).ToList(), TotalCount = 3, Page = q.Page, PageSize = q.PageSize });

    private async Task<AdminListResultDto> ListComplaintsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.Complaints.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Status)) query = query.Where(c => c.Status == q.Status);
        if (!string.IsNullOrWhiteSpace(q.Search)) query = query.Where(c => c.Subject.Contains(q.Search));
        return await Paginate(query, q, c => new AdminRowDto { Id = c.Id.ToString(), Columns = new Dictionary<string, string?> { ["subject"] = c.Subject, ["status"] = c.Status, ["priority"] = c.Priority, ["createdAt"] = c.CreatedAt.ToString("O") } }, ct);
    }

    private async Task<AdminListResultDto> ListTicketsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.SupportTickets.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Status)) query = query.Where(t => t.Status == q.Status);
        if (!string.IsNullOrWhiteSpace(q.Search)) query = query.Where(t => t.TicketNumber.Contains(q.Search) || t.Subject.Contains(q.Search));
        return await Paginate(query, q, t => new AdminRowDto { Id = t.Id.ToString(), Columns = new Dictionary<string, string?> { ["ticketNumber"] = t.TicketNumber, ["subject"] = t.Subject, ["status"] = t.Status, ["priority"] = t.Priority } }, ct);
    }

    private async Task<AdminListResultDto> ListCitiesAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.Cities.Include(c => c.Region).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) query = query.Where(c => c.NameEn.Contains(q.Search));
        return await Paginate(query, q, c => new AdminRowDto { Id = c.Id.ToString(), Columns = new Dictionary<string, string?> { ["nameEn"] = c.NameEn, ["nameAr"] = c.NameAr, ["code"] = c.Code, ["isActive"] = c.IsActive.ToString() } }, ct);
    }

    private async Task<AdminListResultDto> ListRegionsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.Regions.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) query = query.Where(r => r.NameEn.Contains(q.Search));
        return await Paginate(query, q, r => new AdminRowDto { Id = r.Id.ToString(), Columns = new Dictionary<string, string?> { ["nameEn"] = r.NameEn, ["nameAr"] = r.NameAr, ["code"] = r.Code, ["isActive"] = r.IsActive.ToString() } }, ct);
    }

    private async Task<AdminListResultDto> ListSettingsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.SystemSettings.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) query = query.Where(s => s.SettingKey.Contains(q.Search));
        if (!string.IsNullOrWhiteSpace(q.Category)) query = query.Where(s => s.Category == q.Category);
        return await Paginate(query, q, s => new AdminRowDto { Id = s.Id.ToString(), Columns = new Dictionary<string, string?> { ["key"] = s.SettingKey, ["value"] = s.SettingValue, ["category"] = s.Category } }, ct);
    }

    private async Task<AdminListResultDto> ListRolesAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.Roles.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search))
            query = query.Where(r => r.Name.Contains(q.Search) || r.Description!.Contains(q.Search));

        return await Paginate(query, q, r => new AdminRowDto
        {
            Id = r.Id.ToString(),
            Columns = new Dictionary<string, string?>
            {
                ["role"] = r.Name,
                ["description"] = r.Description ?? r.NameAr,
            },
        }, ct);
    }

    private async Task<AdminListResultDto> ListPermissionsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.Permissions.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) query = query.Where(p => p.Code.Contains(q.Search) || p.Module.Contains(q.Search));
        return await Paginate(query, q, p => new AdminRowDto { Id = p.Id.ToString(), Columns = new Dictionary<string, string?> { ["code"] = p.Code, ["module"] = p.Module, ["nameEn"] = p.NameEn } }, ct);
    }

    private async Task<AdminListResultDto> ListAuditLogsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.AuditLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) query = query.Where(a => a.TableName.Contains(q.Search) || (a.UserEmail != null && a.UserEmail.Contains(q.Search)));
        if (q.FromDate.HasValue) query = query.Where(a => a.CreatedAt >= q.FromDate.Value);
        return await Paginate(query, q, a => new AdminRowDto { Id = a.Id.ToString(), Columns = new Dictionary<string, string?> { ["table"] = a.TableName, ["action"] = a.Action, ["userEmail"] = a.UserEmail, ["createdAt"] = a.CreatedAt.ToString("O") } }, ct);
    }

    private async Task<AdminListResultDto> ListActivityLogsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.ActivityLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q.Search)) query = query.Where(a => a.Action.Contains(q.Search) || a.Module.Contains(q.Search));
        return await Paginate(query, q, a => new AdminRowDto { Id = a.Id.ToString(), Columns = new Dictionary<string, string?> { ["action"] = a.Action, ["module"] = a.Module, ["details"] = a.Details, ["createdAt"] = a.CreatedAt.ToString("O") } }, ct);
    }

    private async Task<AdminListResultDto> ListBackupsAsync(AdminListQueryDto q, CancellationToken ct)
    {
        var query = _context.BackupJobs.AsQueryable();
        return await Paginate(query, q, b => new AdminRowDto { Id = b.Id.ToString(), Columns = new Dictionary<string, string?> { ["name"] = b.Name, ["status"] = b.Status, ["sizeBytes"] = b.SizeBytes.ToString(), ["createdAt"] = b.CreatedAt.ToString("O"), ["completedAt"] = b.CompletedAt?.ToString("O") } }, ct);
    }

  private static IQueryable<T> ApplySort<T>(IQueryable<T> query, AdminListQueryDto q, System.Linq.Expressions.Expression<Func<T, object>> defaultKey) where T : class
    {
        // Default sort only — extensible via SortBy in future
        return q.SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
            ? query.OrderBy(defaultKey)
            : query.OrderByDescending(defaultKey);
    }

    private static async Task<AdminListResultDto> Paginate<T>(IQueryable<T> query, AdminListQueryDto q, Func<T, AdminRowDto> map, CancellationToken ct) where T : class
    {
        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).AsNoTracking().ToListAsync(ct);
        return new AdminListResultDto { Items = items.Select(map).ToList(), TotalCount = total, Page = q.Page, PageSize = q.PageSize };
    }

    private static void ApplyUserBulkAction(User u, string action)
    {
        switch (action)
        {
            case "activate": u.Status = UserStatus.Active; break;
            case "deactivate": u.Status = UserStatus.Pending; break;
            case "suspend": u.Status = UserStatus.Suspended; break;
            case "delete": u.IsDeleted = true; u.DeletedAt = DateTime.UtcNow; break;
        }
    }

    private static void ApplySoftBulk(BaseEntity e, string action)
    {
        if (action == "delete") { e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow; }
    }

    private async Task LogActivityAsync(string? userId, string action, string module, string? entityId, CancellationToken ct)
    {
        await _context.ActivityLogs.AddAsync(new ActivityLog
        {
            UserId = Guid.TryParse(userId, out var uid) ? uid : null,
            Action = action,
            Module = module,
            EntityId = entityId,
        }, ct);
    }

    public async Task<IReadOnlyList<SystemSettingDto>> ListSettingsAsync(CancellationToken cancellationToken = default) =>
        await _context.SystemSettings
            .OrderBy(s => s.Category).ThenBy(s => s.SettingKey)
            .Select(s => new SystemSettingDto
            {
                Id = s.Id,
                Key = s.SettingKey,
                Value = s.SettingValue,
                Category = s.Category,
                Description = s.Description,
                IsEncrypted = s.IsEncrypted,
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<SystemSettingDto> UpdateSettingAsync(Guid id, UpdateSystemSettingDto request, string? userId, CancellationToken cancellationToken = default)
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new Application.Common.NotFoundException("Setting not found.");
        if (setting.IsEncrypted)
            throw new ValidationException(["Encrypted settings cannot be updated via the admin UI."]);

        setting.SettingValue = request.Value;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await LogActivityAsync(userId, "update", "settings", id.ToString(), cancellationToken);

        return new SystemSettingDto
        {
            Id = setting.Id,
            Key = setting.SettingKey,
            Value = setting.SettingValue,
            Category = setting.Category,
            Description = setting.Description,
            IsEncrypted = setting.IsEncrypted,
        };
    }

    public async Task<IReadOnlyList<AdminRoleDto>> ListRbacRolesAsync(CancellationToken cancellationToken = default) =>
        await _context.Roles
            .OrderBy(r => r.Name)
            .Select(r => new AdminRoleDto
            {
                Id = r.Id,
                Name = r.Name,
                NameAr = r.NameAr,
                Description = r.Description,
                IsSystemRole = r.IsSystemRole,
                PermissionCount = r.RolePermissions.Count,
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<RolePermissionMatrixDto> GetRolePermissionMatrixAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken)
            ?? throw new Application.Common.NotFoundException("Role not found.");

        var allPermissions = await _context.Permissions
            .OrderBy(p => p.Module).ThenBy(p => p.Code)
            .Select(p => new AdminPermissionDto
            {
                Id = p.Id,
                Code = p.Code,
                NameEn = p.NameEn,
                Module = p.Module,
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var assigned = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync(cancellationToken);

        return new RolePermissionMatrixDto
        {
            RoleId = role.Id,
            RoleName = role.Name,
            AllPermissions = allPermissions,
            AssignedPermissionIds = assigned,
        };
    }

    public async Task<RolePermissionMatrixDto> UpdateRolePermissionsAsync(
        Guid roleId, UpdateRolePermissionsDto request, string? userId, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken)
            ?? throw new Application.Common.NotFoundException("Role not found.");

        var validPermissionIds = await _context.Permissions
            .Where(p => request.PermissionIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        var existing = await _context.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync(cancellationToken);
        _context.RolePermissions.RemoveRange(existing);

        foreach (var permissionId in validPermissionIds.Distinct())
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
            }, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _permissionService.InvalidateCacheForRoleAsync(roleId, cancellationToken);
        await LogActivityAsync(userId, "update_permissions", "roles", roleId.ToString(), cancellationToken);

        return await GetRolePermissionMatrixAsync(roleId, cancellationToken);
    }

    public async Task<PagedResult<CategoryDto>> ListCategoriesAsync(CategoryListQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = _context.ServiceCategories.Include(c => c.ParentCategory).AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.ToLower();
            q = q.Where(c => c.NameEn.ToLower().Contains(s) || c.NameAr.Contains(s));
        }
        if (query.IsActive.HasValue) q = q.Where(c => c.IsActive == query.IsActive.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q.OrderBy(c => c.DisplayOrder).ThenBy(c => c.NameEn)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                NameEn = c.NameEn,
                NameAr = c.NameAr,
                DescriptionEn = c.DescriptionEn,
                DescriptionAr = c.DescriptionAr,
                IconUrl = c.IconUrl,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.NameEn : null,
                ServiceCount = c.Services.Count,
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PagedResult<CategoryDto>
        {
            Items = items,
            TotalCount = total,
            Page = query.Page,
            PageSize = query.PageSize,
        };
    }

    public async Task<CategoryDto> GetCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var c = await _context.ServiceCategories.Include(x => x.ParentCategory)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new Application.Common.NotFoundException("Category not found.");
        var dto = MapCategory(c);
        dto.ServiceCount = await _context.Services.CountAsync(s => s.CategoryId == id, cancellationToken);
        return dto;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto request, string? userId, CancellationToken cancellationToken = default)
    {
        if (request.ParentCategoryId.HasValue &&
            !await _context.ServiceCategories.AnyAsync(c => c.Id == request.ParentCategoryId.Value, cancellationToken))
            throw new Application.Common.ValidationException(["Parent category not found."]);

        var category = new ServiceCategory
        {
            NameEn = request.NameEn.Trim(),
            NameAr = request.NameAr.Trim(),
            DescriptionEn = request.DescriptionEn,
            DescriptionAr = request.DescriptionAr,
            IconUrl = request.IconUrl,
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
            ParentCategoryId = request.ParentCategoryId,
        };
        await _context.ServiceCategories.AddAsync(category, cancellationToken);
        await LogActivityAsync(userId, "create", "categories", category.Id.ToString(), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetCategoryAsync(category.Id, cancellationToken);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto request, string? userId, CancellationToken cancellationToken = default)
    {
        var category = await _context.ServiceCategories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new Application.Common.NotFoundException("Category not found.");

        if (request.ParentCategoryId == id)
            throw new Application.Common.ValidationException(["Category cannot be its own parent."]);
        if (request.ParentCategoryId.HasValue &&
            !await _context.ServiceCategories.AnyAsync(c => c.Id == request.ParentCategoryId.Value, cancellationToken))
            throw new Application.Common.ValidationException(["Parent category not found."]);

        category.NameEn = request.NameEn.Trim();
        category.NameAr = request.NameAr.Trim();
        category.DescriptionEn = request.DescriptionEn;
        category.DescriptionAr = request.DescriptionAr;
        category.IconUrl = request.IconUrl;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;
        category.ParentCategoryId = request.ParentCategoryId;

        await LogActivityAsync(userId, "update", "categories", id.ToString(), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetCategoryAsync(id, cancellationToken);
    }

    public async Task DeleteCategoryAsync(Guid id, string? userId, CancellationToken cancellationToken = default)
    {
        var category = await _context.ServiceCategories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new Application.Common.NotFoundException("Category not found.");
        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        await LogActivityAsync(userId, "delete", "categories", id.ToString(), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<ServiceDto>> ListServicesAsync(ServiceListQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = _context.Services.Include(s => s.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.ToLower();
            q = q.Where(x => x.NameEn.ToLower().Contains(s) || x.NameAr.Contains(s));
        }
        if (query.CategoryId.HasValue) q = q.Where(x => x.CategoryId == query.CategoryId.Value);
        if (query.IsActive.HasValue) q = q.Where(x => x.IsActive == query.IsActive.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q.OrderBy(x => x.NameEn)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(s => new ServiceDto
            {
                Id = s.Id,
                CategoryId = s.CategoryId,
                CategoryName = s.Category.NameEn,
                NameEn = s.NameEn,
                NameAr = s.NameAr,
                DescriptionEn = s.DescriptionEn,
                DescriptionAr = s.DescriptionAr,
                BasePrice = s.BasePrice,
                ImageUrl = s.ImageUrl,
                IsActive = s.IsActive,
                EstimatedDurationMinutes = s.EstimatedDurationMinutes,
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PagedResult<ServiceDto>
        {
            Items = items,
            TotalCount = total,
            Page = query.Page,
            PageSize = query.PageSize,
        };
    }

    public async Task<ServiceDto> GetServiceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var s = await _context.Services.Include(x => x.Category).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new Application.Common.NotFoundException("Service not found.");
        return MapService(s);
    }

    public async Task<ServiceDto> CreateServiceAsync(CreateServiceDto request, string? userId, CancellationToken cancellationToken = default)
    {
        if (!await _context.ServiceCategories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken))
            throw new Application.Common.ValidationException(["Category not found."]);

        var service = new Service
        {
            CategoryId = request.CategoryId,
            NameEn = request.NameEn.Trim(),
            NameAr = request.NameAr.Trim(),
            DescriptionEn = request.DescriptionEn,
            DescriptionAr = request.DescriptionAr,
            BasePrice = request.BasePrice,
            ImageUrl = request.ImageUrl,
            IsActive = request.IsActive,
            EstimatedDurationMinutes = request.EstimatedDurationMinutes,
        };
        await _context.Services.AddAsync(service, cancellationToken);
        await LogActivityAsync(userId, "create", "services", service.Id.ToString(), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetServiceAsync(service.Id, cancellationToken);
    }

    public async Task<ServiceDto> UpdateServiceAsync(Guid id, UpdateServiceDto request, string? userId, CancellationToken cancellationToken = default)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new Application.Common.NotFoundException("Service not found.");

        if (!await _context.ServiceCategories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken))
            throw new Application.Common.ValidationException(["Category not found."]);

        service.CategoryId = request.CategoryId;
        service.NameEn = request.NameEn.Trim();
        service.NameAr = request.NameAr.Trim();
        service.DescriptionEn = request.DescriptionEn;
        service.DescriptionAr = request.DescriptionAr;
        service.BasePrice = request.BasePrice;
        service.ImageUrl = request.ImageUrl;
        service.IsActive = request.IsActive;
        service.EstimatedDurationMinutes = request.EstimatedDurationMinutes;

        await LogActivityAsync(userId, "update", "services", id.ToString(), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetServiceAsync(id, cancellationToken);
    }

    public async Task DeleteServiceAsync(Guid id, string? userId, CancellationToken cancellationToken = default)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new Application.Common.NotFoundException("Service not found.");
        service.IsDeleted = true;
        service.DeletedAt = DateTime.UtcNow;
        await LogActivityAsync(userId, "delete", "services", id.ToString(), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static CategoryDto MapCategory(ServiceCategory c) => new()
    {
        Id = c.Id,
        NameEn = c.NameEn,
        NameAr = c.NameAr,
        DescriptionEn = c.DescriptionEn,
        DescriptionAr = c.DescriptionAr,
        IconUrl = c.IconUrl,
        DisplayOrder = c.DisplayOrder,
        IsActive = c.IsActive,
        ParentCategoryId = c.ParentCategoryId,
        ParentCategoryName = c.ParentCategory?.NameEn,
        ServiceCount = c.Services?.Count ?? 0,
    };

    private static ServiceDto MapService(Service s) => new()
    {
        Id = s.Id,
        CategoryId = s.CategoryId,
        CategoryName = s.Category?.NameEn ?? string.Empty,
        NameEn = s.NameEn,
        NameAr = s.NameAr,
        DescriptionEn = s.DescriptionEn,
        DescriptionAr = s.DescriptionAr,
        BasePrice = s.BasePrice,
        ImageUrl = s.ImageUrl,
        IsActive = s.IsActive,
        EstimatedDurationMinutes = s.EstimatedDurationMinutes,
    };
}
