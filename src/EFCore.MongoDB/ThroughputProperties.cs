// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace IoTSharp.EntityFrameworkCore.MongoDB
{
    public class ThroughputProperties
    {
        public int? AutoscaleMaxThroughput { get; internal set; }
        public int? Throughput { get; internal set; }

        internal static object? CreateAutoscaleThroughput(int value)
        {
            throw new NotImplementedException();
        }

        internal static object? CreateManualThroughput(int value)
        {
            throw new NotImplementedException();
        }
    }
}
