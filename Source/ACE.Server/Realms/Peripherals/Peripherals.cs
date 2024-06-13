using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Realms.Peripherals
{
    internal class Peripherals
    {
        // private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal DungeonSetsPeripheral DungeonSets { get; }

        private Peripherals()
        {
            DungeonSets = DungeonSetsPeripheral.Load();
        }

        internal static Peripherals Load() => new Peripherals();
    }
}
