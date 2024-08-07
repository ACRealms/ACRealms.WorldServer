using ACRealms.Tests.Benchmarks.Tests.Physics;
using BenchmarkDotNet.Running;

namespace ACRealms.Tests.Benchmarks
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
#if DEBUG
            DebugBenchmarks();
            return;
#else
            RunBenchmarks();
#endif
        }
        static void DebugBenchmarks()
        {
            var b = new PhysicsLandblockSmiteBenchmark();
            b.Setup();
            b.IterationSetup();
            b.Smite();
            b.IterationCleanup();
            b.Teardown();
        }

        static void RunBenchmarks()
        {
            BenchmarkRunner.Run<PhysicsLandblockSmiteBenchmark>();
            BenchmarkRunner.Run<PhysicsLandblockLoadBenchmark>();
        }
    }
}
