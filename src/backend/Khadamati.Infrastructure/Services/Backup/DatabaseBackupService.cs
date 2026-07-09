using System.IO.Compression;
using System.Text.Json;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services.Backup;

/// <summary>
/// Exports and restores catalog/configuration tables as a gzip-compressed JSON snapshot.
/// </summary>
public class DatabaseBackupService : IDatabaseBackupService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseBackupService> _logger;

    public DatabaseBackupService(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<DatabaseBackupService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    private string StoragePath =>
        _configuration["Backup:StoragePath"]?.Trim() ?? Path.Combine(AppContext.BaseDirectory, "backups");

    public async Task<BackupFileResult> CreateBackupAsync(string jobName, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(StoragePath);

        var snapshot = new CatalogBackupSnapshot
        {
            ExportedAt = DateTime.UtcNow,
            SystemSettings = await _context.SystemSettings.AsNoTracking().Where(s => !s.IsDeleted).ToListAsync(cancellationToken),
            Categories = await _context.ServiceCategories.AsNoTracking().Where(c => !c.IsDeleted).ToListAsync(cancellationToken),
            Services = await _context.Services.AsNoTracking().Where(s => !s.IsDeleted).ToListAsync(cancellationToken),
            Regions = await _context.Regions.AsNoTracking().Where(r => !r.IsDeleted).ToListAsync(cancellationToken),
            Cities = await _context.Cities.AsNoTracking().Where(c => !c.IsDeleted).ToListAsync(cancellationToken),
            Coupons = await _context.Coupons.AsNoTracking().Where(c => !c.IsDeleted).ToListAsync(cancellationToken),
            Advertisements = await _context.Advertisements.AsNoTracking().Where(a => !a.IsDeleted).ToListAsync(cancellationToken),
        };

        var fileName = $"{Sanitize(jobName)}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json.gz";
        var fullPath = Path.Combine(StoragePath, fileName);

        await using (var fileStream = File.Create(fullPath))
        await using (var gzip = new GZipStream(fileStream, CompressionLevel.Optimal))
        {
            await JsonSerializer.SerializeAsync(gzip, snapshot, JsonOptions, cancellationToken);
        }

        var size = new FileInfo(fullPath).Length;
        _logger.LogInformation("Catalog backup written to {Path} ({Size} bytes)", fullPath, size);

        return new BackupFileResult { FilePath = fullPath, SizeBytes = size };
    }

    public async Task<BackupRestoreResult> RestoreBackupAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Backup file not found.", filePath);

        CatalogBackupSnapshot snapshot;
        await using (var fileStream = File.OpenRead(filePath))
        await using (var gzip = new GZipStream(fileStream, CompressionMode.Decompress))
        {
            snapshot = await JsonSerializer.DeserializeAsync<CatalogBackupSnapshot>(gzip, JsonOptions, cancellationToken)
                ?? throw new InvalidDataException("Backup file is empty or invalid.");
        }

        var restored = 0;
        await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

        restored += await UpsertAsync(_context.SystemSettings, snapshot.SystemSettings, cancellationToken);
        restored += await UpsertAsync(_context.Regions, snapshot.Regions, cancellationToken);
        restored += await UpsertAsync(_context.Cities, snapshot.Cities, cancellationToken);
        restored += await UpsertAsync(_context.ServiceCategories, snapshot.Categories, cancellationToken);
        restored += await UpsertAsync(_context.Services, snapshot.Services, cancellationToken);
        restored += await UpsertAsync(_context.Coupons, snapshot.Coupons, cancellationToken);
        restored += await UpsertAsync(_context.Advertisements, snapshot.Advertisements, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        _logger.LogInformation("Restored {Count} catalog records from {Path}", restored, filePath);

        return new BackupRestoreResult
        {
            RecordsRestored = restored,
            Message = $"Restored {restored} catalog/configuration records from backup.",
        };
    }

    public Task<Stream> OpenBackupStreamAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Backup file not found.", filePath);

        Stream fileStream = File.OpenRead(filePath);
        Stream gzip = new GZipStream(fileStream, CompressionMode.Decompress);
        return Task.FromResult(gzip);
    }

    private async Task<int> UpsertAsync<T>(
        DbSet<T> set, List<T> items, CancellationToken cancellationToken) where T : class
    {
        if (items.Count == 0) return 0;

        var keyProperty = typeof(T).GetProperty("Id")
            ?? throw new InvalidOperationException($"{typeof(T).Name} is missing Id property.");

        foreach (var item in items)
        {
            var id = keyProperty.GetValue(item);
            var existing = await set.FindAsync([id], cancellationToken);
            if (existing is null)
                await set.AddAsync(item, cancellationToken);
            else
                _context.Entry(existing).CurrentValues.SetValues(item);
        }

        return items.Count;
    }

    private static string Sanitize(string value) =>
        string.Concat(value.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '-' : c));

    private sealed class CatalogBackupSnapshot
    {
        public string Version { get; set; } = "1.0";
        public DateTime ExportedAt { get; set; }
        public List<SystemSetting> SystemSettings { get; set; } = [];
        public List<ServiceCategory> Categories { get; set; } = [];
        public List<Service> Services { get; set; } = [];
        public List<Region> Regions { get; set; } = [];
        public List<City> Cities { get; set; } = [];
        public List<Coupon> Coupons { get; set; } = [];
        public List<Advertisement> Advertisements { get; set; } = [];
    }
}
