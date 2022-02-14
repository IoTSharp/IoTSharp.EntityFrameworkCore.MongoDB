// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace IoTSharp.EntityFrameworkCore.MongoDB.Query.Internal;

public partial class MongoDBShapedQueryCompilingExpressionVisitor
{
    private sealed class MongoDBProjectionBindingRemovingReadItemExpressionVisitor : MongoDBProjectionBindingRemovingExpressionVisitorBase
    {
        private readonly ReadItemExpression _readItemExpression;

        public MongoDBProjectionBindingRemovingReadItemExpressionVisitor(
            ReadItemExpression readItemExpression,
            ParameterExpression jObjectParameter,
            bool trackQueryResults)
            : base(jObjectParameter, trackQueryResults)
        {
            _readItemExpression = readItemExpression;
        }

        protected override ProjectionExpression GetProjection(ProjectionBindingExpression _)
            => _readItemExpression.ProjectionExpression;
    }
}
