using Khadamati.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khadamati.Infrastructure.Data;

public static class AuditColumnConfiguration
{
    public static void ConfigureAuditColumns<T>(EntityTypeBuilder<T> builder) where T : BaseEntity
    {
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedDate");
        builder.Property(e => e.UpdatedAt).HasColumnName("ModifiedDate");
        builder.Property(e => e.UpdatedBy).HasColumnName("ModifiedBy");
        builder.Property(e => e.IsDeleted).HasColumnName("Deleted");
        builder.Property(e => e.DeletedAt).HasColumnName("DeletedDate");
    }

    public static void ConfigureAuditColumns(EntityTypeBuilder builder)
    {
        builder.Property(nameof(BaseEntity.CreatedAt)).HasColumnName("CreatedDate");
        builder.Property(nameof(BaseEntity.UpdatedAt)).HasColumnName("ModifiedDate");
        builder.Property(nameof(BaseEntity.UpdatedBy)).HasColumnName("ModifiedBy");
        builder.Property(nameof(BaseEntity.IsDeleted)).HasColumnName("Deleted");
        builder.Property(nameof(BaseEntity.DeletedAt)).HasColumnName("DeletedDate");
    }
}
