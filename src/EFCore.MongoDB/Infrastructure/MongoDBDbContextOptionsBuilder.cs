// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Net;
using IoTSharp.EntityFrameworkCore.MongoDB.Infrastructure.Internal;

namespace IoTSharp.EntityFrameworkCore.MongoDB.Infrastructure;

/// <summary>
///     Allows MongoDB specific configuration to be performed on <see cref="DbContextOptions" />.
/// </summary>
/// <remarks>
///     <para>
///         Instances of this class are returned from a call to
///         <see cref="O:MongoDBDbContextOptionsExtensions.UseMongoDB{TContext}" />
///         and it is not designed to be directly constructed in your application code.
///     </para>
///     <para>
///         See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
///         <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
///     </para>
/// </remarks>
public class MongoDBDbContextOptionsBuilder : IMongoDBDbContextOptionsBuilderInfrastructure
{
    private readonly DbContextOptionsBuilder _optionsBuilder;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MongoDBDbContextOptionsBuilder" /> class.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="optionsBuilder">The options builder.</param>
    public MongoDBDbContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
    {
        _optionsBuilder = optionsBuilder;
    }

    /// <inheritdoc />
    DbContextOptionsBuilder IMongoDBDbContextOptionsBuilderInfrastructure.OptionsBuilder
        => _optionsBuilder;

    /// <summary>
    ///     Configures the context to use the provided <see cref="IExecutionStrategy" />.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="getExecutionStrategy">A function that returns a new instance of an execution strategy.</param>
    public virtual MongoDBDbContextOptionsBuilder ExecutionStrategy(
        Func<ExecutionStrategyDependencies, IExecutionStrategy> getExecutionStrategy)
        => WithOption(e => e.WithExecutionStrategyFactory(Check.NotNull(getExecutionStrategy, nameof(getExecutionStrategy))));

    /// <summary>
    ///     Configures the context to use the provided geo-replicated region.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="region">Azure MongoDB DB region name.</param>
    public virtual MongoDBDbContextOptionsBuilder Region(string region)
        => WithOption(e => e.WithRegion(Check.NotNull(region, nameof(region))));

    /// <summary>
    ///     Limits the operations to the provided endpoint.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="enable"><see langword="true" /> to limit the operations to the provided endpoint.</param>
    public virtual MongoDBDbContextOptionsBuilder LimitToEndpoint(bool enable = true)
        => WithOption(e => e.WithLimitToEndpoint(Check.NotNull(enable, nameof(enable))));

    /// <summary>
    ///     Configures the context to use a specific <see cref="HttpClient" /> factory.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Use
    ///         <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions">
    ///             static lambda expressions
    ///         </see>
    ///         to avoid creating multiple instances.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///         <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    ///     </para>
    /// </remarks>
    /// <param name="httpClientFactory">A function that returns an <see cref="HttpClient" />.</param>
    public virtual MongoDBDbContextOptionsBuilder HttpClientFactory(Func<HttpClient>? httpClientFactory)
        => WithOption(e => e.WithHttpClientFactory(Check.NotNull(httpClientFactory, nameof(httpClientFactory))));

    /// <summary>
    ///     Configures the context to use the provided connection mode.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="connectionMode">Azure MongoDB DB connection mode.</param>
    public virtual MongoDBDbContextOptionsBuilder ConnectionMode(ConnectionMode connectionMode)
        => WithOption(e => e.WithConnectionMode(Check.NotNull(connectionMode, nameof(connectionMode))));

    /// <summary>
    ///     Configures the proxy information used for web requests.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="proxy">The proxy information used for web requests.</param>
    public virtual MongoDBDbContextOptionsBuilder WebProxy(IWebProxy proxy)
        => WithOption(e => e.WithWebProxy(Check.NotNull(proxy, nameof(proxy))));

    /// <summary>
    ///     Configures the timeout when connecting to the Azure MongoDB DB service.
    ///     The number specifies the time to wait for response to come back from network peer.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="timeout">Request timeout.</param>
    public virtual MongoDBDbContextOptionsBuilder RequestTimeout(TimeSpan timeout)
        => WithOption(e => e.WithRequestTimeout(Check.NotNull(timeout, nameof(timeout))));

    /// <summary>
    ///     Configures the amount of time allowed for trying to establish a connection.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="timeout">Open TCP connection timeout.</param>
    public virtual MongoDBDbContextOptionsBuilder OpenTcpConnectionTimeout(TimeSpan timeout)
        => WithOption(e => e.WithOpenTcpConnectionTimeout(Check.NotNull(timeout, nameof(timeout))));

    /// <summary>
    ///     Configures the amount of idle time after which unused connections are closed.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="timeout">Idle connection timeout.</param>
    public virtual MongoDBDbContextOptionsBuilder IdleTcpConnectionTimeout(TimeSpan timeout)
        => WithOption(e => e.WithIdleTcpConnectionTimeout(Check.NotNull(timeout, nameof(timeout))));

    /// <summary>
    ///     Configures the maximum number of concurrent connections allowed for the target service endpoint
    ///     in the Azure MongoDB DB service.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="connectionLimit">The maximum number of concurrent connections allowed.</param>
    public virtual MongoDBDbContextOptionsBuilder GatewayModeMaxConnectionLimit(int connectionLimit)
        => WithOption(e => e.WithGatewayModeMaxConnectionLimit(Check.NotNull(connectionLimit, nameof(connectionLimit))));

    /// <summary>
    ///     Configures the maximum number of TCP connections that may be opened to each MongoDB DB back-end.
    ///     Together with MaxRequestsPerTcpConnection, this setting limits the number of requests that are
    ///     simultaneously sent to a single MongoDB DB back-end (MaxRequestsPerTcpConnection x MaxTcpConnectionPerEndpoint).
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="connectionLimit">The maximum number of TCP connections that may be opened to each MongoDB DB back-end.</param>
    public virtual MongoDBDbContextOptionsBuilder MaxTcpConnectionsPerEndpoint(int connectionLimit)
        => WithOption(e => e.WithMaxTcpConnectionsPerEndpoint(Check.NotNull(connectionLimit, nameof(connectionLimit))));

    /// <summary>
    ///     Configures the number of requests allowed simultaneously over a single TCP connection.
    ///     When more requests are in flight simultaneously, the direct/TCP client will open additional connections.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="requestLimit">The number of requests allowed simultaneously over a single TCP connection.</param>
    public virtual MongoDBDbContextOptionsBuilder MaxRequestsPerTcpConnection(int requestLimit)
        => WithOption(e => e.WithMaxRequestsPerTcpConnection(Check.NotNull(requestLimit, nameof(requestLimit))));

    /// <summary>
    ///     Sets the boolean to only return the headers and status code in the MongoDB DB response for write item operation
    ///     like Create, Upsert, Patch and Replace. Setting the option to false will cause the response to have a null resource.
    ///     This reduces networking and CPU load by not sending the resource back over the network and serializing it on the client.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-dbcontext-options">Using DbContextOptions</see>, and
    ///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
    /// </remarks>
    /// <param name="enabled"><see langword="false" /> to have null resource</param>
    public virtual MongoDBDbContextOptionsBuilder ContentResponseOnWriteEnabled(bool enabled = true)
        => WithOption(e => e.ContentResponseOnWriteEnabled(Check.NotNull(enabled, nameof(enabled))));

    /// <summary>
    ///     Sets an option by cloning the extension used to store the settings. This ensures the builder
    ///     does not modify options that are already in use elsewhere.
    /// </summary>
    /// <param name="setAction">An action to set the option.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    protected virtual MongoDBDbContextOptionsBuilder WithOption(Func<MongoDBOptionsExtension, MongoDBOptionsExtension> setAction)
    {
        ((IDbContextOptionsBuilderInfrastructure)_optionsBuilder).AddOrUpdateExtension(
            setAction(_optionsBuilder.Options.FindExtension<MongoDBOptionsExtension>() ?? new MongoDBOptionsExtension()));

        return this;
    }

    #region Hidden System.Object members

    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string? ToString()
        => base.ToString();

    /// <summary>
    ///     Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><see langword="true" /> if the specified object is equal to the current object; otherwise, <see langword="false" />.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj)
        => base.Equals(obj);

    /// <summary>
    ///     Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
        => base.GetHashCode();

    #endregion
}
