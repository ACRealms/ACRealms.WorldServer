using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets.DBOld
{
    internal record RealmScopeProperty { }
    internal record RealmScopeProperty<TEntityPropPrimitive> : RealmScopeProperty { }
    internal record RealmScopePropertyEquatable<TEntityPropPrimitive>
        : RealmScopeProperty<TEntityPropPrimitive>
    {
        
    }

    internal record RealmScopePropertyComparable<TEntityPropPrimitive>
        : RealmScopePropertyEquatable<TEntityPropPrimitive>
        where TEntityPropPrimitive : struct
    {
        internal TEntityPropPrimitive? Equal { get; init; }
        internal TEntityPropPrimitive? NotEqual { get; init; }
        internal TEntityPropPrimitive? GreaterThan { get; init; }
        internal TEntityPropPrimitive? GreaterThanOrEqual { get; init; }
        internal TEntityPropPrimitive? LessThanOrEqual { get; init; }
        internal TEntityPropPrimitive? LessThan { get; init; }
    }

    internal record RealmScopePropertyBool : RealmScopePropertyComparable<bool> { }
    internal record RealmScopePropertyInt : RealmScopePropertyComparable<int> { }
    internal record RealmScopePropertyInt64 : RealmScopePropertyComparable<long> { }
    internal record RealmScopePropertyString : RealmScopePropertyEquatable<string> { }
}
