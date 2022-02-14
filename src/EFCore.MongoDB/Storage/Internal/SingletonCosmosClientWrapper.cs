// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.Infrastructure.Internal;

namespace IoTSharp.EntityFrameworkCore.MongoDB.Storage.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class SingletonMongoDBClientWrapper : ISingletonMongoDBClientWrapper
{
    private static readonly string UserAgent = " IoTSharp.EntityFrameworkCore.MongoDB/" + ProductInfo.GetVersion();
    private readonly MongoClientSettings _options;
    private readonly string? _endpoint;
    private readonly string? _key;
    private readonly string? _connectionString;
    private MongoClient? _client;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public SingletonMongoDBClientWrapper(MongoClientSettings options)
    {
        _options = options;
    }
    public SingletonMongoDBClientWrapper(string connectionString) => _connectionString = connectionString;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual  MongoClient Client
        => _client ??= string.IsNullOrEmpty(_connectionString)
            ? new MongoClient( _options)
            : new MongoClient(_connectionString);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual void Dispose()
    {
        _client?.Cluster?.Dispose();
        _client = null;
    }
}
