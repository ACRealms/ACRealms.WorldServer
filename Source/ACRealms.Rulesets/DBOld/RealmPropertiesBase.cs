using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using ACRealms.RealmProps;
using ACRealms.RealmProps.Contexts;
using ACRealms.RealmProps.Underlying;
using ACRealms.Rulesets.Contexts;
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
            var proto = RealmPropertyPrototypes.GetPrototypeHandle<TProp, TVal>(prop);
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

        static readonly IReadOnlyDictionary<string, Type> PredicateMap = new Func<FrozenDictionary<string, Type>>(() =>
        {
            var dict = new Dictionary<string, Type>()
            {
                { "Equal", typeof(Contexts.Predicates.Equal<>) },
                { "NotEqual", typeof(Contexts.Predicates.NotEqual<>) },
                { "GreaterThan", typeof(Contexts.Predicates.GreaterThan<>) },
                { "GreaterThanOrEqual", typeof(Contexts.Predicates.GreaterThanOrEqual<>) },
                { "LessThan", typeof(Contexts.Predicates.LessThan<>) },
                { "LessThanOrEqual", typeof(Contexts.Predicates.LessThanOrEqual<>) }
            };
            return dict.ToFrozenDictionary();
        })();

        internal override RealmPropertyScopeOptions ConvertScopeOptions()
        {
            if (RawScope == null || RawScope.Count == 0)
                return RealmPropertyScopeOptions.Empty;

            var prototype = RealmPropertyPrototypes.GetPrototypeHandle<TEnum, TPrim>(EnumValue);
            var contextsOut = new Dictionary<string, FrozenDictionary<string, IRealmPropertyScope>>();

            foreach (var contextKvp in RawScope)
            {
                var scopeParam = contextKvp.Key;
                var scopePropsOut = new Dictionary<string, IRealmPropertyScope>();


                if (!prototype.Contexts.ContainsKey(scopeParam))
                    throw new InvalidDataException($"Scope '{scopeParam}' is not allowed for property '{prototype.CanonicalName}'");
                IScopedWithAttribute<IContextEntity> context = (IScopedWithAttribute<IContextEntity>)prototype.Contexts[scopeParam];

                var scopeValue = contextKvp.Value;
                if (!(scopeValue is JObject))
                    throw new InvalidDataException("Scope must be an object");

                var scopeObjectDefinition = ((JObject)scopeValue).ToObject<Dictionary<string, object>>();

                //Attempt to force predicate data into span for cheaper rule evaluation
                foreach (var scopeEntityPropKvp in scopeObjectDefinition)
                {
                    var scopeEntityPropKey = scopeEntityPropKvp.Key;
                    var compiledScope = MakeScope(prototype, scopeParam, context, scopeEntityPropKvp);

                    scopePropsOut.Add(scopeEntityPropKey, compiledScope);
                }
                contextsOut.Add(scopeParam, scopePropsOut.ToFrozenDictionary());
            }
            var outp = contextsOut.ToFrozenDictionary();
            return new RealmPropertyScopeOptions() { Scopes = outp };

            static IRealmPropertyScope MakeScope(RealmPropertyPrototype<TEnum, TPrim> prototype, string scopeParam, IScopedWithAttribute<IContextEntity> context, KeyValuePair<string, object> scopeEntityPropKvp)
            {
                var scopeEntityPropKey = scopeEntityPropKvp.Key;

                if (!context.RespondsTo(scopeEntityPropKey))
                    throw new NotImplementedException($"Invalid context! A key ({scopeEntityPropKey}) was attempted to be referenced in a scope ({scopeParam}) referencing entity type {context.Entity}");
                var ctxPropType = context.TypeOfProperty(scopeEntityPropKey);

                var isNullable = ctxPropType.IsGenericType && ctxPropType.GetGenericTypeDefinition() == typeof(Nullable<>);
                Type valType = isNullable ? ctxPropType.GenericTypeArguments[0] : ctxPropType;
                

                var scopeEntityPropMatcherCriteria = scopeEntityPropKvp.Value;
                var dictType = typeof(Dictionary<,>).MakeGenericType(typeof(string), valType);

                IDictionary criteriaBase;
                if (scopeEntityPropMatcherCriteria is JObject)
                    criteriaBase = (IDictionary)((JObject)scopeEntityPropMatcherCriteria).ToObject(dictType);
                else // Scope defined with a direct value
                {
                    // Downcast is required for int entity props scopes matched in short-form, as they are deserialized as long
                    if (valType == typeof(int) && scopeEntityPropMatcherCriteria.GetType() == typeof(long))
                    {
                        var longScope = (long)scopeEntityPropMatcherCriteria;
                        if (longScope > int.MaxValue || longScope < int.MinValue)
                            throw new InvalidDataException($"Scope param {scopeParam}.{scopeEntityPropKey} is out of range for System.Int32.");
                        scopeEntityPropMatcherCriteria = Convert.ToInt32(scopeEntityPropMatcherCriteria);
                    }
                    criteriaBase = new Dictionary<string, object>() { { "Equal", scopeEntityPropMatcherCriteria } };
                }

                var predicatesBase = criteriaBase.Keys.Cast<string>().Select(predicateTypeKey =>
                {
                    return CompilePredicate(predicateTypeKey, prototype, scopeParam, valType, criteriaBase);

                    static IPredicate CompilePredicate(string predicateTypeKey, RealmPropertyPrototype<TEnum, TPrim> prototype, string scopeParam, Type valType, IDictionary criteriaBase)
                    {
                        if (!PredicateMap.TryGetValue(predicateTypeKey, out var predicateGenericType))
                        {
                            throw new NotImplementedException(
                                $"Missing predicate type for scoped property {prototype.CanonicalName}, scope {scopeParam}, criterion {predicateTypeKey}. Check PredicateMap for missing entries.");
                        }
                        var predicateArg = criteriaBase[predicateTypeKey];

                        var predicateType = predicateGenericType.MakeGenericType(valType);
                        var predicate = (IPredicate)Activator.CreateInstance(
                            predicateType,
                            BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance,
                            binder: null,
                            args: [predicateArg],
                            System.Globalization.CultureInfo.InvariantCulture,
                            activationAttributes: null
                        );
                        return predicate;
                    }
                });
                return MakeScope_Inner(scopeEntityPropKey, valType, predicatesBase);

                static IRealmPropertyScope MakeScope_Inner(string scopeEntityPropKey, Type valType, IEnumerable<IPredicate> predicatesBase)
                {
                    var predicateMostDerivedCommonType = typeof(IPredicate<>).MakeGenericType(valType);
                    var castMethod = typeof(Enumerable).GetMethod("Cast", BindingFlags.Static | BindingFlags.Public);
                    var castMethodUsable = castMethod.MakeGenericMethod(predicateMostDerivedCommonType);
                    var predicates = castMethodUsable.Invoke(null, [predicatesBase]);

                    var predicateInterface = typeof(IPredicate<>).MakeGenericType(valType);
                    var enumerableInterface = typeof(IEnumerable<>).MakeGenericType(predicateInterface);
                    var aryConvs = typeof(ImmutableArray)
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Where(m =>
                            m.Name == "ToImmutableArray" &&
                            m.IsGenericMethod &&
                            m.ContainsGenericParameters &&
                            m.GetGenericArguments().FirstOrDefault()?.Name == "TSource" &&
                            m.GetParameters().FirstOrDefault()?.Name == "items"
                        );
                    var aryConv = aryConvs.Single();

                    var aryConvUsable = aryConv.MakeGenericMethod([predicateInterface]);
                    var immutableArray = (IList)aryConvUsable.Invoke(null, [predicates]);

                    var scopeOpsType = typeof(RealmPropertyScopeOps<>).MakeGenericType(valType);

                    var createMethod = scopeOpsType.GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic);
                    var scopeOps = createMethod.Invoke(null, [immutableArray]);

                    Type scopeOutType;
                    if (valType.IsClass)
                        scopeOutType = typeof(RealmPropertyEntityObjectPropScope<,>).MakeGenericType(scopeOpsType, valType);
                    else
                        scopeOutType = typeof(RealmPropertyEntityValuePropScope<,>).MakeGenericType(scopeOpsType, valType);

                    var scopeOut = (IRealmPropertyScope)Activator.CreateInstance(
                        scopeOutType,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance,
                        binder: null,
                        args: [scopeEntityPropKey, scopeOps],
                        System.Globalization.CultureInfo.InvariantCulture,
                        activationAttributes: null
                    );
                    return scopeOut;
                }
            }
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
