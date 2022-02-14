// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.ValueGeneration.Internal;

namespace IoTSharp.EntityFrameworkCore.MongoDB.ValueGeneration;

/// <summary>
///     A factory that creates value generators for the 'id' property that combines the primary key values.
/// </summary>
/// <remarks>
///     See <see href="https://aka.ms/efcore-docs-value-generation">EF Core value generation</see>, and
///     <see href="https://aka.ms/efcore-docs-MongoDB">Accessing Azure MongoDB DB with EF Core</see> for more information and examples.
/// </remarks>
public class IdValueGeneratorFactory : ValueGeneratorFactory
{
    /// <inheritdoc />
    public override ValueGenerator Create(IProperty property, IEntityType entityType)
        => new IdValueGenerator();
}
