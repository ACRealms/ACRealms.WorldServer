using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using ACRealms.RealmProps;
using ACRealms.RealmProps.Underlying;
using JObject = Newtonsoft.Json.Linq.JObject;

namespace ACRealms.Rulesets.DBOld
{
    internal abstract class RealmPropertiesBase
    {
        public ushort RealmId { get; set; }
        public int Type { get; internal init; }
        public bool Locked { get; set; }
        public double? Probability { get; set; }
        public Realm Realm { get; set; }
        internal Dictionary<string, object> RawScope { get; init; }
        internal abstract RealmPropertyScopeOptions ConvertScopeOptions();
        
        internal abstract void SetProperties(RealmPropertyJsonModel model);

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
    }

    internal abstract class RealmPropertiesBase<TEnum, TPrim> : RealmPropertiesBase
    where TEnum : Enum
    where TPrim : IParsable<TPrim>, IComparable<TPrim>, IEquatable<TPrim>
    {
        protected static Type EnumType = typeof(TEnum);
        internal TEnum EnumValue { get { var type = Type; return Unsafe.As<int, TEnum>(ref type); } }
        

        internal abstract TemplatedRealmProperty<TPrim> ConvertRealmProperty(RealmPropertyGroupOptions<TPrim> group, RealmPropertyScopeOptions scope);

        internal override RealmPropertyScopeOptions ConvertScopeOptions()
        {
            if (RawScope == null || RawScope.Count == 0)
                return RealmPropertyScopeOptions.Empty;
            foreach (var kvp in RawScope)
            {
                var scopeParam = kvp.Key;
                var scopeValue = kvp.Value;
                if (scopeValue is JObject)
                {
                    var scopeObjectDefinition = ((JObject)scopeValue).ToObject<Dictionary<string, object>>();
                    foreach (var scopeEntityPropKvp in scopeObjectDefinition)
                    {
                        var scopeEntityPropKey = scopeEntityPropKvp.Key;
                        var scopeEntityPropMatcherCriteria = scopeEntityPropKvp.Value;
                        if (scopeEntityPropMatcherCriteria is JObject)
                        {
                            var criteria = ((JObject)scopeEntityPropMatcherCriteria).ToObject<Dictionary<string, object>>();
                            var handle = RealmPropertyPrototypes.GetPrototypeHandle(EnumValue);
                            
                            // Get EnumType of this, load prototype to get context entity info, then assign criteria
                        }
                    }
                    var a = 1;
                }
                else
                    throw new NotImplementedException();
                var b = 1;
            }
            throw new NotImplementedException();
        }
    }

    // TPrim is the unboxed primitive value, required by the base type
    // TVal is the boxed value, and will be used for the Value property
    internal abstract class RealmPropertiesBaseWithBoxableValue<TEnum, TPrim, TVal> : RealmPropertiesBase<TEnum, TPrim>
        where TEnum : Enum
        where TPrim : IParsable<TPrim>, IComparable<TPrim>, IEquatable<TPrim>
    {
        public TVal Value { get; set; }
    }

    internal abstract class RealmPropertiesDirectBase<TEnum, TVal> : RealmPropertiesBaseWithBoxableValue<TEnum, TVal, TVal>
        where TEnum : Enum
        where TVal : IParsable<TVal>, IComparable<TVal>, IEquatable<TVal>
    {
        internal override void SetProperties(RealmPropertyJsonModel model)
        {
            model.ValidateValuePresent();
            this.Value = TVal.Parse(model.value, System.Globalization.CultureInfo.InvariantCulture);
            this.Locked = model.locked ?? false;
            this.Probability = model.probability;
        }

        internal override TemplatedRealmProperty<TVal> ConvertRealmProperty(RealmPropertyGroupOptions<TVal> group, RealmPropertyScopeOptions scope)
        {
            var prop = new RealmPropertyOptions<TVal>(group, Value, Locked, Probability, EnumType, Type, scope);
            return new TemplatedRealmProperty<TVal>(RulesetCompilationContext.DefaultShared, Type, prop);
        }
    }

    internal abstract class RealmPropertiesNumericBase<TEnum, TPrim> : RealmPropertiesBaseWithBoxableValue<TEnum, TPrim, TPrim?>
        where TEnum : Enum
        where TPrim : struct, System.Numerics.IMinMaxValue<TPrim>, IParsable<TPrim>, IComparable<TPrim>, IEquatable<TPrim>
    {
        public TPrim? RandomLowRange { get; set; }
        public TPrim? RandomHighRange { get; set; }
        public byte RandomType { get; set; }
        public byte CompositionType { get; set; }

        internal override void SetProperties(RealmPropertyJsonModel model)
        {
            if (model.value != null)
                this.Value = TPrim.Parse(model.value, System.Globalization.NumberFormatInfo.InvariantInfo);
            if (model.low != null)
            {
                this.RandomLowRange = TPrim.Parse(model.low, System.Globalization.NumberFormatInfo.InvariantInfo);
                this.RandomHighRange = TPrim.Parse(model.high, System.Globalization.NumberFormatInfo.InvariantInfo);
                if (RandomLowRange.Value.CompareTo(RandomHighRange.Value) > 0)
                    throw new Exception("high must be > low");
                if (!model.reroll.HasValue)
                    model.reroll = Enums.RealmPropertyRerollType.landblock;
            }
            this.Locked = model.locked ?? false;
            this.Probability = model.probability;
            this.RandomType = (byte)(model.reroll ?? Enums.RealmPropertyRerollType.never);
            this.CompositionType = (byte)model.compose;
        }

        internal override TemplatedRealmProperty<TPrim> ConvertRealmProperty(RealmPropertyGroupOptions<TPrim> group, RealmPropertyScopeOptions scope)
        {
            RealmPropertyOptions<TPrim> prop;
            if (Value.HasValue)
                prop = new RealmPropertyOptions<TPrim>(group, Value.Value, Locked, Probability, EnumType, Type, scope);
            else
            {
                var @default = group.HardDefaultValue;
                prop = new MinMaxRangedRealmPropertyOptions<TPrim>(group, @default, CompositionType, RandomType, RandomLowRange.Value, RandomHighRange.Value, Locked, Probability, EnumType, Type, scope);
            }
            return new TemplatedRealmProperty<TPrim>(RulesetCompilationContext.DefaultShared, Type, prop);
        }
    }

    internal sealed partial class RealmPropertiesBool : RealmPropertiesDirectBase<RealmPropertyBool, bool> { }
    internal sealed partial class RealmPropertiesString : RealmPropertiesDirectBase<RealmPropertyString, string> { }
    internal sealed partial class RealmPropertiesInt : RealmPropertiesNumericBase<RealmPropertyInt, int> { }
    internal sealed partial class RealmPropertiesInt64 : RealmPropertiesNumericBase<RealmPropertyInt64, long> { }
    internal sealed partial class RealmPropertiesFloat : RealmPropertiesNumericBase<RealmPropertyFloat, double> { }
}
