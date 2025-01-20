using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal RealmPropertyGroupOptions ConvertGroupOptions()
        {
            throw new NotImplementedException();
        }
        public abstract ITemplatedRealmProperty ConvertRealmProperty(RealmPropertyGroupOptions groupOptions, RealmPropertyScopeOptions scopeOptions);
    }
}
