namespace Durable.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Resolved mapping metadata for a single entity property. Each member uses the corresponding Durable
    /// attribute type as its vocabulary; a metadata provider may construct these instances in code rather
    /// than read them from the property.
    /// </summary>
    public sealed class PropertyMetadata
    {
        #region Public-Members

        /// <summary>
        /// Gets the property this metadata describes.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets the column mapping, or null when the property is not mapped to a column.
        /// </summary>
        public PropertyAttribute? Column { get; }

        /// <summary>
        /// Gets the foreign key mapping, or null.
        /// </summary>
        public ForeignKeyAttribute? ForeignKey { get; }

        /// <summary>
        /// Gets the navigation property mapping, or null.
        /// </summary>
        public NavigationPropertyAttribute? Navigation { get; }

        /// <summary>
        /// Gets the inverse navigation property mapping, or null.
        /// </summary>
        public InverseNavigationPropertyAttribute? InverseNavigation { get; }

        /// <summary>
        /// Gets the many-to-many navigation property mapping, or null.
        /// </summary>
        public ManyToManyNavigationPropertyAttribute? ManyToMany { get; }

        /// <summary>
        /// Gets the version (concurrency) column mapping, or null.
        /// </summary>
        public VersionColumnAttribute? VersionColumn { get; }

        /// <summary>
        /// Gets the default value mapping, or null.
        /// </summary>
        public DefaultValueAttribute? DefaultValue { get; }

        /// <summary>
        /// Gets the single-column indexes declared on the property.
        /// </summary>
        public IReadOnlyList<IndexAttribute> Indexes { get; }

        /// <summary>
        /// Gets a value indicating whether the property is any kind of navigation property.
        /// </summary>
        public bool IsNavigation => Navigation != null || InverseNavigation != null || ManyToMany != null;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMetadata"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="column">The column mapping, or null.</param>
        /// <param name="foreignKey">The foreign key mapping, or null.</param>
        /// <param name="navigation">The navigation property mapping, or null.</param>
        /// <param name="inverseNavigation">The inverse navigation property mapping, or null.</param>
        /// <param name="manyToMany">The many-to-many navigation property mapping, or null.</param>
        /// <param name="versionColumn">The version column mapping, or null.</param>
        /// <param name="defaultValue">The default value mapping, or null.</param>
        /// <param name="indexes">The single-column indexes, or null for none.</param>
        /// <exception cref="ArgumentNullException">Thrown when property is null.</exception>
        public PropertyMetadata(
            PropertyInfo property,
            PropertyAttribute? column = null,
            ForeignKeyAttribute? foreignKey = null,
            NavigationPropertyAttribute? navigation = null,
            InverseNavigationPropertyAttribute? inverseNavigation = null,
            ManyToManyNavigationPropertyAttribute? manyToMany = null,
            VersionColumnAttribute? versionColumn = null,
            DefaultValueAttribute? defaultValue = null,
            IEnumerable<IndexAttribute>? indexes = null)
        {
            ArgumentNullException.ThrowIfNull(property);

            Property = property;
            Column = column;
            ForeignKey = foreignKey;
            Navigation = navigation;
            InverseNavigation = inverseNavigation;
            ManyToMany = manyToMany;
            VersionColumn = versionColumn;
            DefaultValue = defaultValue;
            Indexes = (indexes ?? Array.Empty<IndexAttribute>()).ToList().AsReadOnly();
        }

        #endregion
    }
}
