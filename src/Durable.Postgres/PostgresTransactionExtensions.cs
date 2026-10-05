namespace Durable.Postgres
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Durable;
    using Npgsql;

    /// <summary>
    /// Begins PostgreSQL transactions directly from a connection factory. Because every
    /// repository accepts an <see cref="ITransaction"/>, a transaction started here can be shared
    /// by repositories of different entity types that use the same factory (a unit of work).
    /// </summary>
    public static class PostgresTransactionExtensions
    {
        /// <summary>
        /// Begins a new database transaction on a connection from the factory.
        /// The transaction owns the connection and returns it to the factory when disposed.
        /// </summary>
        /// <param name="connectionFactory">The PostgreSQL connection factory.</param>
        /// <returns>A new transaction instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when connectionFactory is null.</exception>
        public static ITransaction BeginTransaction(this IConnectionFactory connectionFactory)
        {
            ArgumentNullException.ThrowIfNull(connectionFactory);

            NpgsqlConnection? connection = null;
            try
            {
                connection = (NpgsqlConnection)PooledConnectionHandle.Unwrap(connectionFactory.GetConnection());

                // Ensure connection is open (GetConnection might return an already open connection)
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                NpgsqlTransaction transaction = connection.BeginTransaction();
                PostgresRepositoryTransaction result = new PostgresRepositoryTransaction(connection, transaction, connectionFactory);
                connection = null; // Transaction now owns the connection
                return result;
            }
            finally
            {
                if (connection != null)
                {
                    connectionFactory.ReturnConnection(connection);
                }
            }
        }

        /// <summary>
        /// Asynchronously begins a new database transaction on a connection from the factory.
        /// The transaction owns the connection and returns it to the factory when disposed.
        /// </summary>
        /// <param name="connectionFactory">The PostgreSQL connection factory.</param>
        /// <param name="token">A cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation with a new transaction instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when connectionFactory is null.</exception>
        public static async Task<ITransaction> BeginTransactionAsync(this IConnectionFactory connectionFactory, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(connectionFactory);
            token.ThrowIfCancellationRequested();

            NpgsqlConnection? connection = null;
            try
            {
                connection = (NpgsqlConnection)PooledConnectionHandle.Unwrap(await connectionFactory.GetConnectionAsync(token).ConfigureAwait(false));

                // Ensure connection is open (GetConnectionAsync might return an already open connection)
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(token).ConfigureAwait(false);
                }

                NpgsqlTransaction transaction = await connection.BeginTransactionAsync(token).ConfigureAwait(false);
                PostgresRepositoryTransaction result = new PostgresRepositoryTransaction(connection, transaction, connectionFactory);
                connection = null; // Transaction now owns the connection
                return result;
            }
            finally
            {
                if (connection != null)
                {
                    await connectionFactory.ReturnConnectionAsync(connection).ConfigureAwait(false);
                }
            }
        }
    }
}
