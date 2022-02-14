// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace IoTSharp.EntityFrameworkCore.MongoDB.Infrastructure;

/// <summary>
///     Explicitly implemented by <see cref="MongoDBDbContextOptionsBuilder" /> to hide
///     methods that are used by database provider extension methods but not intended to be called by application
///     developers.
/// </summary>
public interface IMongoDBDbContextOptionsBuilderInfrastructure
{
    /// <summary>
    ///     Gets the core options builder.
    /// </summary>
    DbContextOptionsBuilder OptionsBuilder { get; }
}
