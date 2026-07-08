using Khadamati.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<PhoneOtpToken> PhoneOtpTokens => Set<PhoneOtpToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<CraftsmanProfile> CraftsmanProfiles => Set<CraftsmanProfile>();
    public DbSet<StoreProfile> StoreProfiles => Set<StoreProfile>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<CraftsmanService> CraftsmanServices => Set<CraftsmanService>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<StoreProduct> StoreProducts => Set<StoreProduct>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<PlanBillingOption> PlanBillingOptions => Set<PlanBillingOption>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<BookingSlotReservation> BookingSlotReservations => Set<BookingSlotReservation>();
    public DbSet<CraftsmanWorkingHour> CraftsmanWorkingHours => Set<CraftsmanWorkingHour>();
    public DbSet<BookingPayment> BookingPayments => Set<BookingPayment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ServiceRequestStatusHistory> ServiceRequestStatusHistories => Set<ServiceRequestStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(Domain.Common.BaseEntity).IsAssignableFrom(e.ClrType)))
        {
            var builder = modelBuilder.Entity(entityType.ClrType);
            AuditColumnConfiguration.ConfigureAuditColumns(builder);
        }

        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<UserProfile>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Address>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<EmailVerificationToken>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PasswordResetToken>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PhoneOtpToken>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CraftsmanProfile>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<StoreProfile>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ServiceCategory>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Service>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CraftsmanService>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ServiceRequest>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<StoreProduct>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<SubscriptionPlan>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PlanBillingOption>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<UserSubscription>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<BookingSlotReservation>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CraftsmanWorkingHour>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<BookingPayment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Notification>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ServiceRequestStatusHistory>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
