// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Text;
using IoTSharp.EntityFrameworkCore.MongoDB.Extensions;

namespace IoTSharp.EntityFrameworkCore.MongoDB.ValueGeneration.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class IdValueGenerator : ValueGenerator
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override bool GeneratesTemporaryValues
        => false;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override bool GeneratesStableValues
        => true;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override object NextValue(EntityEntry entry)
    {
        var builder = new StringBuilder();
        object value =string.Empty;
        var entityType = entry.Metadata;

        var primaryKey = entityType.FindPrimaryKey()!;
        var type = primaryKey.GetKeyType();
        //这里不需要鉴别器， 鉴于 GenerateNewId的实现。 
        //var discriminator = entityType.GetDiscriminatorValue();
        if (type==typeof(ObjectId))
        {
          
            builder.Append(ObjectId.GenerateNewId().ToString());    
        }
        else if (type==typeof(string))
        {

        }
        else if (type== typeof(int))
        {

        }
  
        return  builder.ToString(); 
    }

    private static void AppendString(StringBuilder builder, object? propertyValue)
    {
        switch (propertyValue)
        {
            case string stringValue:
                AppendEscape(builder, stringValue);
                return;
            case IEnumerable enumerable:
                foreach (var item in enumerable)
                {
                    AppendEscape(builder, item.ToString()!);
                    builder.Append('|');
                }

                return;
            case DateTime dateTime:
                AppendEscape(builder, dateTime.ToString("O"));
                return;
            default:
                if (propertyValue == null)
                {
                    builder.Append("null");
                }
                else
                {
                    AppendEscape(builder, propertyValue.ToString()!);
                }

                return;
        }
    }

    private static StringBuilder AppendEscape(StringBuilder builder, string stringValue)
    {
        var startingIndex = builder.Length;
        return builder.Append(stringValue)
            // We need this to avoid collisions with the value separator
            .Replace("|", "^|", startingIndex, builder.Length - startingIndex)
            // These are invalid characters, see https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.documents.resource.id
            .Replace("/", "^2F", startingIndex, builder.Length - startingIndex)
            .Replace("\\", "^5C", startingIndex, builder.Length - startingIndex)
            .Replace("?", "^3F", startingIndex, builder.Length - startingIndex)
            .Replace("#", "^23", startingIndex, builder.Length - startingIndex);
    }
}
