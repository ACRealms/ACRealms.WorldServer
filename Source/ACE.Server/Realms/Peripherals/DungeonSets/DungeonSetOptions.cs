using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Realms.Peripherals.DungeonSets
{
    internal class DungeonSetOptions
    {
        internal string Name { get; private init; }
        internal FrozenSet<ushort> Landblocks { get; private init; }

        internal DungeonSetOptions(DungeonSetConfigV1 config)
        {
            Name = config.name;
            Landblocks = config.landblocks.Select(lbIdString => ushort.Parse(lbIdString, System.Globalization.NumberStyles.AllowHexSpecifier)).ToFrozenSet();
        }
    }
}
