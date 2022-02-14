// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.Extensions;
using IoTSharp.EntityFrameworkCore.MongoDB.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Metadata.Conventions;
using IoTSharp.EntityFrameworkCore.MongoDB.Metadata.Internal;

namespace IoTSharp.EntityFrameworkCore.MongoDB.Infrastructure.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class MongoDBModelValidator : ModelValidator
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public MongoDBModelValidator(ModelValidatorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override void Validate(IModel model, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        base.Validate(model, logger);

        ValidateDatabaseProperties(model, logger);
        ValidateKeys(model, logger);
        ValidateSharedContainerCompatibility(model, logger);
        ValidateOnlyETagConcurrencyToken(model, logger);
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected virtual void ValidateSharedContainerCompatibility(
        IModel model,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        var containers = new Dictionary<string, List<IEntityType>>();
        foreach (var entityType in model.GetEntityTypes().Where(et => et.FindPrimaryKey() != null))
        {
            var container = entityType.GetContainer();
            if (container == null)
            {
                continue;
            }

            if (!containers.TryGetValue(container, out var mappedTypes))
            {
                mappedTypes = new List<IEntityType>();
                containers[container] = mappedTypes;
            }

            mappedTypes.Add(entityType);
        }

        foreach (var (container, mappedTypes) in containers)
        {
            ValidateSharedContainerCompatibility(mappedTypes, container, logger);
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected virtual void ValidateSharedContainerCompatibility(
        IReadOnlyList<IEntityType> mappedTypes,
        string container,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        var discriminatorValues = new Dictionary<object, IEntityType>();
        IProperty? partitionKey = null;
        int? analyticalTtl = null;
        int? defaultTtl = null;
        ThroughputProperties? throughput = null;
        IEntityType? firstEntityType = null;
        foreach (var entityType in mappedTypes)
        {
            Check.DebugAssert(entityType.IsDocumentRoot(), "Only document roots expected here.");
            var partitionKeyPropertyName = entityType.GetPartitionKeyPropertyName();
            if (partitionKeyPropertyName != null)
            {
                var nextPartitionKeyProperty = entityType.FindProperty(partitionKeyPropertyName)!;
                if (partitionKey == null)
                {
                    if (firstEntityType != null)
                    {
                        throw new InvalidOperationException(MongoDBStrings.NoPartitionKey(firstEntityType.DisplayName(), container));
                    }

                    partitionKey = nextPartitionKeyProperty;
                }
                else if (partitionKey.GetJsonPropertyName() != nextPartitionKeyProperty.GetJsonPropertyName())
                {
                    throw new InvalidOperationException(
                        MongoDBStrings.PartitionKeyStoreNameMismatch(
                            partitionKey.Name, firstEntityType!.DisplayName(), partitionKey.GetJsonPropertyName(),
                            nextPartitionKeyProperty.Name, entityType.DisplayName(), nextPartitionKeyProperty.GetJsonPropertyName()));
                }
            }
            else if (partitionKey != null)
            {
                throw new InvalidOperationException(MongoDBStrings.NoPartitionKey(entityType.DisplayName(), container));
            }

            if (mappedTypes.Count == 1)
            {
                break;
            }

            firstEntityType ??= entityType;

            if (entityType.ClrType.IsInstantiable()
                && entityType.GetContainingPropertyName() == null)
            {
                if (entityType.FindDiscriminatorProperty() == null)
                {
                    throw new InvalidOperationException(
                        MongoDBStrings.NoDiscriminatorProperty(entityType.DisplayName(), container));
                }

                var discriminatorValue = entityType.GetDiscriminatorValue();
                if (discriminatorValue == null)
                {
                    throw new InvalidOperationException(
                        MongoDBStrings.NoDiscriminatorValue(entityType.DisplayName(), container));
                }

                if (discriminatorValues.TryGetValue(discriminatorValue, out var duplicateEntityType))
                {
                    throw new InvalidOperationException(
                        MongoDBStrings.DuplicateDiscriminatorValue(
                            entityType.DisplayName(), discriminatorValue, duplicateEntityType.DisplayName(), container));
                }

                discriminatorValues[discriminatorValue] = entityType;
            }

            var currentAnalyticalTtl = entityType.GetAnalyticalStoreTimeToLive();
            if (currentAnalyticalTtl != null)
            {
                if (analyticalTtl == null)
                {
                    analyticalTtl = currentAnalyticalTtl;
                }
                else if (analyticalTtl != currentAnalyticalTtl)
                {
                    var conflictingEntityType = mappedTypes.First(et => et.GetAnalyticalStoreTimeToLive() != null);
                    throw new InvalidOperationException(
                        MongoDBStrings.AnalyticalTTLMismatch(
                            analyticalTtl, conflictingEntityType.DisplayName(), entityType.DisplayName(), currentAnalyticalTtl,
                            container));
                }
            }

            var currentDefaultTtl = entityType.GetDefaultTimeToLive();
            if (currentDefaultTtl != null)
            {
                if (defaultTtl == null)
                {
                    defaultTtl = currentDefaultTtl;
                }
                else if (defaultTtl != currentDefaultTtl)
                {
                    var conflictingEntityType = mappedTypes.First(et => et.GetDefaultTimeToLive() != null);
                    throw new InvalidOperationException(
                        MongoDBStrings.DefaultTTLMismatch(
                            defaultTtl, conflictingEntityType.DisplayName(), entityType.DisplayName(), currentDefaultTtl, container));
                }
            }

            var currentThroughput = entityType.GetThroughput();
            if (currentThroughput != null)
            {
                if (throughput == null)
                {
                    throughput = currentThroughput;
                }
                else if ((throughput.AutoscaleMaxThroughput ?? throughput.Throughput)
                         != (currentThroughput.AutoscaleMaxThroughput ?? currentThroughput.Throughput))
                {
                    var conflictingEntityType = mappedTypes.First(et => et.GetThroughput() != null);
                    throw new InvalidOperationException(
                        MongoDBStrings.ThroughputMismatch(
                            throughput.AutoscaleMaxThroughput ?? throughput.Throughput, conflictingEntityType.DisplayName(),
                            entityType.DisplayName(), currentThroughput.AutoscaleMaxThroughput ?? currentThroughput.Throughput,
                            container));
                }
                else if ((throughput.AutoscaleMaxThroughput == null)
                         != (currentThroughput.AutoscaleMaxThroughput == null))
                {
                    var conflictingEntityType = mappedTypes.First(et => et.GetThroughput() != null);
                    var autoscaleType = throughput.AutoscaleMaxThroughput == null
                        ? entityType
                        : conflictingEntityType;
                    var manualType = throughput.AutoscaleMaxThroughput != null
                        ? entityType
                        : conflictingEntityType;

                    throw new InvalidOperationException(
                        MongoDBStrings.ThroughputTypeMismatch(
                            manualType.DisplayName(), autoscaleType.DisplayName(), container));
                }
            }
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected virtual void ValidateOnlyETagConcurrencyToken(
        IModel model,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        foreach (var entityType in model.GetEntityTypes())
        {
            foreach (var property in entityType.GetDeclaredProperties())
            {
                if (property.IsConcurrencyToken)
                {
                    var storeName = property.GetJsonPropertyName();
                    if (storeName != "_etag")
                    {
                        throw new InvalidOperationException(
                            MongoDBStrings.NonETagConcurrencyToken(entityType.DisplayName(), storeName));
                    }

                    var etagType = property.GetTypeMapping().Converter?.ProviderClrType ?? property.ClrType;
                    if (etagType != typeof(string))
                    {
                        throw new InvalidOperationException(
                            MongoDBStrings.ETagNonStringStoreType(
                                property.Name, entityType.DisplayName(), etagType.ShortDisplayName()));
                    }
                }
            }
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected virtual void ValidateKeys(
        IModel model,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        foreach (var entityType in model.GetEntityTypes())
        {
            var primaryKey = entityType.FindPrimaryKey();
            if (primaryKey == null
                || !entityType.IsDocumentRoot())
            {
                continue;
            }

            var idProperty = entityType.GetProperties()
                .FirstOrDefault(p => p.GetJsonPropertyName() == StoreKeyConvention.IdPropertyJsonName);
            if (idProperty == null)
            {
                throw new InvalidOperationException(MongoDBStrings.NoIdProperty(entityType.DisplayName()));
            }

            var idType = idProperty.GetTypeMapping().Converter?.ProviderClrType
                ?? idProperty.ClrType;
            if (idType != typeof(string))
            {
                throw new InvalidOperationException(
                    MongoDBStrings.IdNonStringStoreType(
                        idProperty.Name, entityType.DisplayName(), idType.ShortDisplayName()));
            }

            if (!idProperty.IsKey())
            {
                throw new InvalidOperationException(MongoDBStrings.NoIdKey(entityType.DisplayName(), idProperty.Name));
            }

            var partitionKeyPropertyName = entityType.GetPartitionKeyPropertyName();
            if (partitionKeyPropertyName != null)
            {
                var partitionKey = entityType.FindProperty(partitionKeyPropertyName);
                if (partitionKey == null)
                {
                    throw new InvalidOperationException(
                        MongoDBStrings.PartitionKeyMissingProperty(entityType.DisplayName(), partitionKeyPropertyName));
                }

                var partitionKeyType = partitionKey.GetTypeMapping().Converter?.ProviderClrType
                    ?? partitionKey.ClrType;
                if (partitionKeyType != typeof(string))
                {
                    throw new InvalidOperationException(
                        MongoDBStrings.PartitionKeyNonStringStoreType(
                            partitionKeyPropertyName, entityType.DisplayName(), partitionKeyType.ShortDisplayName()));
                }

                if (!partitionKey.GetContainingKeys().Any(k => k.Properties.Contains(idProperty)))
                {
                    throw new InvalidOperationException(
                        MongoDBStrings.NoPartitionKeyKey(
                            entityType.DisplayName(), partitionKeyPropertyName, idProperty.Name));
                }
            }
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected virtual void ValidateDatabaseProperties(
        IModel model,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        foreach (var entityType in model.GetEntityTypes())
        {
            var properties = new Dictionary<string, IPropertyBase>();
            foreach (var property in entityType.GetProperties())
            {
                var jsonName = property.GetJsonPropertyName();
                if (string.IsNullOrWhiteSpace(jsonName))
                {
                    continue;
                }

                if (properties.TryGetValue(jsonName, out var otherProperty))
                {
                    throw new InvalidOperationException(
                        MongoDBStrings.JsonPropertyCollision(property.Name, otherProperty.Name, entityType.DisplayName(), jsonName));
                }

                properties[jsonName] = property;
            }

            foreach (var navigation in entityType.GetNavigations())
            {
                if (!navigation.IsEmbedded())
                {
                    continue;
                }

                var jsonName = navigation.TargetEntityType.GetContainingPropertyName()!;
                if (properties.TryGetValue(jsonName, out var otherProperty))
                {
                    throw new InvalidOperationException(
                        MongoDBStrings.JsonPropertyCollision(navigation.Name, otherProperty.Name, entityType.DisplayName(), jsonName));
                }

                properties[jsonName] = navigation;
            }
        }
    }
}
