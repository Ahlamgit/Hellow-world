using Khadamati.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khadamati.Infrastructure.Data.Configurations;

public class VerificationDocumentConfiguration : IEntityTypeConfiguration<VerificationDocument>
{
    public void Configure(EntityTypeBuilder<VerificationDocument> builder)
    {
        builder.ToTable("VerificationDocuments");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.DocumentType).HasMaxLength(100).IsRequired();
        builder.Property(v => v.DocumentUrl).HasMaxLength(1000).IsRequired();
        builder.Property(v => v.Status).HasColumnName("VerificationStatusId").HasConversion<int>();
        builder.Property(v => v.RejectionReason).HasMaxLength(500);
        builder.HasOne(v => v.User).WithMany().HasForeignKey(v => v.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(v => v.ReviewedBy).WithMany().HasForeignKey(v => v.ReviewedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(v => new { v.UserId, v.Status });
    }
}
