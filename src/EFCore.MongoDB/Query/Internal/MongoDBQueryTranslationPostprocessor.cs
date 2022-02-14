// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace IoTSharp.EntityFrameworkCore.MongoDB.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class MongoDBQueryTranslationPostprocessor : QueryTranslationPostprocessor
{
    private readonly ISqlExpressionFactory _sqlExpressionFactory;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public MongoDBQueryTranslationPostprocessor(
        QueryTranslationPostprocessorDependencies dependencies,
        ISqlExpressionFactory sqlExpressionFactory,
        QueryCompilationContext queryCompilationContext)
        : base(dependencies, queryCompilationContext)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override Expression Process(Expression query)
    {
        query = base.Process(query);

        if (query is ShapedQueryExpression { QueryExpression: SelectExpression selectExpression })
        {
            // MongoDB does not have nested select expression so this should be safe.
            selectExpression.ApplyProjection();
        }

        query = new MongoDBValueConverterCompensatingExpressionVisitor(_sqlExpressionFactory).Visit(query);

        return query;
    }
}
