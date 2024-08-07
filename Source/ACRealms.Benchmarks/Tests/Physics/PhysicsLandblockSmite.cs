using ACE.Server.WorldObjects;

namespace ACRealms.Tests.Benchmarks.Tests.Physics
{
    [MemoryDiagnoser]
    [SimpleJob(launchCount: 1, warmupCount: 10, iterationCount: 20, invocationCount: 1)]
    public class PhysicsLandblockSmiteBenchmark : LandblockBenchmark
    {
        private Creature? Smiter;
        private List<Creature>? SmiteeList;

        [Benchmark]
        public void Smite()
        {
            foreach (var smitee in SmiteeList!)
                smitee.Smite(Smiter);
        }

        public override void IterationSetup()
        {
            base.IterationSetup();

            var c = Landblock!.GetAllCreatures();
            Smiter = c.First();
            SmiteeList = c.Skip(1).ToList();
        }
    }
}
