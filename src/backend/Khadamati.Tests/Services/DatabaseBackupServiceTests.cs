using FluentAssertions;
using Khadamati.Domain.Entities;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Services.Backup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Khadamati.Tests.Services;

public class DatabaseBackupServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DatabaseBackupService _service;
    private readonly string _storagePath;

    public DatabaseBackupServiceTests()
    {
        _storagePath = Path.Combine(Path.GetTempPath(), "khadamati-backup-tests", Guid.NewGuid().ToString());
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _context = new ApplicationDbContext(options);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Backup:StoragePath"] = _storagePath })
            .Build();
        _service = new DatabaseBackupService(_context, config, NullLogger<DatabaseBackupService>.Instance);
        SeedData();
    }

    private void SeedData()
    {
        _context.Regions.Add(new Region { NameEn = "Riyadh", NameAr = "الرياض", Code = "RYD", IsActive = true });
        _context.SystemSettings.Add(new SystemSetting
        {
            SettingKey = "platform.name",
            SettingValue = "Khadamati",
            Category = "General",
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateAndRestoreBackup_RoundTripsCatalogData()
    {
        var created = await _service.CreateBackupAsync("test-backup");
        created.FilePath.Should().NotBeNullOrWhiteSpace();
        File.Exists(created.FilePath).Should().BeTrue();
        created.SizeBytes.Should().BeGreaterThan(0);

        _context.Regions.RemoveRange(_context.Regions);
        _context.SystemSettings.RemoveRange(_context.SystemSettings);
        await _context.SaveChangesAsync();
        (await _context.Regions.CountAsync()).Should().Be(0);

        var restored = await _service.RestoreBackupAsync(created.FilePath);
        restored.RecordsRestored.Should().BeGreaterThan(0);
        (await _context.Regions.CountAsync()).Should().Be(1);
        (await _context.SystemSettings.CountAsync()).Should().Be(1);
    }

    public void Dispose()
    {
        _context.Dispose();
        if (Directory.Exists(_storagePath))
            Directory.Delete(_storagePath, true);
    }
}
