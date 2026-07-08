using Khadamati.Application.Common;
using Khadamati.Application.DTOs.Users;

namespace Khadamati.Application.Interfaces;

public interface IUserManagementService
{
    Task<AdminUserDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<AdminUserListItemDto>> SearchAsync(AdminUserListQueryDto query, CancellationToken cancellationToken = default);
    Task<AdminUserDetailDto> CreateAsync(CreateAdminUserDto dto, Guid adminUserId, string? ipAddress, CancellationToken cancellationToken = default);
    Task<AdminUserDetailDto> UpdateAsync(Guid id, UpdateAdminUserDto dto, Guid adminUserId, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, Guid adminUserId, CancellationToken cancellationToken = default);
    Task<UserActionResponseDto> SuspendAsync(Guid id, SuspendUserDto dto, Guid adminUserId, CancellationToken cancellationToken = default);
    Task<UserActionResponseDto> ActivateAsync(Guid id, Guid adminUserId, CancellationToken cancellationToken = default);
    Task<AdminUserDetailDto> AssignRolesAsync(Guid id, AssignUserRolesDto dto, Guid adminUserId, CancellationToken cancellationToken = default);
}
