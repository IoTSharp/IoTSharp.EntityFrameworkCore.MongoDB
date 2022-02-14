// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.Metadata.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace IoTSharp.EntityFrameworkCore.MongoDB.Extensions;

/// <summary>
///     Model extension methods for MongoDB metadata.
/// </summary>
/// <remarks>
///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
/// </remarks>
public static class MongoDBModelExtensions
{
    /// <summary>
    ///     Returns the default container name.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The default container name.</returns>
    public static string? GetDefaultContainer(this IReadOnlyModel model)
        => (string?)model[MongoDBAnnotationNames.ContainerName];

    /// <summary>
    ///     Sets the default container name.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="name">The name to set.</param>
    public static void SetDefaultContainer(this IMutableModel model, string? name)
        => model.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.ContainerName,
            Check.NullButNotEmpty(name, nameof(name)));

    /// <summary>
    ///     Sets the default container name.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="name">The name to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns>The configured value.</returns>
    public static string? SetDefaultContainer(
        this IConventionModel model,
        string? name,
        bool fromDataAnnotation = false)
    {
        model.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.ContainerName,
            Check.NullButNotEmpty(name, nameof(name)),
            fromDataAnnotation);

        return name;
    }

    /// <summary>
    ///     Returns the configuration source for the default container name.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The configuration source for the default container name.</returns>
    public static ConfigurationSource? GetDefaultContainerConfigurationSource(this IConventionModel model)
        => model.FindAnnotation(MongoDBAnnotationNames.ContainerName)?.GetConfigurationSource();

    /// <summary>
    ///     Returns the provisioned throughput at database scope.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The throughput.</returns>
    public static ThroughputProperties? GetThroughput(this IReadOnlyModel model)
        => (ThroughputProperties?)model[MongoDBAnnotationNames.Throughput];

    /// <summary>
    ///     Sets the provisioned throughput at database scope.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="throughput">The throughput to set.</param>
    /// <param name="autoscale">Whether autoscale is enabled.</param>
    public static void SetThroughput(this IMutableModel model, int? throughput, bool? autoscale)
        => model.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.Throughput,
            throughput == null || autoscale == null
                ? null
                : autoscale.Value
                    ? ThroughputProperties.CreateAutoscaleThroughput(throughput.Value)
                    : ThroughputProperties.CreateManualThroughput(throughput.Value));

    /// <summary>
    ///     Sets the provisioned throughput at database scope.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="throughput">The throughput to set.</param>
    /// <param name="autoscale">Whether autoscale is enabled.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static int? SetThroughput(
        this IConventionModel model,
        int? throughput,
        bool? autoscale,
        bool fromDataAnnotation = false)
    {
        var valueSet = (ThroughputProperties?)model.SetOrRemoveAnnotation(
            MongoDBAnnotationNames.Throughput,
            throughput == null || autoscale == null
                ? null
                : autoscale.Value
                    ? ThroughputProperties.CreateAutoscaleThroughput(throughput.Value)
                    : ThroughputProperties.CreateManualThroughput(throughput.Value),
            fromDataAnnotation)?.Value;
        return valueSet?.AutoscaleMaxThroughput ?? valueSet?.Throughput;
    }

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the provisioned throughput at database scope.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the throughput.</returns>
    public static ConfigurationSource? GetThroughputConfigurationSource(this IConventionModel model)
        => model.FindAnnotation(MongoDBAnnotationNames.Throughput)
            ?.GetConfigurationSource();
}
