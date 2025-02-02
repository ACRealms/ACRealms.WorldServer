using ACRealms.RealmProps.Contexts;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
#nullable enable

using SecondaryDict = System.Collections.Frozen.FrozenDictionary<System.Type, ACRealms.RealmProps.RealmPropertySecondaryAttributeBase>;

namespace ACRealms.RealmProps
{
    // These are loaded once at startup, and derived from the generated source attributes of RealmProps
    // This is the warm data representing the metadata about a property (not linked to any ruleset)
    internal static class RealmPropertyPrototypes
    {
        public static IReadOnlyDictionary<RealmPropertyBool, RealmPropertyBoolPrototype> Bool { get; } = RealmPropertyHelper.BuildPrototypes<RealmPropertyBool, RealmPropertyBoolPrototype, bool, RealmPropertyPrimaryAttribute<bool>>();
        public static IReadOnlyDictionary<RealmPropertyInt, RealmPropertyIntPrototype> Int { get; } = RealmPropertyHelper.BuildPrototypes<RealmPropertyInt, RealmPropertyIntPrototype, int, RealmPropertyPrimaryMinMaxAttribute<int>>();
        public static IReadOnlyDictionary<RealmPropertyInt64, RealmPropertyInt64Prototype> Int64 { get; } = RealmPropertyHelper.BuildPrototypes<RealmPropertyInt64, RealmPropertyInt64Prototype, long, RealmPropertyPrimaryMinMaxAttribute<long>>();
        public static IReadOnlyDictionary<RealmPropertyFloat, RealmPropertyFloatPrototype> Float { get; } = RealmPropertyHelper.BuildPrototypes<RealmPropertyFloat, RealmPropertyFloatPrototype, double, RealmPropertyPrimaryMinMaxAttribute<double>>();
        public static IReadOnlyDictionary<RealmPropertyString, RealmPropertyStringPrototype> String { get; } = RealmPropertyHelper.BuildPrototypes<RealmPropertyString, RealmPropertyStringPrototype, string, RealmPropertyPrimaryAttribute<string>>();
        internal static IReadOnlyDictionary<Type, Type> ContextMappings { get; private set; } = FrozenDictionary<Type, Type>.Empty;

        static class HostAssemblyContext
        {
            private static System.Reflection.Assembly? HostAssembly;

            internal static readonly Lazy<List<Type>> CanonicalContextClasses = new Lazy<List<Type>>(() =>
            {
                return
                    HostAssembly!
                    .GetTypes()
                    .Where(x =>
                        !x.IsValueType &&
                        x.IsClass &&
                        DumpInterface(x).Any(i => i.IsGenericType && i.IsAssignableTo(typeof(ICanonicalContextEntity)))
                    ).ToList();
            }, false);

            static bool initialized = false;
            internal static void Initialize(System.Reflection.Assembly hostAssembly)
            { 
                if (initialized)
                    throw new InvalidOperationException("Already initialized");
                HostAssembly = hostAssembly;
                _ = CanonicalContextClasses.Value;

            }

            // source https://stackoverflow.com/a/52803620/881111
            public static IEnumerable<Type> DumpInterface(Type @type) // TODO: Improve performance
            {
                if (@type.IsClass == false)
                {
                    throw new NotSupportedException($"{@type} must be a class.");
                }

                var allInterfaces = new HashSet<Type>(@type.GetInterfaces());
                var baseType = @type.BaseType;
                //If it is not null, it might implement some other interfaces
                if (baseType != null)
                {
                    //So let us remove all the interfaces implemented by the base class
                    allInterfaces.ExceptWith(baseType.GetInterfaces());
                }

                //NOTE: allInterfaces now only includes interfaces implemented by the most derived class and
                //interfaces implemented by those(interfaces of the most derived class)

                //We want to remove interfaces that are implemented by other interfaces
                //i.e
                //public interface A : B{}
                //public interface B {}
                //public class Top : A{}→ We only want to dump interface A so interface B must be removed

                var toRemove = new HashSet<Type>();
                //Considering class A given above allInterfaces contain A and B now
                foreach (var implementedByMostDerivedClass in allInterfaces)
                {
                    //For interface A this will only contain single element, namely B
                    //For interface B this will an empty array

                    foreach (var implementedByOtherInterfaces in implementedByMostDerivedClass.GetInterfaces())
                    {
                        toRemove.Add(implementedByOtherInterfaces);
                    }
                }
                
                //Finally remove the interfaces that do not belong to the most derived class.
                allInterfaces.ExceptWith(toRemove);

                //Result
                return allInterfaces;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void InitializeContextMappings(System.Reflection.Assembly hostAssembly)
        {
            HostAssemblyContext.Initialize(hostAssembly);
            var dict = new Dictionary<Type, Type>();
            foreach(var type in HostAssemblyContext.CanonicalContextClasses.Value)
            {
                var i = type.GetInterface("ACRealms.RealmProps.Contexts.ICanonicalContextEntity`2")!;
                var src = i.GenericTypeArguments[0];
                var dest = i.GenericTypeArguments[1];
                dict.Add(src, dest);
            }
            ContextMappings = dict.ToFrozenDictionary();
        }

        internal static RealmPropertyPrototype<TProp, TPrim> GetPrototypeHandle<TProp, TPrim>(TProp prop)
            where TProp : Enum
            where TPrim : IEquatable<TPrim>
        {
            RealmPropertyPrototypeBase result = prop switch
            {
                RealmPropertyBool => Bool[Unsafe.BitCast<TProp, RealmPropertyBool>(prop)],
                RealmPropertyInt => Int[Unsafe.BitCast<TProp, RealmPropertyInt>(prop)],
                RealmPropertyInt64 => Int64[Unsafe.BitCast<TProp, RealmPropertyInt64>(prop)],
                RealmPropertyFloat => Float[Unsafe.BitCast<TProp, RealmPropertyFloat>(prop)],
                RealmPropertyString => String[Unsafe.BitCast<TProp, RealmPropertyString>(prop)],
                _ => throw new InvalidOperationException("Unexpected canonical prop handle")
            };
            var typedResult = (RealmPropertyPrototype<TProp, TPrim>)result;
            return typedResult;
        }
    }

    internal abstract class RealmPropertyPrototypeBase
    {
        internal string CanonicalName { get; private init; }
        internal int RawIdentifier { get; private init; }
        internal IRealmPropertyPrimaryAttribute PrimaryAttributeBase { get; private init; }
        private SecondaryDict? SecondaryAttributes { get; init; }
        public string SerializedHardDefaultValue { get; private init; }
        
        internal IReadOnlyDictionary<string, IScopedWithAttribute> Contexts { get; private init; }
        protected internal RealmPropertyPrototypeBase(string canonicalName, int rawIdentifier, IRealmPropertyPrimaryAttribute primaryAttrBase, SecondaryDict? secondaryAttributes, string serializedHardDefaultValue,
            IEnumerable<IScopedWithAttribute> scopedWithAttributes)
        {
            CanonicalName = canonicalName;
            RawIdentifier = rawIdentifier;
            PrimaryAttributeBase = primaryAttrBase;
            SecondaryAttributes = secondaryAttributes;
            SerializedHardDefaultValue = serializedHardDefaultValue;
            Contexts = scopedWithAttributes.ToFrozenDictionary(x => x.Name);
        }

        public bool TryGetSecondaryValue<TSecondary, TResult>(Func<TSecondary, TResult> selector, out TResult? result)
            where TSecondary : RealmPropertySecondaryAttributeBase
        {
            if (SecondaryAttributes == null)
            {
                result = default;
                return false;
            }
            var t = typeof(TSecondary);
            if (!SecondaryAttributes!.ContainsKey(t))
            {
                result = default;
                return false;
            }

            var att = (TSecondary)SecondaryAttributes[t];
            result = selector(att);
            return true;
        }
    }

    internal abstract class RealmPropertyPrototype<TPrimitive> : RealmPropertyPrototypeBase
    where TPrimitive : IEquatable<TPrimitive>
    {
        public TPrimitive HardDefaultValue { get; init; }
        protected internal RealmPropertyPrototype(string canonicalName, int rawIdentifier, RealmPropertyPrimaryAttribute<TPrimitive> primaryAttrBase, SecondaryDict? secondaryAttributes, TPrimitive hardDefaultValue,
            IEnumerable<IScopedWithAttribute> scopedWithAttributes)
            : base(canonicalName, rawIdentifier, primaryAttrBase, secondaryAttributes, hardDefaultValue?.ToString() ?? "<null>", scopedWithAttributes)
        {
            HardDefaultValue = hardDefaultValue ?? default(TPrimitive) ?? (TPrimitive)(object)"";
        }
    }

    internal abstract class RealmPropertyPrototype<TEnum, TPrimitive> : RealmPropertyPrototype<TPrimitive>
    where TEnum : System.Enum
    where TPrimitive : IEquatable<TPrimitive>
    {
        public TEnum EnumVal { get; private init; }

        protected internal RealmPropertyPrototype(string canonicalName, TEnum enumVal, RealmPropertyPrimaryAttribute<TPrimitive> primaryAttrBase, SecondaryDict? secondaryAttributes, TPrimitive hardDefaultValue,
            IEnumerable<IScopedWithAttribute> scopedWithAttributes)
            : base(canonicalName, System.Runtime.CompilerServices.Unsafe.As<TEnum, int>(ref enumVal), primaryAttrBase, secondaryAttributes, hardDefaultValue, scopedWithAttributes)
        {
            EnumVal = enumVal;
        }
    }

    internal abstract class RealmPropertyPrototype<TEnum, TPrimitive, TAttribute> : RealmPropertyPrototype<TEnum, TPrimitive>
        where TEnum : System.Enum
        where TPrimitive : IEquatable<TPrimitive>
        where TAttribute : RealmPropertyPrimaryAttribute<TPrimitive>, IRealmPropertyPrimaryAttribute
    {
        public TAttribute PrimaryAttribute { get; private init; }

        protected internal RealmPropertyPrototype(string canonicalName, TEnum enumVal, TAttribute primaryAttribute, SecondaryDict? secondaryAttributes, TPrimitive hardDefaultValue,
            IEnumerable<IScopedWithAttribute> scopedWithAttributes)
            : base(canonicalName, enumVal, primaryAttribute, secondaryAttributes, hardDefaultValue, scopedWithAttributes)
        {
            PrimaryAttribute = primaryAttribute;
        }
    }

    internal sealed class RealmPropertyIntPrototype : RealmPropertyPrototype<RealmPropertyInt, int, RealmPropertyPrimaryMinMaxAttribute<int>>
    {
        internal RealmPropertyIntPrototype(string canonicalName, RealmPropertyInt enumVal, RealmPropertyPrimaryMinMaxAttribute<int> primaryAttribute, SecondaryDict? secondaryAttributes, int hardDefaultValue,
            IEnumerable<IScopedWithAttribute> scopedWithAttributes)
            : base(canonicalName, enumVal, primaryAttribute, secondaryAttributes, hardDefaultValue, scopedWithAttributes) { }
    }
    internal sealed class RealmPropertyInt64Prototype : RealmPropertyPrototype<RealmPropertyInt64, long, RealmPropertyPrimaryMinMaxAttribute<long>>
    {
        internal RealmPropertyInt64Prototype(string canonicalName, RealmPropertyInt64 enumVal, RealmPropertyPrimaryMinMaxAttribute<long> primaryAttribute, SecondaryDict? secondaryAttributes, long hardDefaultValue,
            IEnumerable<IScopedWithAttribute> scopedWithAttributes)
            : base(canonicalName, enumVal, primaryAttribute, secondaryAttributes, hardDefaultValue, scopedWithAttributes) { }
    }
    internal sealed class RealmPropertyBoolPrototype : RealmPropertyPrototype<RealmPropertyBool, bool, RealmPropertyPrimaryAttribute<bool>>
    {
        internal RealmPropertyBoolPrototype(string canonicalName, RealmPropertyBool enumVal, RealmPropertyPrimaryAttribute<bool> primaryAttribute, SecondaryDict? secondaryAttributes, bool hardDefaultValue,
            IEnumerable<IScopedWithAttribute> scopedWithAttributes)
            : base(canonicalName, enumVal, primaryAttribute, secondaryAttributes, hardDefaultValue, scopedWithAttributes) { }
    }
    internal sealed class RealmPropertyFloatPrototype : RealmPropertyPrototype<RealmPropertyFloat, double, RealmPropertyPrimaryMinMaxAttribute<double>>
    {
        internal RealmPropertyFloatPrototype(string canonicalName, RealmPropertyFloat enumVal, RealmPropertyPrimaryMinMaxAttribute<double> primaryAttribute, SecondaryDict? secondaryAttributes, double hardDefaultValue,
            IEnumerable<IScopedWithAttribute> scopedWithAttributes)
            : base(canonicalName, enumVal, primaryAttribute, secondaryAttributes, hardDefaultValue, scopedWithAttributes) { }
    }
    internal sealed class RealmPropertyStringPrototype : RealmPropertyPrototype<RealmPropertyString, string, RealmPropertyPrimaryAttribute<string>>
    {
        internal RealmPropertyStringPrototype(string canonicalName, RealmPropertyString enumVal, RealmPropertyPrimaryAttribute<string> primaryAttribute, SecondaryDict? secondaryAttributes, string hardDefaultValue,
            IEnumerable<IScopedWithAttribute> scopedWithAttributes)
            : base(canonicalName, enumVal, primaryAttribute, secondaryAttributes, hardDefaultValue, scopedWithAttributes) { }
    }

}
