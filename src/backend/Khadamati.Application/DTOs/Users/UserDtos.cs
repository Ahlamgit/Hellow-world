namespace Khadamati.Application.DTOs.Users;

public record AddressDto(
    Guid Id,
    string Label,
    string Street,
    string City,
    string? District,
    string? PostalCode,
    string Country,
    decimal? Latitude,
    decimal? Longitude,
    bool IsDefault);

public record CreateAddressDto(
    string Label,
    string Street,
    string City,
    string? District,
    string? PostalCode,
    string Country,
    decimal? Latitude,
    decimal? Longitude,
    bool IsDefault);

public record UpdateProfileDto(
    string FirstName,
    string LastName,
    string? Bio,
    string PreferredLanguage);

public record UserProfileDto(
    Guid Id,
    string Email,
    string Phone,
    string Role,
    string FirstName,
    string LastName,
    string? Bio,
    string? ProfilePictureUrl,
    string PreferredLanguage,
    IReadOnlyList<AddressDto> Addresses);
