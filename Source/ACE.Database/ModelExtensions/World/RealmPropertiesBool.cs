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
    public partial class RealmPropertiesBool : RealmPropertiesBase
    {
        /// <summary>
        /// Value of this Property
        /// </summary>
        public bool Value { get; set; }

        static readonly Type EnumType = typeof(RealmPropertyBool);

        public override AppliedRealmProperty<bool> ConvertRealmProperty()
        {
            var @enum = (RealmPropertyBool)Type;

            var proto = RealmPropertyPrototypes.Bool[@enum];
            var att = proto.PrimaryAttribute;
            var prop = new RealmPropertyOptions<bool>(proto, @enum.ToString(), Realm.Name, att.DefaultValue, Value, Locked, Probability, EnumType, Type);
            return new AppliedRealmProperty<bool>(RulesetCompilationContext.DefaultShared, Type, prop);
        }
    }
}
