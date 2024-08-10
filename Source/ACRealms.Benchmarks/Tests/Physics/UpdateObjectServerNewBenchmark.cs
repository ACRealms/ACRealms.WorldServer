using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ACE.Server.Physics.Common;
using ACE.Server.WorldObjects;

namespace ACRealms.Tests.Benchmarks.Tests.Physics
{
    public class UpdateObjectServerNewBenchmark : LandblockBenchmark
    {
        public Creature? Creature;

        public override void IterationSetup()
        {
            base.IterationSetup();
            var c = Landblock!.GetAllCreatures();
            Creature = c.First();
        }

        [Benchmark]
        public void UpdateObjectServerNew()
        {
            Creature!.PhysicsObj.update_object_server_new(Landblock!.Instance);
        }
    }
}
