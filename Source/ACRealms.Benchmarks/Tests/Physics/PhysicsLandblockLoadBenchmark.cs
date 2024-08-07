namespace ACRealms.Tests.Benchmarks.Tests.Physics
{
    [MemoryDiagnoser]
    [SimpleJob(launchCount: 1, warmupCount: 10, iterationCount: 20, invocationCount: 1)]
    public class PhysicsLandblockLoadBenchmark : LandblockBenchmark
    {
        //[Params(true, false)]
        public bool SameIID { get; set; } = false;
        protected override bool IncrementInstanceID => !SameIID;
        protected override bool LandblockLoadDuringSetup => false;

        [Benchmark]
        public void LandblockLoad() => GetLandblock();
    }
}
