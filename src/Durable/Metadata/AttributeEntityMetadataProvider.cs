namespace Durable.Metadata
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Default metadata provider that reads Durable's mapping attributes from the entity type.
    /// Results are cached per type. Derive from this class and override the <c>Resolve*</c> methods
    /// to supply mapping from conventions or another attribute system while keeping the remaining defaults.
    /// </summary>
    public class AttributeEntityMetadataProvider : IEntityMetadataProvider
    {
        #region Private-Members

        private readonly ConcurrentDictionary<Type, EntityMetadata> _Cache = new ConcurrentDictionary<Type, EntityMetadata>();

        #endregion

        #region Public-Methods

        /// <inheritdoc />
        public EntityMetadata GetEntityMetadata(Type entityType)
        {
            ArgumentNullException.ThrowIfNull(entityType);
            return _Cache.GetOrAdd(entityType, BuildEntityMetadata);
        }

        #endregion

        #region Protected-Methods

        /// <summary>
        /// Builds the metadata for an entity type. Called once per type; the result is cached.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The entity metadata.</returns>
        protected virtual EntityMetadata BuildEntityMetadata(Type entityType)
        {
            List<PropertyMetadata> properties = new List<PropertyMetadata>();
            foreach (PropertyInfo property in GetMappableProperties(entityType))
            {
                properties.Add(BuildPropertyMetadata(entityType, property));
            }

            return new EntityMetadata(
                entityType,
                ResolveEntity(entityType),
                ResolveSchema(entityType),
                properties,
                ResolveCompositeIndexes(entityType));
        }

        /// <summary>
        /// Gets the properties considered for mapping. Defaults to all public instance properties.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The candidate properties.</returns>
        protected virtual IEnumerable<PropertyInfo> GetMappableProperties(Type entityType)
        {
            return entityType.GetProperties();
        }

        /// <summary>
        /// Builds the metadata for a single property.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="property">The property.</param>
        /// <returns>The property metadata.</returns>
        protected virtual PropertyMetadata BuildPropertyMetadata(Type entityType, PropertyInfo property)
        {
            return new PropertyMetadata(
                property,
                ResolveColumn(entityType, property),
                ResolveForeignKey(entityType, property),
                ResolveNavigation(entityType, property),
                ResolveInverseNavigation(entityType, property),
                ResolveManyToMany(entityType, property),
                ResolveVersionColumn(entityType, property),
                ResolveDefaultValue(entityType, property),
                ResolveIndexes(entityType, property));
        }

        /// <summary>Resolves the entity (table) mapping.</summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The entity mapping, or null.</returns>
        protected virtual EntityAttribute? ResolveEntity(Type entityType) => entityType.GetCustomAttribute<EntityAttribute>();

        /// <summary>Resolves the database schema. Defaults to null (the connection's default schema).</summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The schema name, or null.</returns>
        protected virtual string? ResolveSchema(Type entityType) => null;

        /// <summary>Resolves the composite indexes.</summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The composite indexes.</returns>
        protected virtual IEnumerable<CompositeIndexAttribute> ResolveCompositeIndexes(Type entityType) => entityType.GetCustomAttributes<CompositeIndexAttribute>();

        /// <summary>Resolves the column mapping.</summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="property">The property.</param>
        /// <returns>The column mapping, or null when not mapped.</returns>
        protected virtual PropertyAttribute? ResolveColumn(Type entityType, PropertyInfo property) => property.GetCustomAttribute<PropertyAttribute>();

        /// <summary>Resolves the foreign key mapping.</summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="property">The property.</param>
        /// <returns>The foreign key mapping, or null.</returns>
        protected virtual ForeignKeyAttribute? ResolveForeignKey(Type entityType, PropertyInfo property) => property.GetCustomAttribute<ForeignKeyAttribute>();

        /// <summary>Resolves the navigation property mapping.</summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="property">The property.</param>
        /// <returns>The navigation mapping, or null.</returns>
        protected virtual NavigationPropertyAttribute? ResolveNavigation(Type entityType, PropertyInfo property) => property.GetCustomAttribute<NavigationPropertyAttribute>();

        /// <summary>Resolves the inverse navigation property mapping.</summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="property">The property.</param>
        /// <returns>The inverse navigation mapping, or null.</returns>
        protected virtual InverseNavigationPropertyAttribute? ResolveInverseNavigation(Type entityType, PropertyInfo property) => property.GetCustomAttribute<InverseNavigationPropertyAttribute>();

        /// <summary>Resolves the many-to-many navigation property mapping.</summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="property">The property.</param>
        /// <returns>The many-to-many mapping, or null.</returns>
        protected virtual ManyToManyNavigationPropertyAttribute? ResolveManyToMany(Type entityType, PropertyInfo property) => property.GetCustomAttribute<ManyToManyNavigationPropertyAttribute>();

        /// <summary>Resolves the version column mapping.</summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="property">The property.</param>
        /// <returns>The version column mapping, or null.</returns>
        protected virtual VersionColumnAttribute? ResolveVersionColumn(Type entityType, PropertyInfo property) => property.GetCustomAttribute<VersionColumnAttribute>();

        /// <summary>Resolves the default value mapping.</summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="property">The property.</param>
        /// <returns>The default value mapping, or null.</returns>
        protected virtual DefaultValueAttribute? ResolveDefaultValue(Type entityType, PropertyInfo property) => property.GetCustomAttribute<DefaultValueAttribute>();

        /// <summary>Resolves the single-column indexes.</summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="property">The property.</param>
        /// <returns>The indexes.</returns>
        protected virtual IEnumerable<IndexAttribute> ResolveIndexes(Type entityType, PropertyInfo property) => property.GetCustomAttributes<IndexAttribute>();

        #endregion
    }
}
