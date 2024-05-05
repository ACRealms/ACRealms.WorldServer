using ACE.Entity.Enum.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.WorldObjects
{
    // This should be for AC Realms properties from 42000-42999 only. Use WorldObject_Properties_Custom.cs for custom properties
    partial class WorldObject
    {
        public ushort? ItemGeneratedAtRealmID
        {
            get => (ushort?)GetProperty(PropertyInt.ItemGeneratedAtRealmID);
            set { if (!value.HasValue) RemoveProperty(PropertyInt.ItemGeneratedAtRealmID); else SetProperty(PropertyInt.ItemGeneratedAtRealmID, value.Value); }
        }

        public ushort? ItemLootedHomeRealmID
        {
            get => (ushort?)GetProperty(PropertyInt.ItemLootedHomeRealmID);
            set { if (!value.HasValue) RemoveProperty(PropertyInt.ItemLootedHomeRealmID); else SetProperty(PropertyInt.ItemLootedHomeRealmID, value.Value); }
        }
    }
}
