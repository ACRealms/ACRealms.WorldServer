using ACE.Server.Entity;
using ACE.Server.Managers;

namespace ACRealms.Tests.Benchmarks.Tests
{
    public abstract class LandblockBenchmark : ACRBenchmark
    {
        protected uint CurrentInstance;
        protected Landblock? Landblock;

        protected virtual bool LandblockLoadDuringSetup => true;
        protected virtual bool IncrementInstanceID => true;
        protected virtual ushort LandblockIdToLoad => 0x002B; //Egg Orchard
        public override void Setup()
        {
            base.Setup();
            CurrentInstance = ACE.Entity.Position.InstanceIDFromVars((ushort)ACE.Entity.Enum.Properties.ReservedRealm.@default, 0, false);
        }

        [IterationSetup]
        public virtual void IterationSetup()
        {
            WorldManager.PendingPause = true;
            while (!WorldManager.Paused)
                Thread.Sleep(10);
            if (LandblockLoadDuringSetup)
                GetLandblock(LandblockIdToLoad);
        }

        [IterationCleanup]
        public virtual void IterationCleanup()
        {
            var id = Landblock!.Id;
            var iid = Landblock.Instance;
            LandblockManager.AddToDestructionQueue(Landblock);
            WorldManager.Paused = false;
            while (LandblockManager.IsLoaded(id, iid))
            {
                Thread.Sleep(10);
                if (WorldManager.Paused)
                    throw new InvalidOperationException("Unexpected pause");
            }
            Landblock = null;
        }

        protected void GetLandblock() => GetLandblock(LandblockIdToLoad);
        protected void GetLandblock(ushort landblockId)
        {
            if (Landblock != null)
                throw new InvalidOperationException("Landblock is already loaded.");

            var id = new ACE.Entity.LandblockId(landblockId);
            if (LandblockManager.IsLoaded(id, CurrentInstance))
                throw new InvalidOperationException("Landblock is already loaded.");

            Landblock = LandblockManager.GetLandblock(id, CurrentInstance, null, false, false, true);
            if (IncrementInstanceID)
                CurrentInstance++;

            // Allow generators to spawn
            LandblockManager.Tick(Timers.PortalYearTicks);
        }
    }
}
