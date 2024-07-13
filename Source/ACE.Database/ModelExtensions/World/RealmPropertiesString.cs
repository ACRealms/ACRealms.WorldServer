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
    public partial class RealmPropertiesString : RealmPropertiesBase
    {
        static Type EnumType = typeof(RealmPropertyString);
        public override AppliedRealmProperty<string> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyString)Type;
            var proto = RealmPropertyPrototypes.String[@enum];
            var att = proto.PrimaryAttribute;
            var prop = new RealmPropertyOptions<string>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, Value, Locked, Probability, EnumType, Type);
            return new AppliedRealmProperty<string>(RulesetCompilationContext.DefaultShared, Type, prop);
        }
    }
}
