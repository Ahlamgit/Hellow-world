using AutoMapper;
using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Services;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using MediatR;

namespace Khadamati.Application.Features.Services.Queries;

public record GetServiceCategoriesQuery : IRequest<IReadOnlyList<ServiceCategoryDto>>;
public record GetServicesQuery(Guid? CategoryId) : IRequest<IReadOnlyList<ServiceDto>>;
public record GetServiceRequestsQuery(Guid UserId, int Page = 1, int PageSize = 20) : IRequest<PagedResult<ServiceRequestDto>>;

public class GetServiceCategoriesQueryHandler : IRequestHandler<GetServiceCategoriesQuery, IReadOnlyList<ServiceCategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetServiceCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ServiceCategoryDto>> Handle(GetServiceCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Repository<ServiceCategory>()
            .FindAsync(c => !c.IsDeleted && c.IsActive && c.ParentCategoryId == null, cancellationToken);

        return _mapper.Map<IReadOnlyList<ServiceCategoryDto>>(categories);
    }
}

public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, IReadOnlyList<ServiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetServicesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ServiceDto>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _unitOfWork.Repository<Service>()
            .FindAsync(s => !s.IsDeleted && s.IsActive &&
                (request.CategoryId == null || s.CategoryId == request.CategoryId), cancellationToken);

        return _mapper.Map<IReadOnlyList<ServiceDto>>(services);
    }
}

public class CreateServiceRequestCommand : IRequest<ServiceRequestDto>
{
    public Guid CustomerId { get; init; }
    public CreateServiceRequestDto Request { get; init; } = null!;
}

public class CreateServiceRequestCommandHandler : IRequestHandler<CreateServiceRequestCommand, ServiceRequestDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateServiceRequestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceRequestDto> Handle(CreateServiceRequestCommand request, CancellationToken cancellationToken)
    {
        var service = await _unitOfWork.Repository<Service>().GetByIdAsync(request.Request.ServiceId, cancellationToken)
            ?? throw new NotFoundException("Service not found.");

        var serviceRequest = new ServiceRequest
        {
            CustomerId = request.CustomerId,
            ServiceId = request.Request.ServiceId,
            AddressId = request.Request.AddressId,
            Description = request.Request.Description,
            ScheduledAt = request.Request.ScheduledAt,
            EstimatedPrice = service.BasePrice,
            Status = ServiceRequestStatus.Pending
        };

        await _unitOfWork.Repository<ServiceRequest>().AddAsync(serviceRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        serviceRequest.Service = service;
        return _mapper.Map<ServiceRequestDto>(serviceRequest);
    }
}
