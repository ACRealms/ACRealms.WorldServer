using ACE.Entity;
using ACE.Server.Command.Handlers.Processors;
using ACE.Server.Realms.Peripherals.DungeonSets;
using Newtonsoft.Json;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ACE.Server.Realms.Peripherals
{
    internal class DungeonSetsPeripheral : Peripheral<DungeonSetsPeripheral>
    {
        public override string Name => "dungeon-sets";

        private FrozenDictionary<string, DungeonSetOptions> OptionsForSet { get; init; }
        private FrozenDictionary<string, FrozenSet<ushort>> LandblocksForSet { get; init; }
        private FrozenDictionary<ushort, FrozenSet<string>> SetsForLandblock { get; init; }

        public DungeonSetsPeripheral()
        {
            OptionsForSet = FrozenDictionary<string, DungeonSetOptions>.Empty;
            LandblocksForSet = FrozenDictionary<string, FrozenSet<ushort>>.Empty;
            SetsForLandblock = FrozenDictionary<ushort, FrozenSet<string>>.Empty;
        }

        private DungeonSetsPeripheral(DungeonSetsConfigV1 config)
        {
            var optionsMap = new Dictionary<string, DungeonSetOptions>();
            var landblocksForSet = new Dictionary<string, FrozenSet<ushort>>();
            var setsForLandblock = new Dictionary<ushort, List<string>>();

            foreach (var set in config.dungeon_sets)
            {
                if (optionsMap.ContainsKey(set.name))
                    throw new ConfigurationErrorsException($"Duplicate name '{set.name}' found in dungeon-sets configuration.");
                var opts = new DungeonSetOptions(set);

                optionsMap.Add(set.name, opts);
                set.landblocks = null;

                landblocksForSet.Add(opts.Name, opts.Landblocks);

                foreach (var lb in opts.Landblocks)
                {
                    if (!setsForLandblock.ContainsKey(lb))
                        setsForLandblock.Add(lb, new List<string>());
                    setsForLandblock[lb].Add(opts.Name);
                }
            }

            OptionsForSet = optionsMap.ToFrozenDictionary();
            LandblocksForSet = landblocksForSet.ToFrozenDictionary();

            // Reduce excessive memory by deduplicating lists
            var setsForLandblockDeduplicated = new List<List<string>>();
            foreach (var list in setsForLandblock.Values)
            {
                if (!setsForLandblockDeduplicated.Any(l => l.SequenceEqual(list)))
                    setsForLandblockDeduplicated.Add(list);
            }

            var setsForLandblockDeduplicatedFrozen = setsForLandblockDeduplicated.Select(x => x.ToFrozenSet()).ToList();

            SetsForLandblock = setsForLandblock.ToFrozenDictionary(x => x.Key, kvp =>
            {
                return setsForLandblockDeduplicatedFrozen.First(canonicalSet => kvp.Value.SequenceEqual(canonicalSet));
            });
        }

        public static DungeonSetsPeripheral Load()
        {
            // TODO: Allow for server admin to decide via configuration if server should fail to start if peripheral config is invalid
            try
            {
                DateTime now = DateTime.Now;
                DirectoryInfo di = DeveloperContentCommands.VerifyContentFolder(null);
                if (!di.Exists) throw new DirectoryNotFoundException($"Content folder not found at {di.FullName}");

                var sep = Path.DirectorySeparatorChar;

                var dungeonSetsConfig = $"{di.FullName}{sep}json{sep}peripherals{sep}dungeon-sets{sep}dungeon-sets.jsonc";
                if (!File.Exists(dungeonSetsConfig))
                    throw new FileNotFoundException($"Configuration file not found: {di.FullName}");

                var config = JsonConvert.DeserializeObject<DungeonSetsConfigV1>(File.ReadAllText(dungeonSetsConfig));
                var result = new DungeonSetsPeripheral(config);
                log.Info($"Loaded {result.SetsForLandblock.Count} dungeon sets");
                return result;
            }
            catch (Exception e)
            {
                return ReportLoadErrorAndDefault(e);
            }
        }

        public bool IncludedInSet(LandblockId landblock, string dungeonSetName) => IncludedInSet(landblock.Landblock, dungeonSetName);
        public bool IncludedInSet(UsablePosition position, string dungeonSetName) => IncludedInSet(position.LandblockId.Landblock, dungeonSetName);


        public bool IncludedInSet(ushort landblock, string dungeonSetName) 
        {
            if (!SetsForLandblock.TryGetValue(landblock, out var names))
                return false;
            return names.Contains(dungeonSetName);
        }
    }
}
