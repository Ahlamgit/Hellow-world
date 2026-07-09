using Khadamati.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khadamati.Infrastructure.Data.Configurations;

public class DevicePushTokenConfiguration : IEntityTypeConfiguration<DevicePushToken>
{
    public void Configure(EntityTypeBuilder<DevicePushToken> builder)
    {
        builder.ToTable("DevicePushTokens");
        builder.HasIndex(t => new { t.UserId, t.Token }).IsUnique();
        builder.Property(t => t.Token).HasMaxLength(512).IsRequired();
        builder.Property(t => t.Platform).HasMaxLength(32).IsRequired();
        builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ChatConversationConfiguration : IEntityTypeConfiguration<ChatConversation>
{
    public void Configure(EntityTypeBuilder<ChatConversation> builder)
    {
        builder.ToTable("ChatConversations");
        builder.HasIndex(c => c.BookingId).IsUnique();
        builder.HasOne(c => c.Booking).WithMany().HasForeignKey(c => c.BookingId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");
        builder.Property(m => m.Body).HasMaxLength(4000).IsRequired();
        builder.HasIndex(m => new { m.ConversationId, m.SentAt });
        builder.HasOne(m => m.Conversation).WithMany(c => c.Messages).HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Restrict);
    }
}
