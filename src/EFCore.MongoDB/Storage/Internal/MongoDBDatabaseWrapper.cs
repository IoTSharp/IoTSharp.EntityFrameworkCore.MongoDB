// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Extensions;
using IoTSharp.EntityFrameworkCore.MongoDB.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Metadata.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Update.Internal;
using Database = Microsoft.EntityFrameworkCore.Storage.Database;
using MongoDB.Bson;

namespace IoTSharp.EntityFrameworkCore.MongoDB.Storage.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class MongoDBDatabaseWrapper : Database
{
    private readonly Dictionary<IEntityType, DocumentSource> _documentCollections = new();

    private readonly IMongoDBClientWrapper _MongoDBClient;
    private readonly bool _sensitiveLoggingEnabled;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public MongoDBDatabaseWrapper(
        DatabaseDependencies dependencies,
        IMongoDBClientWrapper MongoDBClient,
        ILoggingOptions loggingOptions)
        : base(dependencies)
    {
        _MongoDBClient = MongoDBClient;

        if (loggingOptions.IsSensitiveDataLoggingEnabled)
        {
            _sensitiveLoggingEnabled = true;
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override int SaveChanges(IList<IUpdateEntry> entries)
    {
        var rowsAffected = 0;
        var entriesSaved = new HashSet<IUpdateEntry>();
        var rootEntriesToSave = new HashSet<IUpdateEntry>();

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var entityType = entry.EntityType;

            Check.DebugAssert(!entityType.IsAbstract(), $"{entityType} is abstract");

            if (!entityType.IsDocumentRoot())
            {
#pragma warning disable EF1001 // Internal EF Core API usage.
                // #16707
                var root = GetRootDocument((InternalEntityEntry)entry);
#pragma warning restore EF1001 // Internal EF Core API usage.
                if (!entriesSaved.Contains(root)
                    && rootEntriesToSave.Add(root)
                    && root.EntityState == EntityState.Unchanged)
                {
#pragma warning disable EF1001 // Internal EF Core API usage.
                    // #16707
                    ((InternalEntityEntry)root).SetEntityState(EntityState.Modified);
#pragma warning restore EF1001 // Internal EF Core API usage.
                }

                continue;
            }

            entriesSaved.Add(entry);

            try
            {
                if (Save(entry))
                {
                    rowsAffected++;
                }
            }
            catch (Exception ex) when (ex is not DbUpdateException and not OperationCanceledException)
            {
                throw WrapUpdateException(ex, entry);
            }
        }

        foreach (var rootEntry in rootEntriesToSave)
        {
            if (!entriesSaved.Contains(rootEntry)
                && Save(rootEntry))
            {
                rowsAffected++;
            }
        }

        return rowsAffected;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override async Task<int> SaveChangesAsync(
        IList<IUpdateEntry> entries,
        CancellationToken cancellationToken = default)
    {
        var rowsAffected = 0;
        var entriesSaved = new HashSet<IUpdateEntry>();
        var rootEntriesToSave = new HashSet<IUpdateEntry>();

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var entityType = entry.EntityType;

            Check.DebugAssert(!entityType.IsAbstract(), $"{entityType} is abstract");

            if (!entityType.IsDocumentRoot())
            {
                var root = GetRootDocument((InternalEntityEntry)entry);
                if (!entriesSaved.Contains(root)
                    && rootEntriesToSave.Add(root)
                    && root.EntityState == EntityState.Unchanged)
                {
#pragma warning disable EF1001 // Internal EF Core API usage.
                    // #16707
                    ((InternalEntityEntry)root).SetEntityState(EntityState.Modified);
#pragma warning restore EF1001 // Internal EF Core API usage.
                }

                continue;
            }

            entriesSaved.Add(entry);
            try
            {
                if (await SaveAsync(entry, cancellationToken).ConfigureAwait(false))
                {
                    rowsAffected++;
                }
            }
            catch (Exception ex) when (ex is not DbUpdateException and not OperationCanceledException)
            {
                throw WrapUpdateException(ex, entry);
            }
        }

        foreach (var rootEntry in rootEntriesToSave)
        {
            if (!entriesSaved.Contains(rootEntry)
                && await SaveAsync(rootEntry, cancellationToken).ConfigureAwait(false))
            {
                rowsAffected++;
            }
        }

        return rowsAffected;
    }

    private bool Save(IUpdateEntry entry)
    {
        var entityType = entry.EntityType;
        var documentSource = GetDocumentSource(entityType);
        var collectionId = documentSource.GetContainerId();
        var state = entry.EntityState;

        if (entry.SharedIdentityEntry != null)
        {
            if (entry.EntityState == EntityState.Deleted)
            {
                return false;
            }

            if (state == EntityState.Added)
            {
                state = EntityState.Modified;
            }
        }

        switch (state)
        {
            case EntityState.Added:
                var newDocument = documentSource.GetCurrentDocument(entry);
                if (newDocument != null)
                {
                    documentSource.UpdateDocument(newDocument, entry);
                }
                else
                {
                    newDocument = documentSource.CreateDocument(entry);
                }

                return _MongoDBClient.CreateItem(collectionId, newDocument,entry);

            case EntityState.Modified:
                var document = documentSource.GetCurrentDocument(entry);
                if (document != null)
                {
                    if (documentSource.UpdateDocument(document, entry) == null)
                    {
                        return false;
                    }
                }
                else
                {
                    document = documentSource.CreateDocument(entry);

                    var propertyName = entityType.FindDiscriminatorProperty()?.GetJsonPropertyName();
                    if (propertyName != null)
                    {
                        document[propertyName] =
                            BsonValue.Create(entityType.GetDiscriminatorValue());
                    }
                }

                return _MongoDBClient.ReplaceItem(
                    collectionId, documentSource.GetId(entry.SharedIdentityEntry ?? entry), document, entry);

            case EntityState.Deleted:
                return _MongoDBClient.DeleteItem(collectionId, documentSource.GetId(entry), entry);

            default:
                return false;
        }
    }

    private Task<bool> SaveAsync(IUpdateEntry entry, CancellationToken cancellationToken)
    {
        var entityType = entry.EntityType;
        var documentSource = GetDocumentSource(entityType);
        var collectionId = documentSource.GetContainerId();
        var state = entry.EntityState;

        if (entry.SharedIdentityEntry != null)
        {
            if (entry.EntityState == EntityState.Deleted)
            {
                return Task.FromResult(false);
            }

            if (state == EntityState.Added)
            {
                state = EntityState.Modified;
            }
        }

        switch (state)
        {
            case EntityState.Added:
                var newDocument = documentSource.GetCurrentDocument(entry);
                if (newDocument != null)
                {
                    documentSource.UpdateDocument(newDocument, entry);
                }
                else
                {
                    newDocument = documentSource.CreateDocument(entry);
                }

                return _MongoDBClient.CreateItemAsync(
                    collectionId,   newDocument, entry, cancellationToken);

            case EntityState.Modified:
                var document = documentSource.GetCurrentDocument(entry);
                if (document != null)
                {
                    if (documentSource.UpdateDocument(document, entry) == null)
                    {
                        return Task.FromResult(false);
                    }
                }
                else
                {
                    document = documentSource.CreateDocument(entry);

                    var propertyName = entityType.FindDiscriminatorProperty()?.GetJsonPropertyName();
                    if (propertyName != null)
                    {
                        document[propertyName] =
                            BsonValue.Create(entityType.GetDiscriminatorValue());
                    }
                }

                return _MongoDBClient.ReplaceItemAsync(
                    collectionId,
                    documentSource.GetId(entry.SharedIdentityEntry ?? entry),
                    document,
                    entry,
                    cancellationToken);

            case EntityState.Deleted:
                return _MongoDBClient.DeleteItemAsync(
                    collectionId, documentSource.GetId(entry), entry, cancellationToken);

            default:
                return Task.FromResult(false);
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual DocumentSource GetDocumentSource(IEntityType entityType)
    {
        if (!_documentCollections.TryGetValue(entityType, out var documentSource))
        {
            _documentCollections.Add(
                entityType, documentSource = new DocumentSource(entityType, this));
        }

        return documentSource;
    }

#pragma warning disable EF1001 // Internal EF Core API usage.
    // Issue #16707
    private IUpdateEntry GetRootDocument(InternalEntityEntry entry)
    {
        var stateManager = entry.StateManager;
        var ownership = entry.EntityType.FindOwnership()!;
        var principal = stateManager.FindPrincipal(entry, ownership);
        if (principal == null)
        {
            if (_sensitiveLoggingEnabled)
            {
                throw new InvalidOperationException(
                    MongoDBStrings.OrphanedNestedDocumentSensitive(
                        entry.EntityType.DisplayName(),
                        ownership.PrincipalEntityType.DisplayName(),
                        entry.BuildCurrentValuesString(entry.EntityType.FindPrimaryKey()!.Properties)));
            }

            throw new InvalidOperationException(
                MongoDBStrings.OrphanedNestedDocument(
                    entry.EntityType.DisplayName(),
                    ownership.PrincipalEntityType.DisplayName()));
        }

        return principal.EntityType.IsDocumentRoot() ? principal : GetRootDocument(principal);
    }
#pragma warning restore EF1001 // Internal EF Core API usage.

    private Exception WrapUpdateException(Exception exception, IUpdateEntry entry)
    {
        var documentSource = GetDocumentSource(entry.EntityType);
        var id = documentSource.GetId(entry.SharedIdentityEntry ?? entry);

        return exception switch
        {
             MongoDBException { StatusCode: HttpStatusCode.PreconditionFailed }
                => new DbUpdateConcurrencyException(MongoDBStrings.UpdateConflict(id), exception, new[] { entry }),
            MongoDBException { StatusCode: HttpStatusCode.Conflict }
                => new DbUpdateException(MongoDBStrings.UpdateConflict(id), exception, new[] { entry }),
            _ => new DbUpdateException(MongoDBStrings.UpdateStoreException(id), exception, new[] { entry })
        };
    }
}
