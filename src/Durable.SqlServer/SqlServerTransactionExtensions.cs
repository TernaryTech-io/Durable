namespace Durable.SqlServer
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Durable;
    using Microsoft.Data.SqlClient;

    /// <summary>
    /// Begins SQL Server transactions directly from a connection factory. Because every repository
    /// accepts an <see cref="ITransaction"/>, a transaction started here can be shared by
    /// repositories of different entity types that use the same factory (a unit of work).
    /// </summary>
    public static class SqlServerTransactionExtensions
    {
        /// <summary>
        /// Begins a new database transaction on a connection from the factory.
        /// The transaction owns the connection and returns it to the factory when disposed.
        /// </summary>
        /// <param name="connectionFactory">The SQL Server connection factory.</param>
        /// <returns>A new transaction instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when connectionFactory is null.</exception>
        public static ITransaction BeginTransaction(this IConnectionFactory connectionFactory)
        {
            ArgumentNullException.ThrowIfNull(connectionFactory);

            SqlConnection? connection = null;
            try
            {
                connection = Unwrap(connectionFactory.GetConnection());
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                SqlTransaction transaction = connection.BeginTransaction();
                SqlServerRepositoryTransaction result = new SqlServerRepositoryTransaction(connection, transaction, connectionFactory);
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
        /// <param name="connectionFactory">The SQL Server connection factory.</param>
        /// <param name="token">A cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation with a new transaction instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when connectionFactory is null.</exception>
        public static async Task<ITransaction> BeginTransactionAsync(this IConnectionFactory connectionFactory, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(connectionFactory);
            token.ThrowIfCancellationRequested();

            SqlConnection? connection = null;
            try
            {
                connection = Unwrap(await connectionFactory.GetConnectionAsync(token).ConfigureAwait(false));
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(token).ConfigureAwait(false);
                }

                SqlTransaction transaction = (SqlTransaction)await connection.BeginTransactionAsync(token).ConfigureAwait(false);
                SqlServerRepositoryTransaction result = new SqlServerRepositoryTransaction(connection, transaction, connectionFactory);
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

        private static SqlConnection Unwrap(DbConnection connection)
        {
            return (SqlConnection)PooledConnectionHandle.Unwrap(connection);
        }
    }
}
