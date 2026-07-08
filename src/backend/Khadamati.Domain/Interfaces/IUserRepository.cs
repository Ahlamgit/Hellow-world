using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;

namespace Khadamati.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<bool> PhoneExistsAsync(string phone, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<User> Items, int TotalCount)> SearchAsync(
        string? search,
        UserStatus? status,
        string? roleName,
        DateTime? fromDate,
        DateTime? toDate,
        string sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
    void SoftDelete(User user, string? deletedBy = null);
}
