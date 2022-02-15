// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.Extensions;

namespace IoTSharp.EntityFrameworkCore.MongoDB.Metadata.Conventions.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class MongoDBConventionSetBuilder : ProviderConventionSetBuilder
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public MongoDBConventionSetBuilder(
        ProviderConventionSetBuilderDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override ConventionSet CreateConventionSet()
    {
        var conventionSet = base.CreateConventionSet();

        conventionSet.ModelInitializedConventions.Add(new ContextContainerConvention(Dependencies));

        conventionSet.ModelFinalizingConventions.Add(new ETagPropertyConvention());

        var storeKeyConvention = new StoreKeyConvention(Dependencies);
        var discriminatorConvention = new MongoDBDiscriminatorConvention(Dependencies);
        KeyDiscoveryConvention keyDiscoveryConvention = new MongoDBKeyDiscoveryConvention(Dependencies);
        InversePropertyAttributeConvention inversePropertyAttributeConvention =
            new MongoDBInversePropertyAttributeConvention(Dependencies);
        RelationshipDiscoveryConvention relationshipDiscoveryConvention =
            new MongoDBRelationshipDiscoveryConvention(Dependencies);
        conventionSet.EntityTypeAddedConventions.Add(storeKeyConvention);
        conventionSet.EntityTypeAddedConventions.Add(discriminatorConvention);
        ReplaceConvention(conventionSet.EntityTypeAddedConventions, keyDiscoveryConvention);
        ReplaceConvention(conventionSet.EntityTypeAddedConventions, inversePropertyAttributeConvention);
        ReplaceConvention(conventionSet.EntityTypeAddedConventions, relationshipDiscoveryConvention);

        ReplaceConvention(conventionSet.EntityTypeIgnoredConventions, relationshipDiscoveryConvention);

        ReplaceConvention(conventionSet.EntityTypeRemovedConventions, (DiscriminatorConvention)discriminatorConvention);
        ReplaceConvention(conventionSet.EntityTypeRemovedConventions, inversePropertyAttributeConvention);

        ValueGenerationConvention valueGenerationConvention = new MongoDBValueGenerationConvention(Dependencies);
        conventionSet.EntityTypeBaseTypeChangedConventions.Add(storeKeyConvention);
        ReplaceConvention(conventionSet.EntityTypeBaseTypeChangedConventions, valueGenerationConvention);
        ReplaceConvention(conventionSet.EntityTypeBaseTypeChangedConventions, (DiscriminatorConvention)discriminatorConvention);
        ReplaceConvention(conventionSet.EntityTypeBaseTypeChangedConventions, keyDiscoveryConvention);
        ReplaceConvention(conventionSet.EntityTypeBaseTypeChangedConventions, inversePropertyAttributeConvention);
        ReplaceConvention(conventionSet.EntityTypeBaseTypeChangedConventions, relationshipDiscoveryConvention);

        ReplaceConvention(conventionSet.EntityTypeMemberIgnoredConventions, keyDiscoveryConvention);
        ReplaceConvention(conventionSet.EntityTypeMemberIgnoredConventions, inversePropertyAttributeConvention);
        ReplaceConvention(conventionSet.EntityTypeMemberIgnoredConventions, relationshipDiscoveryConvention);

        conventionSet.EntityTypePrimaryKeyChangedConventions.Add(storeKeyConvention);
        ReplaceConvention(conventionSet.EntityTypePrimaryKeyChangedConventions, valueGenerationConvention);

        conventionSet.KeyAddedConventions.Add(storeKeyConvention);

        conventionSet.KeyRemovedConventions.Add(storeKeyConvention);
        ReplaceConvention(conventionSet.KeyRemovedConventions, keyDiscoveryConvention);

        ReplaceConvention(conventionSet.ForeignKeyAddedConventions, keyDiscoveryConvention);
        ReplaceConvention(conventionSet.ForeignKeyAddedConventions, valueGenerationConvention);

        ReplaceConvention(conventionSet.ForeignKeyRemovedConventions, relationshipDiscoveryConvention);
        conventionSet.ForeignKeyRemovedConventions.Add(discriminatorConvention);
        conventionSet.ForeignKeyRemovedConventions.Add(storeKeyConvention);
        ReplaceConvention(conventionSet.ForeignKeyRemovedConventions, keyDiscoveryConvention);
        ReplaceConvention(conventionSet.ForeignKeyRemovedConventions, valueGenerationConvention);

        ReplaceConvention(conventionSet.ForeignKeyPropertiesChangedConventions, keyDiscoveryConvention);
        ReplaceConvention(conventionSet.ForeignKeyPropertiesChangedConventions, valueGenerationConvention);

        ReplaceConvention(conventionSet.ForeignKeyUniquenessChangedConventions, keyDiscoveryConvention);

        conventionSet.ForeignKeyOwnershipChangedConventions.Add(discriminatorConvention);
        conventionSet.ForeignKeyOwnershipChangedConventions.Add(storeKeyConvention);
        ReplaceConvention(conventionSet.ForeignKeyOwnershipChangedConventions, keyDiscoveryConvention);
        ReplaceConvention(conventionSet.ForeignKeyOwnershipChangedConventions, valueGenerationConvention);
        ReplaceConvention(conventionSet.ForeignKeyOwnershipChangedConventions, relationshipDiscoveryConvention);

        ReplaceConvention(conventionSet.ForeignKeyNullNavigationSetConventions, relationshipDiscoveryConvention);

        ReplaceConvention(conventionSet.NavigationAddedConventions, inversePropertyAttributeConvention);
        ReplaceConvention(conventionSet.NavigationAddedConventions, relationshipDiscoveryConvention);

        ReplaceConvention(conventionSet.NavigationRemovedConventions, relationshipDiscoveryConvention);

        ManyToManyJoinEntityTypeConvention manyToManyJoinEntityTypeConvention =
            new MongoDBManyToManyJoinEntityTypeConvention(Dependencies);
        ReplaceConvention(conventionSet.SkipNavigationAddedConventions, manyToManyJoinEntityTypeConvention);

        ReplaceConvention(conventionSet.SkipNavigationRemovedConventions, manyToManyJoinEntityTypeConvention);

        ReplaceConvention(conventionSet.SkipNavigationInverseChangedConventions, manyToManyJoinEntityTypeConvention);

        ReplaceConvention(conventionSet.SkipNavigationForeignKeyChangedConventions, manyToManyJoinEntityTypeConvention);

        conventionSet.EntityTypeAnnotationChangedConventions.Add(discriminatorConvention);
        conventionSet.EntityTypeAnnotationChangedConventions.Add(storeKeyConvention);
        conventionSet.EntityTypeAnnotationChangedConventions.Add((MongoDBValueGenerationConvention)valueGenerationConvention);
        conventionSet.EntityTypeAnnotationChangedConventions.Add((MongoDBKeyDiscoveryConvention)keyDiscoveryConvention);
        conventionSet.EntityTypeAnnotationChangedConventions.Add(
            (MongoDBManyToManyJoinEntityTypeConvention)manyToManyJoinEntityTypeConvention);

        ReplaceConvention(conventionSet.PropertyAddedConventions, keyDiscoveryConvention);

        conventionSet.PropertyAnnotationChangedConventions.Add(storeKeyConvention);

        ReplaceConvention(conventionSet.ModelFinalizingConventions, inversePropertyAttributeConvention);

        ReplaceConvention(
            conventionSet.ModelFinalizedConventions,
            (RuntimeModelConvention)new MongoDBRuntimeModelConvention(Dependencies));

        return conventionSet;
    }

    /// <summary>
    ///     Call this method to build a <see cref="ModelBuilder" /> for MongoDB outside of <see cref="DbContext.OnModelCreating" />.
    /// </summary>
    /// <remarks>
    ///     Note that it is unusual to use this method. Consider using <see cref="DbContext" /> in the normal way instead.
    /// </remarks>
    /// <returns>The convention set.</returns>
    public static ModelBuilder CreateModelBuilder()
    {
        using var serviceScope = CreateServiceScope();
        using var context = serviceScope.ServiceProvider.GetRequiredService<DbContext>();
        return new ModelBuilder(ConventionSet.CreateConventionSet(context), context.GetService<ModelDependencies>());
    }

    private static IServiceScope CreateServiceScope()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkMongoDB()
            .AddDbContext<DbContext>(
                (p, o) =>
                    o.UseMongoDB("localhost", "_")
                        .UseInternalServiceProvider(p))
            .BuildServiceProvider();

        return serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
    }
}
