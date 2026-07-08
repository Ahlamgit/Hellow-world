using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Users;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Interfaces;
using MediatR;
using AutoMapper;

namespace Khadamati.Application.Features.Users.Queries;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileDto>;
public record UpdateUserProfileCommand(Guid UserId, UpdateProfileDto Request) : IRequest<UserProfileDto>;
public record AddAddressCommand(Guid UserId, CreateAddressDto Request) : IRequest<AddressDto>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserProfileQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        return _mapper.Map<UserProfileDto>(user);
    }
}

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserProfileDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (user.Profile == null)
        {
            user.Profile = new UserProfile { UserId = user.Id };
            await _unitOfWork.Repository<UserProfile>().AddAsync(user.Profile, cancellationToken);
        }

        user.Profile.FirstName = request.Request.FirstName;
        user.Profile.LastName = request.Request.LastName;
        user.Profile.Bio = request.Request.Bio;
        user.Profile.PreferredLanguage = request.Request.PreferredLanguage;
        user.Profile.UpdatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<UserProfile>().Update(user.Profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserProfileDto>(user);
    }
}

public class AddAddressCommandHandler : IRequestHandler<AddAddressCommand, AddressDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddAddressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AddressDto> Handle(AddAddressCommand request, CancellationToken cancellationToken)
    {
        var address = _mapper.Map<Address>(request.Request);
        address.UserId = request.UserId;

        if (request.Request.IsDefault)
        {
            var existing = await _unitOfWork.Repository<Address>()
                .FindAsync(a => a.UserId == request.UserId && a.IsDefault, cancellationToken);
            foreach (var addr in existing)
            {
                addr.IsDefault = false;
                _unitOfWork.Repository<Address>().Update(addr);
            }
        }

        await _unitOfWork.Repository<Address>().AddAsync(address, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AddressDto>(address);
    }
}
