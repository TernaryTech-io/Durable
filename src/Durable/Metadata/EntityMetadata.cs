namespace Durable.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Resolved mapping metadata for an entity type.
    /// </summary>
    public sealed class EntityMetadata
    {
        #region Public-Members

        /// <summary>
        /// Gets the entity type this metadata describes.
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// Gets the entity (table) mapping, or null when the type is not explicitly mapped to a table.
        /// </summary>
        public EntityAttribute? Entity { get; }

        /// <summary>
        /// Gets the database schema the table belongs to, or null to use the connection's default schema.
        /// </summary>
        public string? Schema { get; }

        /// <summary>
        /// Gets the metadata for each public instance property of the entity, in reflection order.
        /// </summary>
        public IReadOnlyList<PropertyMetadata> Properties { get; }

        /// <summary>
        /// Gets the composite indexes declared for the entity.
        /// </summary>
        public IReadOnlyList<CompositeIndexAttribute> CompositeIndexes { get; }

        #endregion

        #region Private-Members

        private readonly Dictionary<(Module, int), PropertyMetadata> _PropertiesByToken;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityMetadata"/> class.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="entity">The entity (table) mapping, or null when not explicitly mapped.</param>
        /// <param name="schema">The database schema, or null for the default schema.</param>
        /// <param name="properties">The property metadata.</param>
        /// <param name="compositeIndexes">The composite indexes, or null for none.</param>
        /// <exception cref="ArgumentNullException">Thrown when entityType or properties is null.</exception>
        public EntityMetadata(
            Type entityType,
            EntityAttribute? entity,
            string? schema,
            IEnumerable<PropertyMetadata> properties,
            IEnumerable<CompositeIndexAttribute>? compositeIndexes = null)
        {
            ArgumentNullException.ThrowIfNull(entityType);
            ArgumentNullException.ThrowIfNull(properties);

            EntityType = entityType;
            Entity = entity;
            Schema = string.IsNullOrWhiteSpace(schema) ? null : schema;
            Properties = properties.ToList().AsReadOnly();
            CompositeIndexes = (compositeIndexes ?? Array.Empty<CompositeIndexAttribute>()).ToList().AsReadOnly();

            _PropertiesByToken = new Dictionary<(Module, int), PropertyMetadata>();
            foreach (PropertyMetadata property in Properties)
            {
                _PropertiesByToken[(property.Property.Module, property.Property.MetadataToken)] = property;
            }
        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Gets the metadata for the specified property. Properties are matched by their metadata definition,
        /// so a <see cref="PropertyInfo"/> obtained from a base or derived type resolves to the same entry, while
        /// properties hidden with <c>new</c> remain distinct.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The property metadata, or null when the property is not part of this entity.</returns>
        public PropertyMetadata? GetProperty(PropertyInfo property)
        {
            if (property == null) return null;
            _PropertiesByToken.TryGetValue((property.Module, property.MetadataToken), out PropertyMetadata? result);
            return result;
        }

        /// <summary>
        /// Gets the metadata for the property with the specified CLR name.
        /// When several properties share the name (e.g. hidden with <c>new</c>), the most derived one is returned.
        /// </summary>
        /// <param name="propertyName">The CLR property name.</param>
        /// <returns>The property metadata, or null when not found.</returns>
        public PropertyMetadata? GetProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;
            return Properties.FirstOrDefault(p => p.Property.Name == propertyName);
        }

        #endregion
    }
}
