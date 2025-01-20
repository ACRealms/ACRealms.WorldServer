using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.RealmProps.Base
{
    public class RealmPropResolver<TEnum>
    {
        internal TEnum CanonicalProperty { get; init; }

        internal RealmPropResolver(TEnum canonicalProperty)
        {
            this.CanonicalProperty = canonicalProperty;
        }

        
    }
}
