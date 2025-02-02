using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Prototypes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class PropertyPrimaryAttributeBase : Attribute { }
    public abstract class PropertyPrimaryAttribute<TPrimitive> : PropertyPrimaryAttributeBase { }

    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public abstract class RequiresPrimaryAttributeAttribute(Type requiredAttributeType) : Attribute
    {
        public Type RequiredAttributeType { get; } = requiredAttributeType;
    }

    public abstract class RequiresPrimaryAttributeAttribute<TPrimitive>(Type requiredAttributeType)
        : Prototypes.RequiresPrimaryAttributeAttribute(requiredAttributeType)
        where TPrimitive : IEquatable<TPrimitive>
    { }

    public class RequiresPrimaryAttributeAttribute<TAttribute, TPrimitive>
        : Prototypes.RequiresPrimaryAttributeAttribute<TPrimitive>
        where TAttribute : PropertyPrimaryAttribute<TPrimitive>
        where TPrimitive : IEquatable<TPrimitive>
    {
        public RequiresPrimaryAttributeAttribute()
            : base(typeof(TAttribute)) { }
    }
}
