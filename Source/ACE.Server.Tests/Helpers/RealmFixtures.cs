using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Tests.Helpers
{
    public static class RealmFixtures
    {
        static string FixtureRoot { get; } = $"{Paths.SolutionPath}/ACE.Server.Tests/Helpers/realm-fixtures";

        public enum FixtureName
        {
            simple,
        }

        public class RealmFixtureNotFoundException : Exception
        {
            public RealmFixtureNotFoundException(string message) : base(message) { }
        }

        public static void LoadRealmFixture(FixtureName name)
        {
            var fixturePath = $"{FixtureRoot}/{name}";
            if (!Directory.Exists(fixturePath))
                throw new RealmFixtureNotFoundException($"Fixture {name} not found.");

            var realmsJsoncPath = $"{fixturePath}/realms.jsonc";
            if (!File.Exists(realmsJsoncPath))
                throw new RealmFixtureNotFoundException($"Fixture {name} does not contain a realms.jsonc.");

            var realms = ACE.Server.Command.Handlers.Processors.DeveloperContentCommands.ImportJsonRealmsFolder(null, fixturePath);
            if (realms != null)
                ACE.Server.Command.Handlers.Processors.DeveloperContentCommands.ImportJsonRealmsIndex(null, realmsJsoncPath, realms);
        }
    }
}
