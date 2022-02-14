// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.Storage.Internal;

namespace IoTSharp.EntityFrameworkCore.MongoDB.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class MongoDBQueryContextFactory : IQueryContextFactory
{
    private readonly IMongoDBClientWrapper _MongoDBClient;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public MongoDBQueryContextFactory(
        QueryContextDependencies dependencies,
        IMongoDBClientWrapper MongoDBClient)
    {
        Dependencies = dependencies;
        _MongoDBClient = MongoDBClient;
    }

    /// <summary>
    ///     Dependencies for this service.
    /// </summary>
    protected virtual QueryContextDependencies Dependencies { get; }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual QueryContext Create()
        => new MongoDBQueryContext(Dependencies, _MongoDBClient);
}
