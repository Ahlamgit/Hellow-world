using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Verification;
using Khadamati.Application.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Verification;

public record SubmitVerificationDocumentCommand(Guid UserId, SubmitVerificationDocumentDto Request)
    : IRequest<VerificationDocumentDto>;

public record ListVerificationDocumentsQuery(VerificationDocumentListQueryDto Query)
    : IRequest<PagedResult<VerificationDocumentDto>>;

public record GetVerificationDocumentQuery(Guid Id) : IRequest<VerificationDocumentDto>;

public record ApproveVerificationDocumentCommand(Guid Id, Guid ReviewerId) : IRequest<VerificationDocumentDto>;

public record RejectVerificationDocumentCommand(Guid Id, Guid ReviewerId, RejectVerificationDocumentDto Request)
    : IRequest<VerificationDocumentDto>;

public class SubmitVerificationDocumentCommandHandler : IRequestHandler<SubmitVerificationDocumentCommand, VerificationDocumentDto>
{
    private readonly IVerificationDocumentService _service;
    public SubmitVerificationDocumentCommandHandler(IVerificationDocumentService service) => _service = service;
    public Task<VerificationDocumentDto> Handle(SubmitVerificationDocumentCommand request, CancellationToken ct) =>
        _service.SubmitAsync(request.UserId, request.Request, ct);
}

public class ListVerificationDocumentsQueryHandler : IRequestHandler<ListVerificationDocumentsQuery, PagedResult<VerificationDocumentDto>>
{
    private readonly IVerificationDocumentService _service;
    public ListVerificationDocumentsQueryHandler(IVerificationDocumentService service) => _service = service;
    public Task<PagedResult<VerificationDocumentDto>> Handle(ListVerificationDocumentsQuery request, CancellationToken ct) =>
        _service.AdminListAsync(request.Query, ct);
}

public class GetVerificationDocumentQueryHandler : IRequestHandler<GetVerificationDocumentQuery, VerificationDocumentDto>
{
    private readonly IVerificationDocumentService _service;
    public GetVerificationDocumentQueryHandler(IVerificationDocumentService service) => _service = service;
    public Task<VerificationDocumentDto> Handle(GetVerificationDocumentQuery request, CancellationToken ct) =>
        _service.AdminGetAsync(request.Id, ct);
}

public class ApproveVerificationDocumentCommandHandler : IRequestHandler<ApproveVerificationDocumentCommand, VerificationDocumentDto>
{
    private readonly IVerificationDocumentService _service;
    public ApproveVerificationDocumentCommandHandler(IVerificationDocumentService service) => _service = service;
    public Task<VerificationDocumentDto> Handle(ApproveVerificationDocumentCommand request, CancellationToken ct) =>
        _service.ApproveAsync(request.Id, request.ReviewerId, ct);
}

public class RejectVerificationDocumentCommandHandler : IRequestHandler<RejectVerificationDocumentCommand, VerificationDocumentDto>
{
    private readonly IVerificationDocumentService _service;
    public RejectVerificationDocumentCommandHandler(IVerificationDocumentService service) => _service = service;
    public Task<VerificationDocumentDto> Handle(RejectVerificationDocumentCommand request, CancellationToken ct) =>
        _service.RejectAsync(request.Id, request.ReviewerId, request.Request, ct);
}

public record DownloadAdminBackupQuery(Guid Id) : IRequest<BackupDownloadDto>;

public class BackupDownloadDto
{
    public Stream Stream { get; set; } = Stream.Null;
    public string FileName { get; set; } = "backup.json.gz";
}

public class DownloadAdminBackupQueryHandler : IRequestHandler<DownloadAdminBackupQuery, BackupDownloadDto>
{
    private readonly IAdminService _admin;
    private readonly IDatabaseBackupService _backup;

    public DownloadAdminBackupQueryHandler(IAdminService admin, IDatabaseBackupService backup)
    {
        _admin = admin;
        _backup = backup;
    }

    public async Task<BackupDownloadDto> Handle(DownloadAdminBackupQuery request, CancellationToken ct)
    {
        var job = await _admin.GetBackupJobAsync(request.Id, ct);
        if (string.IsNullOrWhiteSpace(job.FilePath) || job.Status != "Completed")
            throw new ConflictException("Backup file is not available for download.");

        var stream = await _backup.OpenBackupStreamAsync(job.FilePath, ct);
        return new BackupDownloadDto
        {
            Stream = stream,
            FileName = Path.GetFileName(job.FilePath),
        };
    }
}
