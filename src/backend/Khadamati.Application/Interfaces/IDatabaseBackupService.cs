namespace Khadamati.Application.Interfaces;

public interface IDatabaseBackupService
{
    Task<BackupFileResult> CreateBackupAsync(string jobName, CancellationToken cancellationToken = default);
    Task<BackupRestoreResult> RestoreBackupAsync(string filePath, CancellationToken cancellationToken = default);
    Task<Stream> OpenBackupStreamAsync(string filePath, CancellationToken cancellationToken = default);
}

public class BackupFileResult
{
    public string FilePath { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
}

public class BackupRestoreResult
{
    public int RecordsRestored { get; set; }
    public string Message { get; set; } = string.Empty;
}
