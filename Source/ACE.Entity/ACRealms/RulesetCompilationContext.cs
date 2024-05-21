using ACE.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Entity.ACRealms
{
    public record RulesetCompilationContext(
        bool Trace,
        int? RandomSeed,
        bool DeriveNewSeedEachPhase,
        string GitHash)
    {
        private static Random RootRandom = Random.Shared;
        private static int GetNewRootSeed() => RootRandom.Next();
        private static bool DefaultSharedWasResetOnce = false;
        public static RulesetCompilationContext DefaultShared { get; private set; } = new(
            Trace: false,
            RandomSeed: null,
            DeriveNewSeedEachPhase: false,
            GitHash: null
        );

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SetNewRuntimeDefaults(RulesetCompilationContext target)
        {
            if (DefaultSharedWasResetOnce) throw new InvalidOperationException("The default shared compilation context may only be reset once.");
            DefaultShared = target;
        }

        public Random Randomizer { get; private init; } = RandomSeed.HasValue ? new Random(RandomSeed.Value) : Random.Shared;
        private IList<string> LogOutput { get; init; } = Trace ? new List<string>() : null;

        public RulesetCompilationContext WithDerivedNewSeed(bool onlyIfConfigured = true) => ((DeriveNewSeedEachPhase && RandomSeed.HasValue) || !onlyIfConfigured) ? WithDerivedNewSeed(Randomizer.Next()) : this;
        public RulesetCompilationContext WithNewSeed() => WithDerivedNewSeed(onlyIfConfigured: false);
        private RulesetCompilationContext WithDerivedNewSeed(int seed) => this with { RandomSeed = seed, Randomizer = new Random(seed) };
        public RulesetCompilationContext WithTrace(bool deriveNewSeedEachPhase, bool alwaysReset = false) =>
            (Trace || alwaysReset)
            ? this
            : this with {
                Trace = true,
                LogOutput = new List<string>(),
                DeriveNewSeedEachPhase = deriveNewSeedEachPhase,
            };

        public RulesetCompilationContext Merge(RulesetCompilationContext priorContext) => priorContext with {
            Trace = Trace || priorContext.Trace,
            LogOutput = LogOutput ?? priorContext.LogOutput ?? null
        };

        public static RulesetCompilationContext CreateContext(
# if DEBUG
            int _ = 0, // Force named argument
# endif
            bool enableSeedTracking = false,
            int? randomSeed = null,
            string gitHash = null
        ) 
        {
            if (enableSeedTracking || randomSeed.HasValue)
                return new(
                    Trace: false,
                    RandomSeed: randomSeed.HasValue ? randomSeed : GetNewRootSeed(),
                    DeriveNewSeedEachPhase: true,
                    GitHash: gitHash
                );
            else return DefaultShared;
        }

        // I have not confirmed if this works as intended (defers the allocation of the actual string until after the check for Trace enabled)
        public void Log(Func<string> messageAllocatorIfTraceEnabled)
        {
            if (!Trace) return;
            LogDirect(messageAllocatorIfTraceEnabled());
        }

        public void LogDirect(string m) => LogOutput.Add(m);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void FlushLogToFile(string filename)  
        {
            System.IO.File.WriteAllText(filename, $@"ACRealms Ruleset Compilation Log
Generated at: {DateTime.UtcNow} UTC
{(GitHash != null ? $"Commit Hash: {GitHash}" : "Commit hash unavailable")}

Context Configuration:
  DeriveNewSeedEachPhase: {DeriveNewSeedEachPhase}
  RandomSeed (Placeholder, not implemented in randomization yet){(DeriveNewSeedEachPhase ? " (Initial)" : "")}: {(RandomSeed.HasValue ? RandomSeed.Value : "Untracked (acr_enable_ruleset_seeds disabled)")}
  

");
            System.IO.File.AppendAllLines(filename, LogOutput);
            LogOutput.Clear();
        }
    }
}
