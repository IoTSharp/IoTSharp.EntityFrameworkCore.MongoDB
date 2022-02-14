// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.Metadata.Internal;

#nullable enable

namespace IoTSharp.EntityFrameworkCore.MongoDB.Metadata.Conventions;

/// <summary>
///     A convention that creates an optimized copy of the mutable model. This convention is typically
///     implemented by database providers to update provider annotations when creating a read-only model.
/// </summary>
/// <remarks>
///     See <see href="https://aka.ms/efcore-docs-conventions">Model building conventions</see>, and
///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
/// </remarks>
public class MongoDBRuntimeModelConvention : RuntimeModelConvention
{
    /// <summary>
    ///     Creates a new instance of <see cref="MongoDBRuntimeModelConvention" />.
    /// </summary>
    /// <param name="dependencies">Parameter object containing dependencies for this convention.</param>
    public MongoDBRuntimeModelConvention(
        ProviderConventionSetBuilderDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <summary>
    ///     Updates the model annotations that will be set on the read-only object.
    /// </summary>
    /// <param name="annotations">The annotations to be processed.</param>
    /// <param name="model">The source model.</param>
    /// <param name="runtimeModel">The target model that will contain the annotations.</param>
    /// <param name="runtime">Indicates whether the given annotations are runtime annotations.</param>
    protected override void ProcessModelAnnotations(
        Dictionary<string, object?> annotations,
        IModel model,
        RuntimeModel runtimeModel,
        bool runtime)
    {
        base.ProcessModelAnnotations(annotations, model, runtimeModel, runtime);

        if (!runtime)
        {
            annotations.Remove(MongoDBAnnotationNames.Throughput);
        }
    }

    /// <summary>
    ///     Updates the entity type annotations that will be set on the read-only object.
    /// </summary>
    /// <param name="annotations">The annotations to be processed.</param>
    /// <param name="entityType">The source entity type.</param>
    /// <param name="runtimeEntityType">The target entity type that will contain the annotations.</param>
    /// <param name="runtime">Indicates whether the given annotations are runtime annotations.</param>
    protected override void ProcessEntityTypeAnnotations(
        IDictionary<string, object?> annotations,
        IEntityType entityType,
        RuntimeEntityType runtimeEntityType,
        bool runtime)
    {
        base.ProcessEntityTypeAnnotations(annotations, entityType, runtimeEntityType, runtime);

        if (!runtime)
        {
            annotations.Remove(MongoDBAnnotationNames.AnalyticalStoreTimeToLive);
            annotations.Remove(MongoDBAnnotationNames.DefaultTimeToLive);
            annotations.Remove(MongoDBAnnotationNames.Throughput);
        }
    }
}
