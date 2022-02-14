// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;

namespace IoTSharp.EntityFrameworkCore.MongoDB
{
    public class MongoDBException : Exception
    {
        public HttpStatusCode StatusCode { get; internal set; }
        public TimeSpan? RetryAfter { get; internal set; }
    }
}
