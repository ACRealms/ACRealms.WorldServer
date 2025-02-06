using ACE.Database.Models.Shard;
using ACE.Entity.ACRealms;
using ACE.Entity.Models;
using ACRealms.Prototypes;
using ACRealms.RealmProps.Contexts;
using ACRealms.Rulesets.Contexts;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.WorldObjects
{
    public partial class WorldObject :
        ACRealms.RealmProps.Contexts.ICanonicalContextEntity<IWorldObjectContextEntity, WorldObject>,
        ACRealms.Prototypes.IResolvableContext<BiotaPropertyPrototypes, ACE.Entity.Models.Biota>,
        IWorldObjectContextEntity
    {
        public IPrototypes Prototypes => UnderlyingContext.Prototypes;
        public IResolvableContext UnderlyingContext => Biota;

        public static bool RespondsTo(string key)
        {
            if (ACE.Entity.Models.Biota.RespondsTo(key))
                return true;
            return false; // TODO: Expand system for WorldObject props via an interface
        }

        public static Type TypeOfProperty(string key)
        {
            if (ACE.Entity.Models.Biota.RespondsTo(key))
                return ACE.Entity.Models.Biota.TypeOfProperty(key);

            return null;
        }

        bool IResolvableContext.TryFetchObject(IPrototype prototype, out object result)
        {
            throw new NotImplementedException();
        }

        bool IResolvableContext.TryFetchValue(IPrototype prototype, out ValueType result)
        {
            throw new NotImplementedException();
        }
    }
}
