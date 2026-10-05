namespace Durable.Postgres
{
    using System;
    using Durable;

    /// <summary>
    /// Helpers for building PostgreSQL identifiers.
    /// </summary>
    internal static class PostgresIdentifierExtensions
    {
        /// <summary>
        /// Sanitizes a table name, qualifying it with its schema when one is specified.
        /// </summary>
        /// <param name="sanitizer">The identifier sanitizer.</param>
        /// <param name="tableName">The table name.</param>
        /// <param name="schema">The schema name, or null to leave the table unqualified.</param>
        /// <returns>The sanitized, optionally schema-qualified, table name.</returns>
        /// <exception cref="ArgumentNullException">Thrown when sanitizer or tableName is null.</exception>
        internal static string SanitizeTableName(this ISanitizer sanitizer, string tableName, string? schema)
        {
            ArgumentNullException.ThrowIfNull(sanitizer);
            ArgumentNullException.ThrowIfNull(tableName);

            if (string.IsNullOrWhiteSpace(schema))
                return sanitizer.SanitizeIdentifier(tableName);

            return sanitizer.SanitizeIdentifier(schema) + "." + sanitizer.SanitizeIdentifier(tableName);
        }
    }
}
