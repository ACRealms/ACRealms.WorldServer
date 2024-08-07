using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("ACRealms.Tests.Benchmarks")]

public static class AssemblyInfo
{
    public static string GitHash { get; } = new Func<string>(() =>
    {
        var asm = typeof(AssemblyInfo).Assembly;
        var attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
        return attrs.FirstOrDefault(a => a.Key == "GitHash")?.Value;
    })();
}
