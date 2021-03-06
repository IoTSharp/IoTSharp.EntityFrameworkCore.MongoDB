// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IoTSharp.EntityFrameworkCore.MongoDB.Extensions;

namespace IoTSharp.EntityFrameworkCore.MongoDB.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class MongoDBQueryMetadataExtractingExpressionVisitor : ExpressionVisitor
{
    private readonly MongoDBQueryCompilationContext _MongoDBQueryCompilationContext;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public MongoDBQueryMetadataExtractingExpressionVisitor(MongoDBQueryCompilationContext MongoDBQueryCompilationContext)
    {
        _MongoDBQueryCompilationContext = MongoDBQueryCompilationContext;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {
        if (methodCallExpression.Method.IsGenericMethod
            && methodCallExpression.Method.GetGenericMethodDefinition() == MongoDBQueryableExtensions.WithPartitionKeyMethodInfo)
        {
            var innerQueryable = Visit(methodCallExpression.Arguments[0]);

            _MongoDBQueryCompilationContext.PartitionKeyFromExtension = methodCallExpression.Arguments[1].GetConstantValue<string>();

            return innerQueryable;
        }

        return base.VisitMethodCall(methodCallExpression);
    }
}
