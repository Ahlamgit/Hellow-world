using Khadamati.Application.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Infrastructure.Common;

internal static class DbPersistenceExceptionMapper
{
    private const int SqlUniqueConstraintViolation = 2627;
    private const int SqlUniqueIndexViolation = 2601;

    public static async Task<int> SaveChangesAsync(
        Func<Task<int>> saveChanges,
        string concurrencyMessage,
        string? uniqueConstraintMessage = null)
    {
        try
        {
            return await saveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(concurrencyMessage);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new ConflictException(uniqueConstraintMessage ?? "A conflicting record already exists.");
        }
    }

    public static bool IsUniqueConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException sql &&
        (sql.Number == SqlUniqueConstraintViolation || sql.Number == SqlUniqueIndexViolation);

    public static bool IsSlotReservationConflict(DbUpdateException exception) =>
        exception.Entries.Any(e => e.Entity is Domain.Entities.BookingSlotReservation) ||
        (exception.InnerException?.Message.Contains("BookingSlotReservations", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (exception.InnerException?.Message.Contains("IX_BookingSlotReservations_ActiveSlot", StringComparison.OrdinalIgnoreCase) ?? false);
}
