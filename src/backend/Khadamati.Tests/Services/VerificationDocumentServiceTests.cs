using FluentAssertions;
using Khadamati.Application.DTOs.Verification;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Tests.Services;

public class VerificationDocumentServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly VerificationDocumentService _service;

    public VerificationDocumentServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _service = new VerificationDocumentService(_context);
        SeedData();
    }

    private void SeedData()
    {
        var craftsman = new User
        {
            Email = "craftsman@test.com",
            Phone = "+966500000201",
            PasswordHash = "hash",
            Role = UserRole.Craftsman,
            Status = UserStatus.Active,
            VerificationStatus = VerificationStatus.Unverified,
            SubscriptionStatus = SubscriptionStatus.Active,
            Profile = new UserProfile { FirstName = "Craft", LastName = "Man", PreferredLanguage = "en" },
        };
        var admin = new User
        {
            Email = "admin@test.com",
            Phone = "+966500000202",
            PasswordHash = "hash",
            Role = UserRole.Administrator,
            Status = UserStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            SubscriptionStatus = SubscriptionStatus.Active,
            Profile = new UserProfile { FirstName = "Admin", LastName = "User", PreferredLanguage = "en" },
        };
        _context.Users.AddRange(craftsman, admin);
        _context.SaveChanges();
    }

    [Fact]
    public async Task SubmitAsync_SetsPendingReview()
    {
        var craftsman = await _context.Users.FirstAsync(u => u.Role == UserRole.Craftsman);
        var result = await _service.SubmitAsync(craftsman.Id, new SubmitVerificationDocumentDto
        {
            DocumentType = "National ID",
            DocumentUrl = "https://example.com/id.pdf",
        });

        result.Status.Should().Be("PendingReview");
        (await _context.Users.FindAsync(craftsman.Id))!.VerificationStatus.Should().Be(VerificationStatus.PendingReview);
    }

    [Fact]
    public async Task ApproveAsync_UpdatesUserVerificationStatus()
    {
        var craftsman = await _context.Users.FirstAsync(u => u.Role == UserRole.Craftsman);
        var admin = await _context.Users.FirstAsync(u => u.Role == UserRole.Administrator);
        var doc = await _service.SubmitAsync(craftsman.Id, new SubmitVerificationDocumentDto
        {
            DocumentType = "License",
            DocumentUrl = "https://example.com/license.pdf",
        });

        var approved = await _service.ApproveAsync(doc.Id, admin.Id);
        approved.Status.Should().Be("Verified");
        (await _context.Users.FindAsync(craftsman.Id))!.VerificationStatus.Should().Be(VerificationStatus.Verified);
    }

    [Fact]
    public async Task RejectAsync_RequiresReason()
    {
        var craftsman = await _context.Users.FirstAsync(u => u.Role == UserRole.Craftsman);
        var admin = await _context.Users.FirstAsync(u => u.Role == UserRole.Administrator);
        var doc = await _service.SubmitAsync(craftsman.Id, new SubmitVerificationDocumentDto
        {
            DocumentType = "License",
            DocumentUrl = "https://example.com/license.pdf",
        });

        var rejected = await _service.RejectAsync(doc.Id, admin.Id, new RejectVerificationDocumentDto
        {
            Reason = "Document is blurry.",
        });

        rejected.Status.Should().Be("Rejected");
        rejected.RejectionReason.Should().Be("Document is blurry.");
        (await _context.Users.FindAsync(craftsman.Id))!.VerificationStatus.Should().Be(VerificationStatus.Rejected);
    }

    public void Dispose() => _context.Dispose();
}
