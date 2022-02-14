// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace IoTSharp.EntityFrameworkCore.MongoDB.Storage.Internal
{
    public class MongoDBSerializer
    {
        public virtual  T FromStream<T>(Stream stream)
        {
            return default(T);  
        }
        
        /// <inheritdoc />
        public virtual  Stream ToStream<T>(T input)
        {
            return new Stream(input);
        }
    }
}
