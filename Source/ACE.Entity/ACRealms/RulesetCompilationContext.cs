using ACE.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Entity.ACRealms
{
    public record RulesetCompilationContext
    {
        private static Random RootRandom = Random.Shared;
        private static int GetNewRootSeed() => RootRandom.Next();
        private static bool DefaultSharedWasResetOnce = false;
        public static RulesetCompilationContext DefaultShared { get; private set; } = new();

        private DateTime? TimeContext { get; init; }
        public Random Randomizer { get; private init; } = Random.Shared; //RandomSeed.HasValue ? new Random(RandomSeed.Value) : Random.Shared;
        public PropertyOperators Operators { get; private init; } = new PropertyOperators(Random.Shared);
        public bool Trace { get; private init; } = false;
        public bool DeriveNewSeedEachPhase { get; private init; } = false;
        public int? RandomSeed { get; private init; } = null;
        public string GitHash { get; init; } = null;
        private IList<string> LogOutput { get; init; } = null;


        public RulesetCompilationContext WithTimeContext(DateTime time) => this with { TimeContext = time };
        public RulesetCompilationContext WithDerivedNewSeed() => (DeriveNewSeedEachPhase && RandomSeed.HasValue) ? WithNewSeed(Randomizer.Next()) : this;
        public RulesetCompilationContext WithNewSeed(int seed) => (this with { RandomSeed = seed, Randomizer = new Random(seed) }).AfterSeedUpdated();
        private RulesetCompilationContext AfterSeedUpdated() => this with { Operators = new PropertyOperators(Randomizer) };
        public RulesetCompilationContext WithTrace(bool deriveNewSeedEachPhase, bool withNewTraceLog = false) =>
            (Trace && !withNewTraceLog)
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
            {
                var ctx = new RulesetCompilationContext { GitHash = gitHash, DeriveNewSeedEachPhase = true };
                return ctx.WithNewSeed(randomSeed ?? GetNewRootSeed());
            }
            else return DefaultShared;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SetNewRuntimeDefaults(RulesetCompilationContext target)
        {
            if (DefaultSharedWasResetOnce) throw new InvalidOperationException("The default shared compilation context may only be reset once.");
            DefaultShared = target;
        }


        // I have not confirmed if this works as intended (defers the allocation of the actual string until after the check for Trace enabled)
        public void Log(Func<string> messageAllocatorIfTraceEnabled)
        {
            if (!Trace) return;
            LogDirect(messageAllocatorIfTraceEnabled());
        }

        public void LogDirect(string m) => LogOutput.Add(m);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void FlushLogToFile(string filename) => System.IO.File.WriteAllText(filename, FlushLog());

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string FlushLog()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($@"ACRealms Ruleset Compilation Log
Generated at: {TimeContext ?? DateTime.UtcNow} UTC
{(GitHash != null ? $"Commit Hash: {GitHash}" : "Commit hash unavailable")}

Context Configuration:
  DeriveNewSeedEachPhase: {DeriveNewSeedEachPhase}
  RandomSeed {(DeriveNewSeedEachPhase ? " (Initial)" : "")}: {(RandomSeed.HasValue ? RandomSeed.Value : "Untracked (acr_enable_ruleset_seeds disabled)")}
  

");
            foreach (var line in LogOutput)
                sb.AppendLine(line);
            var s = sb.ToString();
            ClearLog();
            return s;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ClearLog() => LogOutput.Clear();


        public interface IPropertyOperators { }
        public interface IPropertyOperators<T> : IPropertyOperators { }

        public interface IPropertyOperatorsRollable<T> : IPropertyOperators<T>
            where T : IEquatable<T>
        {
            public abstract T RollValue(T min, T max);
        }

        public interface IPropertyOperatorsMinMax<T> : IPropertyOperatorsRollable<T>
            where T : IMinMaxValue<T>, IEquatable<T>, IComparable<T>
        {
            public abstract T AddValue(T val1, T val2);
            public abstract T MultiplyValue(T val1, T val2);
        }

        public abstract class PropertyOperatorsBase : IPropertyOperators
        {
            public Random Randomizer { get; init; }
        }

        public class PropertyOperatorsFloat : PropertyOperatorsBase, IPropertyOperatorsMinMax<double>
        {
            public double RollValue(double min, double max) => Randomizer.NextDouble() * (max - min) + min;
            public double AddValue(double val1, double val2) => val1 + val2;
            public double MultiplyValue(double val1, double val2) => val1 * val2;
        }

        public class PropertyOperatorsInt : PropertyOperatorsBase, IPropertyOperatorsMinMax<int>
        {
            public int RollValue(int min, int max) => Randomizer.Next(min, max);
            public int AddValue(int val1, int val2) => val1 + val2;
            public int MultiplyValue(int val1, int val2) => val1 * val2;
        }

        public class PropertyOperatorsInt64 : PropertyOperatorsBase, IPropertyOperatorsMinMax<long>
        {
            public long RollValue(long min, long max) => Randomizer.NextInt64(min, max);
            public long AddValue(long val1, long val2) => val1 + val2;
            public long MultiplyValue(long val1, long val2) => val1 * val2;
        }

        public class PropertyOperators
        {
            public PropertyOperatorsInt64 Int64 { get; init; }
            public PropertyOperatorsInt Int { get; init; }
            public PropertyOperatorsFloat Float { get; init; }

            public PropertyOperators(Random randomizer)
            {
                Int = new PropertyOperatorsInt() { Randomizer = randomizer };
                Int64 = new PropertyOperatorsInt64() { Randomizer = randomizer };
                Float = new PropertyOperatorsFloat() { Randomizer = randomizer };
            }
        }
    }
}
