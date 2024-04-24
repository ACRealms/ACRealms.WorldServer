using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Tests.Helpers
{
    public static class Paths
    {
        public static string SolutionPath { get; } = new Func<string>(() =>
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (!directory.GetFiles("*.sln").Any())
                directory = directory.Parent;
            return directory.FullName;
        })();

        public static string LocalDataPath { get; } =
            Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "tests_data");
    }
}
