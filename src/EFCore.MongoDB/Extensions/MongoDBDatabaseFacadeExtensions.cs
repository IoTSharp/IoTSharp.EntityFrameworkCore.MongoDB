// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.Infrastructure.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace IoTSharp.EntityFrameworkCore.MongoDB.Extensions;

/// <summary>
///     Extension methods for the <see cref="DatabaseFacade" /> returned from <see cref="DbContext.Database" />
///     that can be used only with the MongoDB provider.
/// </summary>
public static class MongoDBDatabaseFacadeExtensions
{
    /// <summary>
    ///     Gets the underlying <see cref="CosmosClient" /> for this <see cref="DbContext" />.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="databaseFacade">The <see cref="DatabaseFacade" /> for the context.</param>
    /// <returns>The <see cref="MongoClient" /></returns>
    public static MongoClient GetMongoDBClient(this DatabaseFacade databaseFacade)
        => GetService<ISingletonMongoDBClientWrapper>(databaseFacade).Client;

    private static TService GetService<TService>(IInfrastructure<IServiceProvider> databaseFacade)
    {
        var service = databaseFacade.Instance.GetService<TService>();
        if (service == null)
        {
            throw new InvalidOperationException(MongoDBStrings.MongoDBNotInUse);
        }

        return service;
    }

    /// <summary>
    ///     Returns <see langword="true" /> if the database provider currently in use is the MongoDB provider.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This method can only be used after the <see cref="DbContext" /> has been configured because
    ///         it is only then that the provider is known. This means that this method cannot be used
    ///         in <see cref="DbContext.OnConfiguring" /> because this is where application code sets the
    ///         provider to use as part of configuring the context.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    ///     </para>
    /// </remarks>
    /// <param name="database">The facade from <see cref="DbContext.Database" />.</param>
    /// <returns><see langword="true" /> if the MongoDB provider is being used.</returns>
    public static bool IsMongoDB(this DatabaseFacade database)
        => database.ProviderName == typeof(MongoDBOptionsExtension).Assembly.GetName().Name;
}
