using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ACRealms.RealmProps;

namespace ACRealms.Rulesets.DBOld
{
    internal abstract class RealmPropertiesBase
    {
        public ushort RealmId { get; set; }
        public int Type { get; set; }
        public bool Locked { get; set; }
        public double? Probability { get; set; }
        public virtual Realm Realm { get; set; }

        internal RealmPropertyScopeOptions ConvertScopeOptions()
        {
            throw new NotImplementedException();
        }

        internal RealmPropertyGroupOptions<TVal> ConvertGroupOptions<TVal, TProp>(TProp prop)
            where TVal : IEquatable<TVal>
            where TProp : Enum
        {
            var proto = RealmPropertyPrototypes.GetPrototypeHandle(prop);
            var type = Type;
            var @enum = Unsafe.As<int, TProp>(ref type);
            var propName = @enum.ToString();
            
            
            var att = (RealmPropertyPrimaryAttribute<TVal>)proto.PrimaryAttributeBase;
            var opts = new RealmPropertyGroupOptions<TVal>(proto, Realm.Name, propName, att.DefaultValue);

            return opts;
        }
        public abstract ITemplatedRealmProperty ConvertRealmProperty<TVal>(RealmPropertyGroupOptions<TVal> groupOptions, RealmPropertyScopeOptions scopeOptions)
            where TVal : IEquatable<TVal>;
    }
}
