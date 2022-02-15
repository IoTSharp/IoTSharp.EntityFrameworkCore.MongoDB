// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace IoTSharp.EntityFrameworkCore.MongoDB.Storage.Internal
{
    public class FeedIterator : IFindFluent<BsonDocument, BsonValue>
    {
        public FilterDefinition<BsonDocument> Filter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public FindOptions<BsonDocument, BsonValue> Options => throw new NotImplementedException();

        public IFindFluent<BsonDocument, TResult> As<TResult>(global::MongoDB.Bson.Serialization.IBsonSerializer<TResult> resultSerializer = null)
        {
            throw new NotImplementedException();
        }

        public long Count(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public long CountDocuments(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<long> CountDocumentsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IFindFluent<BsonDocument, BsonValue> Limit(int? limit)
        {
            throw new NotImplementedException();
        }

        public IFindFluent<BsonDocument, TNewProjection> Project<TNewProjection>(ProjectionDefinition<BsonDocument, TNewProjection> projection)
        {
            throw new NotImplementedException();
        }

        public IFindFluent<BsonDocument, BsonValue> Skip(int? skip)
        {
            throw new NotImplementedException();
        }

        public IFindFluent<BsonDocument, BsonValue> Sort(SortDefinition<BsonDocument> sort)
        {
            throw new NotImplementedException();
        }

        public IAsyncCursor<BsonValue> ToCursor(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IAsyncCursor<BsonValue>> ToCursorAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
