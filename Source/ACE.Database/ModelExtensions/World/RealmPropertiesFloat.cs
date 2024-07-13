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
    public partial class RealmPropertiesFloat : RealmPropertiesBase
    {
        static Type EnumType = typeof(RealmPropertyFloat);
        public override AppliedRealmProperty<double> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyFloat)Type;
            var proto = RealmPropertyPrototypes.Float[@enum];
            var att = proto.PrimaryAttribute;
            RealmPropertyOptions<double> prop;
            if (Value.HasValue)
                prop = new RealmPropertyOptions<double>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, Value.Value, Locked, Probability, EnumType, Type);
            else
                prop = new MinMaxRangedRealmPropertyOptions<double>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, CompositionType, RandomType, RandomLowRange.Value, RandomHighRange.Value, Locked, Probability, EnumType, Type);
            return new AppliedRealmProperty<double>(RulesetCompilationContext.DefaultShared, Type, prop);
        }
    }
}
