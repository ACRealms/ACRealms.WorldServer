using ACE.Database.Models.Shard;
using ACE.Entity.Models;
using ACRealms.RealmProps.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.WorldObjects
{
    public partial class WorldObject : ACRealms.RealmProps.Contexts.ICanonicalContextEntity<IWorldObjectContextEntity, WorldObject>
    {
        public static bool RespondsTo(string key)
        {
            if (ACE.Entity.Models.Biota.RespondsTo(key))
                return true;

            // Whitelisted properties for WorldObject? Stick with just Biota for now
            return key == "Level";
        }

        public static Type TypeOfProperty(string key)
        {
            if (ACE.Entity.Models.Biota.RespondsTo(key))
                return ACE.Entity.Models.Biota.TypeOfProperty(key);

            return null;
        }
    }
}
