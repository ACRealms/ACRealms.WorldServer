using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Realms.Peripherals.DungeonSets
{
#pragma warning disable IDE1006 // Naming Styles
    public class DungeonSetConfigV1
    {
        public string name { get; set; }
        public string[] landblocks { get; set; }
    }

    public class DungeonSetsConfigV1
    {
        public List<DungeonSetConfigV1> dungeon_sets { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
}
