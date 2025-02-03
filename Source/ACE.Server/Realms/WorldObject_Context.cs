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
        public IPrototypes Prototypes => Biota.Prototypes;

        public ACE.Entity.Models.Biota UnderlyingContext => Biota;

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

        public TVal? FetchContextProperty<TVal>(string name)
            where TVal : struct
        {
            return Biota.FetchContextProperty<TVal>(name);
        }

        public bool Match(FrozenDictionary<string, IRealmPropertyScope> propsToMatch)
        {
            throw new NotImplementedException();
        }
    }
}
