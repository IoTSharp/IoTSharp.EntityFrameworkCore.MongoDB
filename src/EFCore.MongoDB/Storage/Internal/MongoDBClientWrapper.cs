// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using IoTSharp.EntityFrameworkCore.MongoDB.Diagnostics.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Extensions;
using IoTSharp.EntityFrameworkCore.MongoDB.Infrastructure.Internal;
using IoTSharp.EntityFrameworkCore.MongoDB.Metadata.Conventions;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver.Core.WireProtocol.Messages;


namespace IoTSharp.EntityFrameworkCore.MongoDB.Storage.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class MongoDBClientWrapper : IMongoDBClientWrapper
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
   // public static readonly JsonSerializer Serializer;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static readonly string DefaultKey = "_id";

    private readonly ISingletonMongoDBClientWrapper _singletonWrapper;
    private readonly string _databaseId;
    private readonly IExecutionStrategy _executionStrategy;
    private readonly IDiagnosticsLogger<DbLoggerCategory.Database.Command> _commandLogger;
    private readonly bool? _enableContentResponseOnWrite;

    static MongoDBClientWrapper()
    {
        //Serializer = JsonSerializer.Create();
        //Serializer.Converters.Add(new ByteArrayConverter());
        //Serializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        //Serializer.DateParseHandling = DateParseHandling.None;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public MongoDBClientWrapper(
        ISingletonMongoDBClientWrapper singletonWrapper,
        IDbContextOptions dbContextOptions,
        IExecutionStrategy executionStrategy,
        IDiagnosticsLogger<DbLoggerCategory.Database.Command> commandLogger)
    {
        var options = dbContextOptions.FindExtension<MongoDBOptionsExtension>();

        _singletonWrapper = singletonWrapper;
        _databaseId = options!.DatabaseName;
        _executionStrategy = executionStrategy;
        _commandLogger = commandLogger;
        _enableContentResponseOnWrite = options.EnableContentResponseOnWrite;
    }

    private MongoClient Client
        => _singletonWrapper.Client;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual bool CreateDatabaseIfNotExists(ThroughputProperties? throughput)
        => _executionStrategy.Execute((throughput, this), CreateDatabaseIfNotExistsOnce, null);

    private static bool CreateDatabaseIfNotExistsOnce(
        DbContext? context,
        (ThroughputProperties? Throughput, MongoDBClientWrapper Wrapper) parameters)
        => CreateDatabaseIfNotExistsOnceAsync(context, parameters).GetAwaiter().GetResult();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual Task<bool> CreateDatabaseIfNotExistsAsync(
        ThroughputProperties? throughput,
        CancellationToken cancellationToken = default)
        => _executionStrategy.ExecuteAsync(
            (throughput, this), CreateDatabaseIfNotExistsOnceAsync, null, cancellationToken);

    private static async Task<bool> CreateDatabaseIfNotExistsOnceAsync(
        DbContext? _,
        (ThroughputProperties? Throughput, MongoDBClientWrapper Wrapper) parameters,
        CancellationToken cancellationToken = default)
    {
        var (throughput, wrapper) = parameters;
        var db = wrapper.Client.GetDatabase(wrapper._databaseId);
        var names = await db.ListCollectionNamesAsync(cancellationToken: cancellationToken).ConfigureAwait(true);
        return names != null;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual bool DeleteDatabase()
        => _executionStrategy.Execute(this, DeleteDatabaseOnce, null);

    private static bool DeleteDatabaseOnce(
        DbContext? context,
        MongoDBClientWrapper wrapper)
        => DeleteDatabaseOnceAsync(context, wrapper).GetAwaiter().GetResult();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual Task<bool> DeleteDatabaseAsync(
        CancellationToken cancellationToken = default)
        => _executionStrategy.ExecuteAsync(this, DeleteDatabaseOnceAsync, null, cancellationToken);

    private static async Task<bool> DeleteDatabaseOnceAsync(
        DbContext? _,
        MongoDBClientWrapper wrapper,
        CancellationToken cancellationToken = default)
    {
        await wrapper.Client.DropDatabaseAsync(wrapper._databaseId, cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual bool CreateContainerIfNotExists(ContainerProperties properties)
        => _executionStrategy.Execute((properties, this), CreateContainerIfNotExistsOnce, null);

    private static bool CreateContainerIfNotExistsOnce(
        DbContext context,
        (ContainerProperties Parameters, MongoDBClientWrapper Wrapper) parametersTuple)
        => CreateContainerIfNotExistsOnceAsync(context, parametersTuple).GetAwaiter().GetResult();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual Task<bool> CreateContainerIfNotExistsAsync(
        ContainerProperties properties,
        CancellationToken cancellationToken = default)
        => _executionStrategy.ExecuteAsync((properties, this), CreateContainerIfNotExistsOnceAsync, null, cancellationToken);

    private static async Task<bool> CreateContainerIfNotExistsOnceAsync(
        DbContext _,
        (ContainerProperties Parameters, MongoDBClientWrapper Wrapper) parametersTuple,
        CancellationToken cancellationToken = default)
    {
        var (parameters, wrapper) = parametersTuple;
       await wrapper.Client.GetDatabase(wrapper._databaseId).CreateCollectionAsync(parameters.Id, cancellationToken: cancellationToken).ConfigureAwait(false);

        return  true;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual bool CreateItem(
        string containerId,
        BsonDocument document,
        IUpdateEntry entry)
        => _executionStrategy.Execute((containerId, document, entry, this), CreateItemOnce, null);
  
  

    private static bool CreateItemOnce(
        DbContext context,
        (string ContainerId, BsonDocument Document, IUpdateEntry Entry, MongoDBClientWrapper Wrapper) parameters)
        => CreateItemOnceAsync(context, parameters).GetAwaiter().GetResult();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual Task<bool> CreateItemAsync(
        string containerId,
        BsonDocument document,
        IUpdateEntry updateEntry,
        CancellationToken cancellationToken = default)
        => _executionStrategy.ExecuteAsync((containerId, document, updateEntry, this), CreateItemOnceAsync, null, cancellationToken);


   

    private static async Task<bool> CreateItemOnceAsync(
        DbContext _,
        (string ContainerId, BsonDocument Document, IUpdateEntry Entry, MongoDBClientWrapper Wrapper) parameters,
        CancellationToken cancellationToken = default)
    {
        
        var entry = parameters.Entry;
        var wrapper = parameters.Wrapper;
        var container = wrapper.Client.GetDatabase(wrapper._databaseId).GetCollection<BsonDocument>(parameters.ContainerId);
        var itemRequestOptions = CreateItemRequestOptions(entry, wrapper._enableContentResponseOnWrite);
        var partitionKey = CreatePartitionKey(entry);
       await container.InsertOneAsync(parameters.Document,  new InsertOneOptions() {  BypassDocumentValidation=true}, cancellationToken)
            .ConfigureAwait(false);

        wrapper._commandLogger.ExecutedCreateItem(
             TimeSpan.Zero,
         0,
          "",
            parameters.Document["_id"].ToString(),
            parameters.ContainerId,
            partitionKey);

        return true;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual bool ReplaceItem(
        string collectionId,
        string documentId,
        BsonDocument document,
        IUpdateEntry entry)
        => _executionStrategy.Execute((collectionId, documentId, document, entry, this), ReplaceItemOnce, null);

    private static bool ReplaceItemOnce(
        DbContext context,
        (string ContainerId, string ItemId, BsonDocument Document, IUpdateEntry Entry, MongoDBClientWrapper Wrapper) parameters)
        => ReplaceItemOnceAsync(context, parameters).GetAwaiter().GetResult();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual Task<bool> ReplaceItemAsync(
        string collectionId,
        string documentId,
        BsonDocument document,
        IUpdateEntry updateEntry,
        CancellationToken cancellationToken = default)
        => _executionStrategy.ExecuteAsync(
            (collectionId, documentId, document, updateEntry, this), ReplaceItemOnceAsync, null, cancellationToken);

    private static async Task<bool> ReplaceItemOnceAsync(
        DbContext _,
        (string ContainerId, string ResourceId, BsonDocument Document, IUpdateEntry Entry, MongoDBClientWrapper Wrapper) parameters,
        CancellationToken cancellationToken = default)
    {
        var entry = parameters.Entry;
        var wrapper = parameters.Wrapper;
        var resid = parameters.ResourceId;
        var collname = parameters.ContainerId;
        var container = wrapper.Client.GetDatabase(wrapper._databaseId).GetCollection<BsonDocument>(collname);
        var itemRequestOptions = CreateItemRequestOptions(entry, wrapper._enableContentResponseOnWrite);
      //  var partitionKey = CreatePartitionKey(entry);
            var response = await container.ReplaceOneAsync(bs =>   bs[itemRequestOptions.Key.Name] == resid, parameters.Document, cancellationToken: cancellationToken).ConfigureAwait(false);
        wrapper._commandLogger.ExecutedReplaceItem(
            TimeSpan.Zero,
            1,
            "",
            parameters.ResourceId,
            parameters.ContainerId,
            "");


        return response.IsModifiedCountAvailable && response.ModifiedCount>0;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual bool DeleteItem(
        string containerId,
        string documentId,
        IUpdateEntry entry)
        => _executionStrategy.Execute(new MongoParameter(containerId, documentId, entry, this), DeleteItemOnce, null);

    private static bool DeleteItemOnce(
        DbContext context,
        MongoParameter parameters)
        => DeleteItemOnceAsync(context, parameters).GetAwaiter().GetResult();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual Task<bool> DeleteItemAsync(
        string containerId,
        string documentId,
        IUpdateEntry entry,
        CancellationToken cancellationToken = default)
        => _executionStrategy.ExecuteAsync(new MongoParameter(containerId, documentId, entry, this), DeleteItemOnceAsync, null, cancellationToken);

    private static async Task<bool> DeleteItemOnceAsync(
        DbContext? _,
        MongoParameter parameters,
        CancellationToken cancellationToken = default)
    {
        var entry = parameters.Entry;
        var wrapper = parameters.Wrapper;
        var items = wrapper.Client.GetDatabase(wrapper._databaseId).GetCollection<BsonDocument>( parameters.ColletionName);

        var itemRequestOptions = CreateItemRequestOptions(entry, wrapper._enableContentResponseOnWrite);
        
      
        var response = await items.DeleteOneAsync(bd => bd[itemRequestOptions.Key.Name] == itemRequestOptions.Id, cancellationToken).ConfigureAwait(false);
             

        wrapper._commandLogger.ExecutedDeleteItem(
            TimeSpan.Zero,
           response.DeletedCount,
           "" ,
            parameters.TableName,
            parameters.ColletionName,
            "");


        return response.DeletedCount==1 ;
    }

    private static ItemRequestOptions? CreateItemRequestOptions(IUpdateEntry entry, bool? enableContentResponseOnWrite)
    {
        var idProperty = entry.EntityType.FindPrimaryKey().Properties.FirstOrDefault();
        if (idProperty == null)
        {
            return null;
        }

        var id = entry.GetOriginalValue(idProperty);
        var converter = idProperty.GetTypeMapping().Converter;
        var vgf = idProperty.GetValueGeneratorFactory();
        
        if (converter != null)
        {
            id = converter.ConvertToProvider(id);
        }

        bool enabledContentResponse;
        if (enableContentResponseOnWrite.HasValue)
        {
            enabledContentResponse = enableContentResponseOnWrite.Value;
        }
        else
        {
            switch (entry.EntityState)
            {
                case EntityState.Modified:
                {
                    var jObjectProperty = entry.EntityType.FindProperty(StoreKeyConvention.JObjectPropertyName);
                    enabledContentResponse = (jObjectProperty?.ValueGenerated & ValueGenerated.OnUpdate) == ValueGenerated.OnUpdate;
                    break;
                }
                case EntityState.Added:
                {
                    var jObjectProperty = entry.EntityType.FindProperty(StoreKeyConvention.JObjectPropertyName);
                    enabledContentResponse = (jObjectProperty?.ValueGenerated & ValueGenerated.OnAdd) == ValueGenerated.OnAdd;
                    break;
                }
                default:
                    enabledContentResponse = false;
                    break;
            }
        }
        return new ItemRequestOptions { Key = idProperty , Id =  (ObjectId) id, EnableContentResponseOnWrite = enabledContentResponse };
    }

    private static string? CreatePartitionKey(IUpdateEntry entry)
    {
        object? partitionKey = null;
        var partitionKeyPropertyName = entry.EntityType.GetPartitionKeyPropertyName();
        if (partitionKeyPropertyName != null)
        {
            var partitionKeyProperty = entry.EntityType.FindProperty(partitionKeyPropertyName)!;
            partitionKey = entry.GetCurrentValue(partitionKeyProperty);

            var converter = partitionKeyProperty.GetTypeMapping().Converter;
            if (converter != null)
            {
                partitionKey = converter.ConvertToProvider(partitionKey);
            }
        }

        return (string?)partitionKey;
    }

  

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual IEnumerable<BsonDocument> ExecuteSqlQuery(
        string containerId,
        string? partitionKey,
        MongoDBSqlQuery query)
    {
        _commandLogger.ExecutingSqlQuery(containerId, partitionKey, query);

        return new DocumentEnumerable(this, containerId, partitionKey, query);
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual IAsyncEnumerable<BsonDocument> ExecuteSqlQueryAsync(
        string containerId,
        string? partitionKey,
        MongoDBSqlQuery query)
    {
        _commandLogger.ExecutingSqlQuery(containerId, partitionKey, query);

        return new DocumentAsyncEnumerable(this, containerId, partitionKey, query);
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual BsonDocument? ExecuteReadItem(
        string containerId,
        string? partitionKey,
        string resourceId)
    {
        _commandLogger.ExecutingReadItem(containerId, partitionKey, resourceId);

        var response = _executionStrategy.Execute((containerId, partitionKey, resourceId, this), CreateSingleItemQuery, null);

        _commandLogger.ExecutedReadItem(
             TimeSpan.Zero,
            0,
            response.RequestId.ToString(),
            resourceId,
            containerId,
            partitionKey);

        return JObjectFromReadItemResponseMessage(response);
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual async Task<BsonDocument?> ExecuteReadItemAsync(
        string containerId,
        string? partitionKey,
        string resourceId,
        CancellationToken cancellationToken = default)
    {
        _commandLogger.ExecutingReadItem(containerId, partitionKey, resourceId);

        var response = await _executionStrategy.ExecuteAsync(
                (containerId, partitionKey, resourceId, this),
                CreateSingleItemQueryAsync,
                null,
                cancellationToken)
            .ConfigureAwait(false);

        _commandLogger.ExecutedReadItem(
              TimeSpan.Zero,
            0,
            response.RequestId.ToString(),
            resourceId,
            containerId,
            partitionKey);

        return JObjectFromReadItemResponseMessage(response);
    }

    private static ResponseMessage CreateSingleItemQuery(
        DbContext? context,
        (string ContainerId, string? PartitionKey, string ResourceId, MongoDBClientWrapper Wrapper) parameters)
        => CreateSingleItemQueryAsync(context, parameters).GetAwaiter().GetResult();

    private static Task<ResponseMessage> CreateSingleItemQueryAsync(
        DbContext? _,
        (string ContainerId, string? PartitionKey, string ResourceId, MongoDBClientWrapper Wrapper) parameters,
        CancellationToken cancellationToken = default)
    {
        var (containerId, partitionKey, resourceId, wrapper) = parameters;
        var container = wrapper.Client.GetDatabase(wrapper._databaseId).GetCollection<BsonDocument>(containerId);
        
        //var v = container.FindAsync<BsonDocument>(bs=>bs]).GetAwaiter().GetResult();
        // var rm = new ResponseMessage() { MessageType = MongoDBMessageType.Query }

       throw new NotImplementedException();
    }

    private static BsonDocument? JObjectFromReadItemResponseMessage(ResponseMessage responseMessage)
    {
        ;

        return new BsonDocument();
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual FeedIterator CreateQuery(
        string containerId,
        string? partitionKey,
        MongoDBSqlQuery query)
    {
        var container = Client.GetDatabase(_databaseId).GetCollection<BsonDocument>(containerId);
        var queryDefinition =  ( FilterDefinition<BsonDocument>)query.Query ;
       
        container.Find(queryDefinition);
        //queryDefinition = query.Parameters
        //    .Aggregate(
        //        queryDefinition,
        //        (current, parameter) => current.n(parameter.Name, parameter.Value));
        if (string.IsNullOrEmpty(partitionKey))
        {
            return (FeedIterator)container.Find(queryDefinition);
        }

    //    var queryRequestOptions = new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) };

        return (FeedIterator)container.Find(queryDefinition);
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static JsonTextReader CreateJsonReader(TextReader reader)
    //{
    //    var jsonReader =     BsonDocument.Parse(rea;

    //    while (jsonReader.Read())
    //    {
    //        if (jsonReader.TokenType == JsonToken.StartObject)
    //        {
    //            while (jsonReader.Read())
    //            {
    //                if (jsonReader.TokenType == JsonToken.StartArray)
    //                {
    //                    return jsonReader;
    //                }
    //            }
    //        }
    //    }

    //    return jsonReader;
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static bool TryReadJObject(bson jsonReader, [NotNullWhen(true)] out BsonDocument? jObject)
    //{
    //    jObject = null;

    //    while (jsonReader.Read())
    //    {
    //        if (jsonReader.TokenType == JsonToken.StartObject)
    //        {
    //            jObject = Serializer.Deserialize<BsonDocument>(jsonReader);
    //            return true;
    //        }
    //    }

    //    return false;
    //}    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static bool TryReadJObject(bson jsonReader, [NotNullWhen(true)] out BsonDocument? jObject)
    //{
    //    jObject = null;

    //    while (jsonReader.Read())
    //    {
    //        if (jsonReader.TokenType == JsonToken.StartObject)
    //        {
    //            jObject = Serializer.Deserialize<BsonDocument>(jsonReader);
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    private sealed class DocumentEnumerable : IEnumerable<BsonDocument>
    {
        private readonly MongoDBClientWrapper _MongoDBClient;
        private readonly string _containerId;
        private readonly string? _partitionKey;
        private readonly MongoDBSqlQuery _MongoDBSqlQuery;

        public DocumentEnumerable(
            MongoDBClientWrapper MongoDBClient,
            string containerId,
            string? partitionKey,
            MongoDBSqlQuery MongoDBSqlQuery)
        {
            _MongoDBClient = MongoDBClient;
            _containerId = containerId;
            _partitionKey = partitionKey;
            _MongoDBSqlQuery = MongoDBSqlQuery;
        }

        public IEnumerator<BsonDocument> GetEnumerator()
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private sealed class Enumerator : IEnumerator<BsonDocument>
        {
            private readonly MongoDBClientWrapper _MongoDBClientWrapper;
            private readonly string _containerId;
            private readonly string? _partitionKey;
            private readonly MongoDBSqlQuery _MongoDBSqlQuery;

            private BsonDocument? _current;
            private ResponseMessage? _responseMessage;
            private Stream? _responseStream;
            private StreamReader? _reader;

            private FeedIterator? _query;

            public Enumerator(DocumentEnumerable documentEnumerable)
            {
                _MongoDBClientWrapper = documentEnumerable._MongoDBClient;
                _containerId = documentEnumerable._containerId;
                _partitionKey = documentEnumerable._partitionKey;
                _MongoDBSqlQuery = documentEnumerable._MongoDBSqlQuery;
            }

            public BsonDocument Current
                => _current ?? throw new InvalidOperationException();

            object IEnumerator.Current
                => Current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                //if (_jsonReader == null)
                //{
                //    _query ??= _MongoDBClientWrapper.CreateQuery(_containerId, _partitionKey, _MongoDBSqlQuery);

                //    if (!_query.Any())
                //    {
                //        _current = null;
                //        return false;
                //    }

                //    //_responseMessage = _query.ReadNextAsync().GetAwaiter().GetResult();

                //    //_MongoDBClientWrapper._commandLogger.ExecutedReadNext(
                //    //    _responseMessage.Diagnostics.GetClientElapsedTime(),
                //    //    _responseMessage.Headers.RequestCharge,
                //    //    _responseMessage.Headers.ActivityId,
                //    //    _containerId,
                //    //    _partitionKey,
                //    //    _MongoDBSqlQuery);

                //    //_responseMessage.EnsureSuccessStatusCode();

                //    //_responseStream = _responseMessage.Content;
                //    _reader = new StreamReader(_responseStream);
                //    _jsonReader = CreateJsonReader(_reader);
                //}

                //if (TryReadJObject(_jsonReader, out var jObject))
                //{
                //    _current = jObject;
                //    return true;
                //}

                //ResetRead();

                return MoveNext();
            }

            private void ResetRead()
            {
                _reader?.Dispose();
                _reader = null;
                _responseStream?.Dispose();
                _responseStream = null;
            }

            public void Dispose()
            {
                ResetRead();

               // _responseMessage?.Dispose();
                _responseMessage = null;
            }

            public void Reset()
                => throw new NotSupportedException(CoreStrings.EnumerableResetNotSupported);
        }
    }

    private sealed class DocumentAsyncEnumerable : IAsyncEnumerable<BsonDocument>
    {
        private readonly MongoDBClientWrapper _MongoDBClient;
        private readonly string _containerId;
        private readonly string? _partitionKey;
        private readonly MongoDBSqlQuery _MongoDBSqlQuery;

        public DocumentAsyncEnumerable(
            MongoDBClientWrapper MongoDBClient,
            string containerId,
            string? partitionKey,
            MongoDBSqlQuery MongoDBSqlQuery)
        {
            _MongoDBClient = MongoDBClient;
            _containerId = containerId;
            _partitionKey = partitionKey;
            _MongoDBSqlQuery = MongoDBSqlQuery;
        }

        public IAsyncEnumerator<BsonDocument> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new AsyncEnumerator(this, cancellationToken);

        private sealed class AsyncEnumerator : IAsyncEnumerator<BsonDocument>
        {
            private readonly MongoDBClientWrapper _MongoDBClientWrapper;
            private readonly string _containerId;
            private readonly string? _partitionKey;
            private readonly MongoDBSqlQuery _MongoDBSqlQuery;
            private readonly CancellationToken _cancellationToken;

            private BsonDocument? _current;
            private ResponseMessage? _responseMessage;
            private Stream? _responseStream;
            private StreamReader? _reader;

            private FeedIterator? _query;

            public BsonDocument Current
                => _current ?? throw new InvalidOperationException();

            public AsyncEnumerator(DocumentAsyncEnumerable documentEnumerable, CancellationToken cancellationToken)
            {
                _MongoDBClientWrapper = documentEnumerable._MongoDBClient;
                _containerId = documentEnumerable._containerId;
                _partitionKey = documentEnumerable._partitionKey;
                _MongoDBSqlQuery = documentEnumerable._MongoDBSqlQuery;
                _cancellationToken = cancellationToken;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public async ValueTask<bool> MoveNextAsync()
            {
                _cancellationToken.ThrowIfCancellationRequested();

                //if (_jsonReader == null)
                //{
                //    _query ??= _MongoDBClientWrapper.CreateQuery(_containerId, _partitionKey, _MongoDBSqlQuery);

                //    //if (!_query.HasMoreResults)
                //    //{
                //    //    _current = null;
                //    //    return false;
                //    //}

                //    //_responseMessage = await _query.ReadNextAsync(_cancellationToken).ConfigureAwait(false);

                //    //_MongoDBClientWrapper._commandLogger.ExecutedReadNext(
                //    //    _responseMessage.Diagnostics.GetClientElapsedTime(),
                //    //    _responseMessage.Headers.RequestCharge,
                //    //    _responseMessage.Headers.ActivityId,
                //    //    _containerId,
                //    //    _partitionKey,
                //    //    _MongoDBSqlQuery);

                //    //_responseMessage.EnsureSuccessStatusCode();

                //    //_responseStream = _responseMessage.Content;
                //    _reader = new StreamReader(_responseStream);
                //    _jsonReader = CreateJsonReader(_reader);
                //}

                //if (TryReadJObject(_jsonReader, out var jObject))
                //{
                //    _current = jObject;
                //    return true;
                //}

                await ResetReadAsync().ConfigureAwait(false);

                return await MoveNextAsync().ConfigureAwait(false);
            }

            private async Task ResetReadAsync()
            {
                await _reader.DisposeAsyncIfAvailable().ConfigureAwait(false);
                _reader = null;
                await _responseStream.DisposeAsyncIfAvailable().ConfigureAwait(false);
                _responseStream = null;
            }

            public async ValueTask DisposeAsync()
            {
                await ResetReadAsync().ConfigureAwait(false);

               // await _responseMessage.DisposeAsyncIfAvailable().ConfigureAwait(false);
                _responseMessage = null;
            }
        }
    }
}

internal record struct MongoParameter
{
   public  string ColletionName;
    public string TableName;
    public IUpdateEntry Entry;
    public MongoDBClientWrapper Wrapper;
     
    public MongoParameter(string colletionName, string tableName, IUpdateEntry entry, MongoDBClientWrapper mongoDBClientWrapper) : this()
    {
        Entry = entry;
        ColletionName = colletionName;
        TableName = tableName;
        Wrapper= mongoDBClientWrapper;
    }
}
