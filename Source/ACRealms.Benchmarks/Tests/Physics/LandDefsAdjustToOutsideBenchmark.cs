using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ACE.Server.Physics.Common;

namespace ACRealms.Tests.Benchmarks.Tests.Physics
{
    [MemoryDiagnoser]
    public class LandDefsAdjustToOutsideBenchmark
    {
        [Benchmark]
        public bool AdjustToOutside()
        {
            uint arg1 = 0x0105003fu;
            Vector3 arg2 = new Vector3(174.79199f, 149.61775f, 101.63402f);
            return LandDefs.AdjustToOutside(ref arg1, ref arg2);
        }
    }
}
