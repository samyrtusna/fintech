using Microsoft.EntityFrameworkCore;
using Npgsql;
using fintech.Application.Exceptions;

namespace fintech.Infrastructure.Persistence.Exceptions
{
    public static class DatabaseExceptionMapper
    {
        public static Exception Map(Exception ex)
        {
            var pgEx = GetPostgresException(ex);

            if (pgEx != null && pgEx.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return MapUniqueConstraint(pgEx);
            }

            return ex;
        }

        private static Exception MapUniqueConstraint(PostgresException pgEx)
        {
            return pgEx.ConstraintName switch
            {
                "IX_Users_Email" => new DuplicateValueException("Email already exists."),
                "IX_Users_Username" => new DuplicateValueException("Username is already taken."),

               
                _ => new DuplicateValueException("A unique constraint violation occurred.")
            };
        }

        private static PostgresException? GetPostgresException(Exception ex)
        {
            while (ex != null)
            {
                if (ex is PostgresException pgEx)
                    return pgEx;

                ex = ex.InnerException!;
            }
            return null;
        }
    }
}