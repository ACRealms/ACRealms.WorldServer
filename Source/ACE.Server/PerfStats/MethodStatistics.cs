using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACE.Server.PerfStats
{
#if METHODSTATISTICS
    public static class MethodStatistics
    {

        public static readonly ThreadLocal<Dictionary<Type, Dictionary<string, ulong>>> MethodCounters = new(() => [], true);

        public static void Increment(Type klass, string method)
        {
            var dict = MethodCounters.Value;
            if (!dict.ContainsKey(klass))
                dict.Add(klass, []);
            var methods = dict[klass];
            if (!methods.TryAdd(method, 1))
                methods[method] += 1;
        }

        public static string Dump()
        {
            var sb = new StringBuilder();
            Dictionary<string, ulong> values = [];
            foreach (var threadVal in MethodCounters.Values)
            {
                foreach (var kvp in threadVal)
                {
                    foreach (var val in kvp.Value)
                    {
                        var key = $"{kvp.Key.Name}.{val.Key}";
                        if (!values.TryAdd(key, val.Value))
                            values[key] += val.Value;
                    }
                }
            }
            foreach (var val in values.OrderByDescending(x => x.Value))
                sb.AppendLine($"{val.Key}: {val.Value}");

            return sb.ToString();
        }
    }
#endif
}
