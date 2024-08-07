namespace ACRealms.Tests.Benchmarks.Tests.Physics
{
    [MemoryDiagnoser]
    [SimpleJob(launchCount: 1, warmupCount: 10, iterationCount: 20, invocationCount: 1)]
    public class PhysicsLandblockLoadDifferentInstanceBenchmark : LandblockBenchmark
    {
        protected override bool IncrementInstanceID => true;
        protected override bool LandblockLoadDuringSetup => false;

        [Benchmark]
        public void LandblockLoad() => GetLandblock();
    }
}
