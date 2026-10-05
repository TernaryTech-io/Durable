namespace Durable.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Convenience lookups over an <see cref="IEntityMetadataProvider"/>.
    /// </summary>
    public static class EntityMetadataProviderExtensions
    {
        /// <summary>
        /// Gets the metadata for a property, resolved against the type the property was obtained from.
        /// </summary>
        /// <param name="provider">The metadata provider.</param>
        /// <param name="property">The property.</param>
        /// <returns>The property metadata, or null when the property is not part of the entity.</returns>
        public static PropertyMetadata? GetPropertyMetadata(this IEntityMetadataProvider provider, PropertyInfo? property)
        {
            if (property == null) return null;
            Type? entityType = property.ReflectedType ?? property.DeclaringType;
            if (entityType == null) return null;
            return provider.GetEntityMetadata(entityType).GetProperty(property);
        }

        /// <summary>Gets the entity (table) mapping for a type.</summary>
        /// <param name="provider">The metadata provider.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The entity mapping, or null.</returns>
        public static EntityAttribute? GetEntity(this IEntityMetadataProvider provider, Type entityType) => provider.GetEntityMetadata(entityType).Entity;

        /// <summary>Gets the column mapping for a property.</summary>
        /// <param name="provider">The metadata provider.</param>
        /// <param name="property">The property.</param>
        /// <returns>The column mapping, or null.</returns>
        public static PropertyAttribute? GetColumn(this IEntityMetadataProvider provider, PropertyInfo? property) => provider.GetPropertyMetadata(property)?.Column;

        /// <summary>Gets the foreign key mapping for a property.</summary>
        /// <param name="provider">The metadata provider.</param>
        /// <param name="property">The property.</param>
        /// <returns>The foreign key mapping, or null.</returns>
        public static ForeignKeyAttribute? GetForeignKey(this IEntityMetadataProvider provider, PropertyInfo? property) => provider.GetPropertyMetadata(property)?.ForeignKey;

        /// <summary>Gets the navigation property mapping for a property.</summary>
        /// <param name="provider">The metadata provider.</param>
        /// <param name="property">The property.</param>
        /// <returns>The navigation mapping, or null.</returns>
        public static NavigationPropertyAttribute? GetNavigation(this IEntityMetadataProvider provider, PropertyInfo? property) => provider.GetPropertyMetadata(property)?.Navigation;

        /// <summary>Gets the inverse navigation property mapping for a property.</summary>
        /// <param name="provider">The metadata provider.</param>
        /// <param name="property">The property.</param>
        /// <returns>The inverse navigation mapping, or null.</returns>
        public static InverseNavigationPropertyAttribute? GetInverseNavigation(this IEntityMetadataProvider provider, PropertyInfo? property) => provider.GetPropertyMetadata(property)?.InverseNavigation;

        /// <summary>Gets the many-to-many navigation property mapping for a property.</summary>
        /// <param name="provider">The metadata provider.</param>
        /// <param name="property">The property.</param>
        /// <returns>The many-to-many mapping, or null.</returns>
        public static ManyToManyNavigationPropertyAttribute? GetManyToMany(this IEntityMetadataProvider provider, PropertyInfo? property) => provider.GetPropertyMetadata(property)?.ManyToMany;

        /// <summary>Gets the version column mapping for a property.</summary>
        /// <param name="provider">The metadata provider.</param>
        /// <param name="property">The property.</param>
        /// <returns>The version column mapping, or null.</returns>
        public static VersionColumnAttribute? GetVersionColumn(this IEntityMetadataProvider provider, PropertyInfo? property) => provider.GetPropertyMetadata(property)?.VersionColumn;

        /// <summary>Gets the default value mapping for a property.</summary>
        /// <param name="provider">The metadata provider.</param>
        /// <param name="property">The property.</param>
        /// <returns>The default value mapping, or null.</returns>
        public static DefaultValueAttribute? GetDefaultValue(this IEntityMetadataProvider provider, PropertyInfo? property) => provider.GetPropertyMetadata(property)?.DefaultValue;

        /// <summary>Gets the single-column indexes for a property.</summary>
        /// <param name="provider">The metadata provider.</param>
        /// <param name="property">The property.</param>
        /// <returns>The indexes; empty when none.</returns>
        public static IReadOnlyList<IndexAttribute> GetIndexes(this IEntityMetadataProvider provider, PropertyInfo? property) => provider.GetPropertyMetadata(property)?.Indexes ?? Array.Empty<IndexAttribute>();
    }
}
