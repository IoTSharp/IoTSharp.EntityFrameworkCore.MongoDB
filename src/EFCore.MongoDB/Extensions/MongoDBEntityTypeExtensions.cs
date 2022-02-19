// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.Metadata.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace IoTSharp.EntityFrameworkCore.MongoDB.Extensions;

/// <summary>
///     Entity type extension methods for MongoDB metadata.
/// </summary>
/// <remarks>
///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
/// </remarks>
public static class MongoDBEntityTypeExtensions
{
    /// <summary>
    ///     Returns the name of the container to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to get the container name for.</param>
    /// <returns>The name of the container to which the entity type is mapped.</returns>
    public static string? GetContainer(this IReadOnlyEntityType entityType)
        => entityType.BaseType != null
            ? entityType.GetRootType().GetContainer()
            : (string?)entityType[MongoDBAnnotationNames.ContainerName]
                ?? GetDefaultContainer(entityType);

    private static string? GetDefaultContainer(IReadOnlyEntityType entityType)
        => entityType.FindOwnership() != null
            ? null
            : entityType.ShortName() 
                ?? entityType.Model.GetDefaultContainer();

    /// <summary>
    ///     Sets the name of the container to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to set the container name for.</param>
    /// <param name="name">The name to set.</param>
    public static void SetContainer(this IMutableEntityType entityType, string? name)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.ContainerName,
            Check.NullButNotEmpty(name, nameof(name)));

    /// <summary>
    ///     Sets the name of the container to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to set the container name for.</param>
    /// <param name="name">The name to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetContainer(
        this IConventionEntityType entityType,
        string? name,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.ContainerName,
            Check.NullButNotEmpty(name, nameof(name)),
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the container to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to find configuration source for.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the container to which the entity type is mapped.</returns>
    public static ConfigurationSource? GetContainerConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(MongoDBAnnotationNames.ContainerName)
            ?.GetConfigurationSource();

    /// <summary>
    ///     Returns the name of the parent property to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to get the containing property name for.</param>
    /// <returns>The name of the parent property to which the entity type is mapped.</returns>
    public static string? GetContainingPropertyName(this IReadOnlyEntityType entityType)
        => entityType[MongoDBAnnotationNames.PropertyName] as string
            ?? GetDefaultContainingPropertyName(entityType);

    private static string? GetDefaultContainingPropertyName(IReadOnlyEntityType entityType)
        => entityType.FindOwnership() is IReadOnlyForeignKey ownership
            ? ownership.PrincipalToDependent!.Name
            : null;

    /// <summary>
    ///     Sets the name of the parent property to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to set the containing property name for.</param>
    /// <param name="name">The name to set.</param>
    public static void SetContainingPropertyName(this IMutableEntityType entityType, string? name)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.PropertyName,
            Check.NullButNotEmpty(name, nameof(name)));

    /// <summary>
    ///     Sets the name of the parent property to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to set the containing property name for.</param>
    /// <param name="name">The name to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetContainingPropertyName(
        this IConventionEntityType entityType,
        string? name,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.PropertyName,
            Check.NullButNotEmpty(name, nameof(name)),
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the parent property to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to find configuration source for.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the parent property to which the entity type is mapped.</returns>
    public static ConfigurationSource? GetContainingPropertyNameConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(MongoDBAnnotationNames.PropertyName)
            ?.GetConfigurationSource();

    /// <summary>
    ///     Returns the name of the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to get the partition key property name for.</param>
    /// <returns>The name of the partition key property.</returns>
    public static string? GetPartitionKeyPropertyName(this IReadOnlyEntityType entityType)
        => entityType[MongoDBAnnotationNames.PartitionKeyName] as string;

    /// <summary>
    ///     Sets the name of the property that is used to store the partition key key.
    /// </summary>
    /// <param name="entityType">The entity type to set the partition key property name for.</param>
    /// <param name="name">The name to set.</param>
    public static void SetPartitionKeyPropertyName(this IMutableEntityType entityType, string? name)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.PartitionKeyName,
            Check.NullButNotEmpty(name, nameof(name)));

    /// <summary>
    ///     Sets the name of the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to set the partition key property name for.</param>
    /// <param name="name">The name to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetPartitionKeyPropertyName(
        this IConventionEntityType entityType,
        string? name,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.PartitionKeyName,
            Check.NullButNotEmpty(name, nameof(name)),
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to find configuration source for.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the partition key property.</returns>
    public static ConfigurationSource? GetPartitionKeyPropertyNameConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(MongoDBAnnotationNames.PartitionKeyName)
            ?.GetConfigurationSource();

    /// <summary>
    ///     Returns the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to get the partition key property for.</param>
    /// <returns>The name of the partition key property.</returns>
    public static IReadOnlyProperty? GetPartitionKeyProperty(this IReadOnlyEntityType entityType)
    {
        var partitionKeyPropertyName = entityType.GetPartitionKeyPropertyName();
        return partitionKeyPropertyName == null
            ? null
            : entityType.FindProperty(partitionKeyPropertyName);
    }

    /// <summary>
    ///     Returns the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to get the partition key property for.</param>
    /// <returns>The name of the partition key property.</returns>
    public static IMutableProperty? GetPartitionKeyProperty(this IMutableEntityType entityType)
    {
        var partitionKeyPropertyName = entityType.GetPartitionKeyPropertyName();
        return partitionKeyPropertyName == null
            ? null
            : entityType.FindProperty(partitionKeyPropertyName);
    }

    /// <summary>
    ///     Returns the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to get the partition key property for.</param>
    /// <returns>The name of the partition key property.</returns>
    public static IConventionProperty? GetPartitionKeyProperty(this IConventionEntityType entityType)
    {
        var partitionKeyPropertyName = entityType.GetPartitionKeyPropertyName();
        return partitionKeyPropertyName == null
            ? null
            : entityType.FindProperty(partitionKeyPropertyName);
    }

    /// <summary>
    ///     Returns the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to get the partition key property for.</param>
    /// <returns>The name of the partition key property.</returns>
    public static IProperty? GetPartitionKeyProperty(this IEntityType entityType)
    {
        var partitionKeyPropertyName = entityType.GetPartitionKeyPropertyName();
        return partitionKeyPropertyName == null
            ? null
            : entityType.FindProperty(partitionKeyPropertyName);
    }

    /// <summary>
    ///     Returns the name of the property that is used to store the ETag.
    /// </summary>
    /// <param name="entityType">The entity type to get the etag property name for.</param>
    /// <returns>The name of the etag property.</returns>
    public static string? GetETagPropertyName(this IReadOnlyEntityType entityType)
        => entityType[MongoDBAnnotationNames.ETagName] as string;

 

    /// <summary>
    ///     Sets the name of the property that is used to store the ETag key.
    /// </summary>
    /// <param name="entityType">The entity type to set the etag property name for.</param>
    /// <param name="name">The name to set.</param>
    public static void SetETagPropertyName(this IMutableEntityType entityType, string? name)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.ETagName,
            Check.NullButNotEmpty(name, nameof(name)));

    /// <summary>
    ///     Sets the name of the property that is used to store the ETag.
    /// </summary>
    /// <param name="entityType">The entity type to set the ETag property name for.</param>
    /// <param name="name">The name to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetETagPropertyName(
        this IConventionEntityType entityType,
        string? name,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.ETagName,
            Check.NullButNotEmpty(name, nameof(name)),
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the property that is used to store the etag.
    /// </summary>
    /// <param name="entityType">The entity type to find configuration source for.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the etag property.</returns>
    public static ConfigurationSource? GetETagPropertyNameConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(MongoDBAnnotationNames.ETagName)
            ?.GetConfigurationSource();

    /// <summary>
    ///     Gets the property on this entity that is mapped to MongoDB ETag, if it exists.
    /// </summary>
    /// <param name="entityType">The entity type to get the ETag property for.</param>
    /// <returns>The property mapped to ETag, or <see langword="null" /> if no property is mapped to ETag.</returns>
    public static IReadOnlyProperty? GetETagProperty(this IReadOnlyEntityType entityType)
    {
        var etagPropertyName = entityType.GetETagPropertyName();

        return !string.IsNullOrEmpty(etagPropertyName) ? entityType.FindProperty(etagPropertyName) : null;
    }
   
    /// <summary>
    ///     Gets the property on this entity that is mapped to MongoDB ETag, if it exists.
    /// </summary>
    /// <param name="entityType">The entity type to get the ETag property for.</param>
    /// <returns>The property mapped to etag, or <see langword="null" /> if no property is mapped to ETag.</returns>
    public static IProperty? GetETagProperty(this IEntityType entityType)
        => (IProperty?)((IReadOnlyEntityType)entityType).GetETagProperty();

   
    /// <summary>
    ///     Returns the time to live for analytical store in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>The time to live.</returns>
    public static int? GetAnalyticalStoreTimeToLive(this IReadOnlyEntityType entityType)
        => entityType.BaseType != null
            ? entityType.GetRootType().GetAnalyticalStoreTimeToLive()
            : (int?)entityType[MongoDBAnnotationNames.AnalyticalStoreTimeToLive];

    /// <summary>
    ///     Sets the time to live for analytical store in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="seconds">The time to live to set.</param>
    public static void SetAnalyticalStoreTimeToLive(this IMutableEntityType entityType, int? seconds)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.AnalyticalStoreTimeToLive,
            seconds);

    /// <summary>
    ///     Sets the time to live for analytical store in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="seconds">The time to live to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetAnalyticalStoreTimeToLive(
        this IConventionEntityType entityType,
        int? seconds,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.AnalyticalStoreTimeToLive,
            seconds,
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the time to live for analytical store in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity typer.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the time to live for analytical store.</returns>
    public static ConfigurationSource? GetAnalyticalStoreTimeToLiveConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(MongoDBAnnotationNames.AnalyticalStoreTimeToLive)
            ?.GetConfigurationSource();

    /// <summary>
    ///     Returns the default time to live in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>The time to live.</returns>
    public static int? GetDefaultTimeToLive(this IReadOnlyEntityType entityType)
        => entityType.BaseType != null
            ? entityType.GetRootType().GetDefaultTimeToLive()
            : (int?)entityType[MongoDBAnnotationNames.DefaultTimeToLive];

    /// <summary>
    ///     Sets the default time to live in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="seconds">The time to live to set.</param>
    public static void SetDefaultTimeToLive(this IMutableEntityType entityType, int? seconds)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.DefaultTimeToLive,
            seconds);

    /// <summary>
    ///     Sets the default time to live in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="seconds">The time to live to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetDefaultTimeToLive(
        this IConventionEntityType entityType,
        int? seconds,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.DefaultTimeToLive,
            seconds,
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the default time to live in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type to find configuration source for.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the default time to live.</returns>
    public static ConfigurationSource? GetDefaultTimeToLiveConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(MongoDBAnnotationNames.DefaultTimeToLive)
            ?.GetConfigurationSource();

    /// <summary>
    ///     Returns the provisioned throughput at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>The throughput.</returns>
    public static ThroughputProperties? GetThroughput(this IReadOnlyEntityType entityType)
        => entityType.BaseType != null
            ? entityType.GetRootType().GetThroughput()
            : (ThroughputProperties?)entityType[MongoDBAnnotationNames.Throughput];

    /// <summary>
    ///     Sets the provisioned throughput at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="throughput">The throughput to set.</param>
    /// <param name="autoscale">Whether autoscale is enabled.</param>
    public static void SetThroughput(this IMutableEntityType entityType, int? throughput, bool? autoscale)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.Throughput,
            throughput == null || autoscale == null
                ? null
                : autoscale.Value
                    ? ThroughputProperties.CreateAutoscaleThroughput(throughput.Value)
                    : ThroughputProperties.CreateManualThroughput(throughput.Value));

    /// <summary>
    ///     Sets the provisioned throughput at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="throughput">The throughput to set.</param>
    /// <param name="autoscale">Whether autoscale is enabled.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetThroughput(
        this IConventionEntityType entityType,
        int? throughput,
        bool? autoscale,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.Throughput,
            throughput == null || autoscale == null
                ? null
                : autoscale.Value
                    ? ThroughputProperties.CreateAutoscaleThroughput(throughput.Value)
                    : ThroughputProperties.CreateManualThroughput(throughput.Value),
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the provisioned throughput at container scope.
    /// </summary>
    /// <param name="entityType">The entity type to find configuration source for.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the throughput.</returns>
    public static ConfigurationSource? GetThroughputConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(MongoDBAnnotationNames.Throughput)
            ?.GetConfigurationSource();
}
