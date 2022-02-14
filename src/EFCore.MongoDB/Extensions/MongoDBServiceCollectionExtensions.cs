// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using IoTSharp.EntityFrameworkCore.MongoDB.Diagnostics.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Infrastructure;
using IoTSharp.EntityFrameworkCore.MongoDB.Infrastructure.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Metadata.Conventions.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Query.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Storage.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.ValueGeneration.Internal;

// ReSharper disable once CheckNamespace
namespace IoTSharp.EntityFrameworkCore.MongoDB.Extensions;

/// <summary>
///     MongoDB-specific extension methods for <see cref="IServiceCollection" />.
/// </summary>
/// <remarks>
///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
/// </remarks>
public static class MongoDBServiceCollectionExtensions
{
    /// <summary>
    ///     Registers the given Entity Framework <see cref="DbContext" /> as a service in the <see cref="IServiceCollection" />
    ///     and configures it to connect to an Azure MongoDB database.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This method is a shortcut for configuring a <see cref="DbContext" /> to use MongoDB. It does not support all options.
    ///         Use <see cref="O:EntityFrameworkServiceCollectionExtensions.AddDbContext" /> and related methods for full control of
    ///         this process.
    ///     </para>
    ///     <para>
    ///         Use this method when using dependency injection in your application, such as with ASP.NET Core.
    ///         For applications that don't use dependency injection, consider creating <see cref="DbContext" />
    ///         instances directly with its constructor. The <see cref="DbContext.OnConfiguring" /> method can then be
    ///         overridden to configure the MongoDB database provider.
    ///     </para>
    ///     <para>
    ///         To configure the <see cref="DbContextOptions{TContext}" /> for the context, either override the
    ///         <see cref="DbContext.OnConfiguring" /> method in your derived context, or supply
    ///         an optional action to configure the <see cref="DbContextOptions" /> for the context.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///         <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    ///     </para>
    /// </remarks>
    /// <typeparam name="TContext">The type of context to be registered.</typeparam>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">The connection string of the database to connect to.</param>
    /// <param name="databaseName">The database name.</param>
    /// <param name="MongoDBOptionsAction">An optional action to allow additional MongoDB-specific configuration.</param>
    /// <param name="optionsAction">An optional action to configure the <see cref="DbContextOptions" /> for the context.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMongoDB<TContext>(
        this IServiceCollection serviceCollection,
        string connectionString,
        string databaseName,
        Action<MongoDBDbContextOptionsBuilder>? MongoDBOptionsAction = null,
        Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContext : DbContext
        => serviceCollection.AddDbContext<TContext>(
            (serviceProvider, options) =>
            {
                optionsAction?.Invoke(options);
                options.UseMongoDB(connectionString, databaseName, MongoDBOptionsAction);
            });

    /// <summary>
    ///     <para>
    ///         Adds the services required by the Azure MongoDB database provider for Entity Framework
    ///         to an <see cref="IServiceCollection" />.
    ///     </para>
    ///     <para>
    ///         Warning: Do not call this method accidentally. It is much more likely you need
    ///         to call <see cref="AddMongoDB{TContext}" />.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     Calling this method is no longer necessary when building most applications, including those that
    ///     use dependency injection in ASP.NET or elsewhere.
    ///     It is only needed when building the internal service provider for use with
    ///     the <see cref="DbContextOptionsBuilder.UseInternalServiceProvider" /> method.
    ///     This is not recommend other than for some advanced scenarios.
    /// </remarks>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>
    ///     The same service collection so that multiple calls can be chained.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IServiceCollection AddEntityFrameworkMongoDB(this IServiceCollection serviceCollection)
    {
        var builder = new EntityFrameworkServicesBuilder(serviceCollection)
            .TryAdd<LoggingDefinitions, MongoDBLoggingDefinitions>()
            .TryAdd<IDatabaseProvider, DatabaseProvider<MongoDBOptionsExtension>>()
            .TryAdd<IDatabase, MongoDBDatabaseWrapper>()
            .TryAdd<IExecutionStrategyFactory, MongoDBExecutionStrategyFactory>()
            .TryAdd<IDbContextTransactionManager, MongoDBTransactionManager>()
            .TryAdd<IModelValidator, MongoDBModelValidator>()
            .TryAdd<IProviderConventionSetBuilder, MongoDBConventionSetBuilder>()
            .TryAdd<IValueGeneratorSelector, MongoDBValueGeneratorSelector>()
            .TryAdd<IDatabaseCreator, MongoDBDatabaseCreator>()
            .TryAdd<IQueryContextFactory, MongoDBQueryContextFactory>()
            .TryAdd<ITypeMappingSource, MongoDBMappingSource>()
            .TryAdd<IQueryableMethodTranslatingExpressionVisitorFactory, MongoDBQueryableMethodTranslatingExpressionVisitorFactory>()
            .TryAdd<IShapedQueryCompilingExpressionVisitorFactory, MongoDBShapedQueryCompilingExpressionVisitorFactory>()
            .TryAdd<ISingletonOptions, IMongoDBSingletonOptions>(p => p.GetRequiredService<IMongoDBSingletonOptions>())
            .TryAdd<IQueryTranslationPreprocessorFactory, MongoDBQueryTranslationPreprocessorFactory>()
            .TryAdd<IQueryCompilationContextFactory, MongoDBQueryCompilationContextFactory>()
            .TryAdd<IQueryTranslationPostprocessorFactory, MongoDBQueryTranslationPostprocessorFactory>()
            .TryAddProviderSpecificServices(
                b => b
                    .TryAddSingleton<IMongoDBSingletonOptions, MongoDBSingletonOptions>()
                    .TryAddSingleton<ISingletonMongoDBClientWrapper, SingletonMongoDBClientWrapper>()
                    .TryAddSingleton<IQuerySqlGeneratorFactory, QuerySqlGeneratorFactory>()
                    .TryAddScoped<ISqlExpressionFactory, SqlExpressionFactory>()
                    .TryAddScoped<IMemberTranslatorProvider, MongoDBMemberTranslatorProvider>()
                    .TryAddScoped<IMethodCallTranslatorProvider, MongoDBMethodCallTranslatorProvider>()
                    .TryAddScoped<IMongoDBClientWrapper, MongoDBClientWrapper>()
            );

        builder.TryAddCoreServices();

        return serviceCollection;
    }
}
