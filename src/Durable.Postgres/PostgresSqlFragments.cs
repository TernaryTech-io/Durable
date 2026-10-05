namespace Durable.Postgres
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Helpers for composing SQL fragments.
    /// </summary>
    internal static class PostgresSqlFragments
    {
        /// <summary>
        /// Combines conditions with AND. When there is more than one condition each is wrapped in
        /// parentheses, so a condition containing OR cannot bind across its neighbours.
        /// </summary>
        /// <param name="conditions">The conditions to combine.</param>
        /// <returns>The combined condition.</returns>
        internal static string JoinConditions(IEnumerable<string> conditions)
        {
            List<string> list = conditions.ToList();
            if (list.Count == 1)
                return list[0];

            return string.Join(" AND ", list.Select(c => $"({c})"));
        }
    }
}
