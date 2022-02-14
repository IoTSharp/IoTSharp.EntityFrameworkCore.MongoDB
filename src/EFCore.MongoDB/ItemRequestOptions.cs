// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson;

namespace IoTSharp.EntityFrameworkCore.MongoDB
{
    internal class ItemRequestOptions
    {
        public  BsonValue? IfMatchEtag { get; set; }
        public bool EnableContentResponseOnWrite { get; set; }
    }
}
