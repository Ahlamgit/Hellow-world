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
