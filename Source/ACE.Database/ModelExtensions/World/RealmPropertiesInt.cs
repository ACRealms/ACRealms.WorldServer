using ACE.Entity.ACRealms;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Enum.RealmProperties;
using ACE.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Database.Models.World
{
    public partial class RealmPropertiesInt : RealmPropertiesBase
    {
        static Type EnumType = typeof(RealmPropertyInt);
        public override AppliedRealmProperty<int> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyInt)Type;
            var proto = RealmPropertyPrototypes.Int[@enum];
            var att = proto.PrimaryAttribute;
            RealmPropertyOptions<int> prop;
            if (Value.HasValue)
                prop = new RealmPropertyOptions<int>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, Value.Value, Locked, Probability, EnumType, Type);
            else
                prop = new MinMaxRangedRealmPropertyOptions<int>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, CompositionType, RandomType, RandomLowRange.Value, RandomHighRange.Value, Locked, Probability, EnumType, Type);
            return new AppliedRealmProperty<int>(RulesetCompilationContext.DefaultShared, Type, prop);
        }
    }
}
