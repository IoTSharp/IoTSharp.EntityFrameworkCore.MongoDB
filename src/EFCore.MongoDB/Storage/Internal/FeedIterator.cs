// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Newtonsoft.Json.Linq;

namespace IoTSharp.EntityFrameworkCore.MongoDB.Storage.Internal
{
    public class FeedIterator : IFindFluent<Newtonsoft.Json.Linq.JObject, Newtonsoft.Json.Linq.JToken>
    {
        public FilterDefinition<JObject> Filter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public FindOptions<JObject, JToken> Options => throw new NotImplementedException();

        public IFindFluent<JObject, TResult> As<TResult>(global::MongoDB.Bson.Serialization.IBsonSerializer<TResult> resultSerializer = null)
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

        public IFindFluent<JObject, JToken> Limit(int? limit)
        {
            throw new NotImplementedException();
        }

        public IFindFluent<JObject, TNewProjection> Project<TNewProjection>(ProjectionDefinition<JObject, TNewProjection> projection)
        {
            throw new NotImplementedException();
        }

        public IFindFluent<JObject, JToken> Skip(int? skip)
        {
            throw new NotImplementedException();
        }

        public IFindFluent<JObject, JToken> Sort(SortDefinition<JObject> sort)
        {
            throw new NotImplementedException();
        }

        public IAsyncCursor<JToken> ToCursor(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IAsyncCursor<JToken>> ToCursorAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
