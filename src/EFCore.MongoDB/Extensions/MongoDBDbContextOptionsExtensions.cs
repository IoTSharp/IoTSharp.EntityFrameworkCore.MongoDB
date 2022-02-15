// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.Infrastructure;
using IoTSharp.EntityFrameworkCore.MongoDB.Infrastructure.Internal;

// ReSharper disable once CheckNamespace
namespace IoTSharp.EntityFrameworkCore.MongoDB.Extensions;

/// <summary>
///     MongoDB-specific extension methods for <see cref="DbContextOptionsBuilder" />.
/// </summary>
/// <remarks>
///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
/// </remarks>
public static class MongoDBDbContextOptionsExtensions
{


    /// <summary>
    ///     Configures the context to connect to an Azure MongoDB database.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="optionsBuilder">The builder being used to configure the context.</param>
    /// <param name="connectionString"></param>
    /// <param name="MongoDBOptionsAction">An optional action to allow additional MongoDB-specific configuration.</param>
    /// <returns>The options builder so that further configuration can be chained.</returns>
    public static DbContextOptionsBuilder UseMongoDB(
        this DbContextOptionsBuilder optionsBuilder,
        string connectionString,
        Action<MongoDBDbContextOptionsBuilder>? MongoDBOptionsAction = null)
    {
        Check.NotNull(optionsBuilder, nameof(optionsBuilder));
        Check.NotEmpty(connectionString, nameof(connectionString));

        var extension = optionsBuilder.Options.FindExtension<MongoDBOptionsExtension>()
            ?? new MongoDBOptionsExtension();

        extension = extension
            .WithConnectionString(connectionString);

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        MongoDBOptionsAction?.Invoke(new MongoDBDbContextOptionsBuilder(optionsBuilder));

        return optionsBuilder;
    }

    /// <summary>
    ///     Configures the context to connect to an Azure MongoDB database.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <typeparam name="TContext">The type of context to be configured.</typeparam>
    /// <param name="optionsBuilder">The builder being used to configure the context.</param>
    /// <param name="connectionString">The connection string of the database to connect to.</param>
    /// <param name="databaseName">The database name.</param>
    /// <param name="MongoDBOptionsAction">An optional action to allow additional MongoDB-specific configuration.</param>
    /// <returns>The options builder so that further configuration can be chained.</returns>
    public static DbContextOptionsBuilder<TContext> UseMongoDB<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        string connectionString,
        string databaseName,
        Action<MongoDBDbContextOptionsBuilder>? MongoDBOptionsAction = null)
        where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)((DbContextOptionsBuilder)optionsBuilder).UseMongoDB(
            connectionString,
            databaseName,
            MongoDBOptionsAction);

    /// <summary>
    ///     Configures the context to connect to an Azure MongoDB database.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="optionsBuilder">The builder being used to configure the context.</param>
    /// <param name="connectionString">The connection string of the database to connect to.</param>
    /// <param name="databaseName">The database name.</param>
    /// <param name="MongoDBOptionsAction">An optional action to allow additional MongoDB-specific configuration.</param>
    /// <returns>The options builder so that further configuration can be chained.</returns>
    public static DbContextOptionsBuilder UseMongoDB(
        this DbContextOptionsBuilder optionsBuilder,
        string connectionString,
        string databaseName,
        Action<MongoDBDbContextOptionsBuilder>? MongoDBOptionsAction = null)
    {
        Check.NotNull(optionsBuilder, nameof(optionsBuilder));
        Check.NotNull(connectionString, nameof(connectionString));
        Check.NotNull(databaseName, nameof(databaseName));

        var extension = optionsBuilder.Options.FindExtension<MongoDBOptionsExtension>()
            ?? new MongoDBOptionsExtension();

        extension = extension
            .WithConnectionString(connectionString)
            .WithDatabaseName(databaseName);

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        MongoDBOptionsAction?.Invoke(new MongoDBDbContextOptionsBuilder(optionsBuilder));

        return optionsBuilder;
    }
}
