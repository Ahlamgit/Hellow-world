using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupService> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromHours(6);

    public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var expired = await context.RefreshTokens
                    .Where(t => t.ExpiresAt < DateTime.UtcNow || t.RevokedAt != null)
                    .Where(t => t.DeletedAt == null)
                    .ToListAsync(stoppingToken);

                foreach (var token in expired)
                {
                    token.IsDeleted = true;
                    token.DeletedAt = DateTime.UtcNow;
                }

                if (expired.Count > 0)
                {
                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Cleaned up {Count} expired refresh tokens", expired.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token cleanup");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}
