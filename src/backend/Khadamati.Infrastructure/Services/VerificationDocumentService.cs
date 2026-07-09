using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Verification;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Services;

public class VerificationDocumentService : IVerificationDocumentService
{
    private readonly ApplicationDbContext _context;

    public VerificationDocumentService(ApplicationDbContext context) => _context = context;

    public async Task<VerificationDocumentDto> SubmitAsync(
        Guid userId, SubmitVerificationDocumentDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.DocumentType) || string.IsNullOrWhiteSpace(request.DocumentUrl))
            throw new ValidationException(["Document type and URL are required."]);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (user.Role is not (UserRole.Craftsman or UserRole.Store))
            throw new ConflictException("Only craftsmen and store accounts can submit verification documents.");

        var document = new VerificationDocument
        {
            UserId = userId,
            DocumentType = request.DocumentType.Trim(),
            DocumentUrl = request.DocumentUrl.Trim(),
            Status = VerificationStatus.PendingReview,
        };

        user.VerificationStatus = VerificationStatus.PendingReview;
        await _context.VerificationDocuments.AddAsync(document, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return await MapAsync(document.Id, cancellationToken);
    }

    public async Task<PagedResult<VerificationDocumentDto>> AdminListAsync(
        VerificationDocumentListQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = _context.VerificationDocuments
            .Include(v => v.User)
            .AsNoTracking()
            .Where(v => !v.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<VerificationStatus>(query.Status, true, out var status))
        {
            q = q.Where(v => v.Status == status);
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(v => v.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<VerificationDocumentDto>
        {
            Items = items.Select(Map).ToList(),
            TotalCount = total,
            Page = query.Page,
            PageSize = query.PageSize,
        };
    }

    public Task<VerificationDocumentDto> AdminGetAsync(Guid id, CancellationToken cancellationToken = default) =>
        MapAsync(id, cancellationToken);

    public async Task<VerificationDocumentDto> ApproveAsync(
        Guid id, Guid reviewerId, CancellationToken cancellationToken = default)
    {
        var document = await _context.VerificationDocuments
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Verification document not found.");

        if (document.Status == VerificationStatus.Verified)
            throw new ConflictException("Document is already approved.");

        document.Status = VerificationStatus.Verified;
        document.ReviewedByUserId = reviewerId;
        document.ReviewedAt = DateTime.UtcNow;
        document.RejectionReason = null;
        document.User.VerificationStatus = VerificationStatus.Verified;

        await _context.SaveChangesAsync(cancellationToken);
        return Map(document);
    }

    public async Task<VerificationDocumentDto> RejectAsync(
        Guid id, Guid reviewerId, RejectVerificationDocumentDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            throw new ValidationException(["Rejection reason is required."]);

        var document = await _context.VerificationDocuments
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Verification document not found.");

        document.Status = VerificationStatus.Rejected;
        document.ReviewedByUserId = reviewerId;
        document.ReviewedAt = DateTime.UtcNow;
        document.RejectionReason = request.Reason.Trim();
        document.User.VerificationStatus = VerificationStatus.Rejected;

        await _context.SaveChangesAsync(cancellationToken);
        return Map(document);
    }

    private async Task<VerificationDocumentDto> MapAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await _context.VerificationDocuments
            .Include(v => v.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Verification document not found.");
        return Map(document);
    }

    private static VerificationDocumentDto Map(VerificationDocument document) => new()
    {
        Id = document.Id,
        UserId = document.UserId,
        UserEmail = document.User.Email,
        UserRole = document.User.Role.ToString(),
        DocumentType = document.DocumentType,
        DocumentUrl = document.DocumentUrl,
        Status = document.Status.ToString(),
        RejectionReason = document.RejectionReason,
        CreatedAt = document.CreatedAt,
        ReviewedAt = document.ReviewedAt,
    };
}
