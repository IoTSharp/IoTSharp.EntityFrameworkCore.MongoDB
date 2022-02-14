// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.Metadata.Internal;

// ReSharper disable once CheckNamespace
namespace IoTSharp.EntityFrameworkCore.MongoDB.Extensions;

/// <summary>
///     Property extension methods for MongoDB metadata.
/// </summary>
/// <remarks>
///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
/// </remarks>
public static class MongoDBPropertyExtensions
{
    /// <summary>
    ///     Returns the property name that the property is mapped to when targeting MongoDB.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>Returns the property name that the property is mapped to when targeting MongoDB.</returns>
    public static string GetJsonPropertyName(this IReadOnlyProperty property)
        => (string?)property[MongoDBAnnotationNames.PropertyName]
            ?? GetDefaultJsonPropertyName(property);

    private static string GetDefaultJsonPropertyName(IReadOnlyProperty property)
    {
        var entityType = property.DeclaringEntityType;
        var ownership = entityType.FindOwnership();

        if (ownership != null
            && !entityType.IsDocumentRoot())
        {
            var pk = property.FindContainingPrimaryKey();
            if (pk != null
                && (property.ClrType == typeof(int) || ownership.Properties.Contains(property))
                && pk.Properties.Count == ownership.Properties.Count + (ownership.IsUnique ? 0 : 1)
                && ownership.Properties.All(fkProperty => pk.Properties.Contains(fkProperty)))
            {
                return "";
            }
        }

        return property.Name;
    }

    /// <summary>
    ///     Sets the property name that the property is mapped to when targeting MongoDB.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="name">The name to set.</param>
    public static void SetJsonPropertyName(this IMutableProperty property, string? name)
        => property.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.PropertyName,
            name);

    /// <summary>
    ///     Sets the property name that the property is mapped to when targeting MongoDB.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="name">The name to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns>The configured value.</returns>
    public static string? SetJsonPropertyName(
        this IConventionProperty property,
        string? name,
        bool fromDataAnnotation = false)
    {
        property.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.PropertyName,
            name,
            fromDataAnnotation);

        return name;
    }

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> the property name that the property is mapped to when targeting MongoDB.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>
    ///     The <see cref="ConfigurationSource" /> the property name that the property is mapped to when targeting MongoDB.
    /// </returns>
    public static ConfigurationSource? GetJsonPropertyNameConfigurationSource(this IConventionProperty property)
        => property.FindAnnotation(MongoDBAnnotationNames.PropertyName)?.GetConfigurationSource();
}
