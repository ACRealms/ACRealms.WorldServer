using ACE.Database;
using ACE.Server.Command.Handlers;
using ACE.Server.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
            coverage,
            simple_random
        }

        public class RealmFixtureNotFoundException : Exception
        {
            public RealmFixtureNotFoundException(string message) : base(message) { }
        }

        public static FixtureName? CurrentlyLoadedFixture { get; private set; }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void LoadRealmFixture(FixtureName name, bool clearDbRealmsFirst = true, bool forceReload = false)
        {
            if (!forceReload && CurrentlyLoadedFixture == name) return;

            var fixturePath = $"{FixtureRoot}/{name}";
            if (!Directory.Exists(fixturePath))
                throw new RealmFixtureNotFoundException($"Fixture {name} not found.");

            var realmsJsoncPath = $"{fixturePath}/realms.jsonc";

            var realms = ACE.Server.Command.Handlers.RealmDataHelpers.ImportJsonRealmsFolder(null, fixturePath);
            if (realms != null)
            {
                if (clearDbRealmsFirst)
                {
                    if (File.Exists(realmsJsoncPath))
                        File.Delete(realmsJsoncPath);
                    RealmManager.ClearCache();
                    DatabaseManager.World.ReplaceAllRealms(new Dictionary<ushort, ACE.Database.Adapter.RealmToImport>());
                    RealmManager.ClearCache();
                }
                ACE.Server.Command.Handlers.RealmDataHelpers.ImportJsonRealmsIndex(null, realmsJsoncPath, realms);
            }
            else
                throw new InvalidDataException("Realm fixture import error");
            CurrentlyLoadedFixture = name;
        }
    }
}
