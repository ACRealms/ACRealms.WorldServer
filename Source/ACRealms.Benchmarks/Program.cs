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

        static void RunForProfiler()
        {
            var b = new UpdateObjectServerNewBenchmark();
            b.Setup();
            b.IterationSetup();
            while(true)
            {
                b.Creature!.PhysicsObj.update_object_server_new(b.Landblock.Instance);
            }
        }

        static void RunBenchmarks()
        {
            //RunForProfiler();
            BenchmarkRunner.Run<LandDefsAdjustToOutsideBenchmark>();
        }
    }
}
