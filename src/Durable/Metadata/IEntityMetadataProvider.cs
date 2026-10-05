namespace Durable.Metadata
{
    using System;

    /// <summary>
    /// Resolves the mapping metadata (table, columns, keys, relationships, indexes) for an entity type.
    /// Repositories consult this provider instead of reading mapping attributes directly, which allows
    /// mapping to be supplied by conventions or by another attribute system.
    /// </summary>
    public interface IEntityMetadataProvider
    {
        /// <summary>
        /// Gets the mapping metadata for the specified entity type.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The resolved metadata. Never null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when entityType is null.</exception>
        EntityMetadata GetEntityMetadata(Type entityType);
    }
}
