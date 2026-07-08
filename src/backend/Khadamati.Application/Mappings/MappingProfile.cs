using AutoMapper;
using Khadamati.Application.DTOs.Auth;
using Khadamati.Application.DTOs.Services;
using Khadamati.Application.DTOs.Users;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;

namespace Khadamati.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.VerificationStatus, o => o.MapFrom(s => s.VerificationStatus.ToString()))
            .ForMember(d => d.SubscriptionStatus, o => o.MapFrom(s => s.SubscriptionStatus.ToString()))
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.Profile != null ? s.Profile.FirstName : string.Empty))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.Profile != null ? s.Profile.LastName : string.Empty))
            .ForMember(d => d.ProfilePictureUrl, o => o.MapFrom(s => s.Profile != null ? s.Profile.ProfilePictureUrl : null))
            .ForMember(d => d.PreferredLanguage, o => o.MapFrom(s => s.Profile != null ? s.Profile.PreferredLanguage : "ar"));

        CreateMap<User, UserProfileDto>()
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.Profile!.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.Profile!.LastName))
            .ForMember(d => d.Bio, o => o.MapFrom(s => s.Profile!.Bio))
            .ForMember(d => d.ProfilePictureUrl, o => o.MapFrom(s => s.Profile!.ProfilePictureUrl))
            .ForMember(d => d.PreferredLanguage, o => o.MapFrom(s => s.Profile!.PreferredLanguage))
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

        CreateMap<Address, AddressDto>();
        CreateMap<CreateAddressDto, Address>();

        CreateMap<ServiceCategory, ServiceCategoryDto>()
            .ForMember(d => d.SubCategories, o => o.MapFrom(s => s.SubCategories.Where(c => !c.IsDeleted && c.IsActive)));

        CreateMap<Service, ServiceDto>();

        CreateMap<ServiceRequest, ServiceRequestDto>()
            .ForMember(d => d.ServiceName, o => o.MapFrom(s => s.Service.NameEn))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.CraftsmanName, o => o.MapFrom(s =>
                s.Craftsman != null && s.Craftsman.Profile != null
                    ? $"{s.Craftsman.Profile.FirstName} {s.Craftsman.Profile.LastName}"
                    : null));
    }
}
