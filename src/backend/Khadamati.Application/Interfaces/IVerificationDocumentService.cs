using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Verification;

namespace Khadamati.Application.Interfaces;

public interface IVerificationDocumentService
{
    Task<VerificationDocumentDto> SubmitAsync(Guid userId, SubmitVerificationDocumentDto request, CancellationToken cancellationToken = default);
    Task<PagedResult<VerificationDocumentDto>> AdminListAsync(VerificationDocumentListQueryDto query, CancellationToken cancellationToken = default);
    Task<VerificationDocumentDto> AdminGetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VerificationDocumentDto> ApproveAsync(Guid id, Guid reviewerId, CancellationToken cancellationToken = default);
    Task<VerificationDocumentDto> RejectAsync(Guid id, Guid reviewerId, RejectVerificationDocumentDto request, CancellationToken cancellationToken = default);
}
