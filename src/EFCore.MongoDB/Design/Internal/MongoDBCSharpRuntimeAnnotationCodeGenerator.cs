// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Design.Internal;

namespace IoTSharp.EntityFrameworkCore.MongoDB.Design.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
#pragma warning disable EF1001 // Internal EF Core API usage.
public class MongoDBCSharpRuntimeAnnotationCodeGenerator : CSharpRuntimeAnnotationCodeGenerator
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public MongoDBCSharpRuntimeAnnotationCodeGenerator(
        CSharpRuntimeAnnotationCodeGeneratorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <inheritdoc />
    public override void Generate(IModel model, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        var annotations = parameters.Annotations;
        if (!parameters.IsRuntime)
        {
            annotations.Remove(MongoDBAnnotationNames.Throughput);
        }

        base.Generate(model, parameters);
    }

    /// <inheritdoc />
    public override void Generate(IEntityType entityType, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        var annotations = parameters.Annotations;
        if (!parameters.IsRuntime)
        {
            annotations.Remove(MongoDBAnnotationNames.AnalyticalStoreTimeToLive);
            annotations.Remove(MongoDBAnnotationNames.DefaultTimeToLive);
            annotations.Remove(MongoDBAnnotationNames.Throughput);
        }

        base.Generate(entityType, parameters);
    }
}
