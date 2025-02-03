using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Net.Http.Headers;
using ACE.Entity.ACRealms;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACRealms;
using ACRealms.Prototypes;

namespace ACE.Entity.Models
{
    /// <summary>
    /// Only populated collections and dictionaries are initialized.
    /// We do this to conserve memory in ACE.Server
    /// Be sure to check for null first.
    /// </summary>
    public class Biota : IWeenie<Biota>, IResolvableContext<BiotaPropertyPrototypes, Biota>
    {
        public ulong Id { get; set; }
        public uint WeenieClassId { get; set; }
        public WeenieType WeenieType { get; set; }

        public IDictionary<PropertyBool, bool> PropertiesBool { get; set; }
        public IDictionary<PropertyDataId, uint> PropertiesDID { get; set; }
        public IDictionary<PropertyFloat, double> PropertiesFloat { get; set; }
        public IDictionary<PropertyInstanceId, ulong> PropertiesIID { get; set; }
        public IDictionary<PropertyInt, int> PropertiesInt { get; set; }
        public IDictionary<PropertyInt64, long> PropertiesInt64 { get; set; }
        public IDictionary<PropertyString, string> PropertiesString { get; set; }

        // TODO: Fix these, they are inefficient (unnecessary?), still proof of concept
        public IDictionaryConvertibleKey<ushort> CovariantPropertiesInt =>
            new CovariantReadOnlyDictionaryPolyKey<PropertyInt, ushort, int>(PropertiesInt?.AsReadOnly());
        public IDictionaryConvertibleKey<ushort> CovariantPropertiesInt64 =>
            new CovariantReadOnlyDictionaryPolyKey<PropertyInt64, ushort, long>(PropertiesInt64?.AsReadOnly());
        public IDictionaryConvertibleKey<ushort> CovariantPropertiesFloat =>
            new CovariantReadOnlyDictionaryPolyKey<PropertyFloat, ushort, double>(PropertiesFloat?.AsReadOnly());
        public IDictionaryConvertibleKey<ushort> CovariantPropertiesBool =>
            new CovariantReadOnlyDictionaryPolyKey<PropertyBool, ushort, bool>(PropertiesBool?.AsReadOnly());
        public IDictionaryConvertibleKey<ushort> CovariantPropertiesString =>
            new CovariantReadOnlyDictionaryPolyKey<PropertyString, ushort, string>(PropertiesString?.AsReadOnly());
        public IDictionary<PositionType, PropertiesPosition> PropertiesPosition { get; set; }

        public IDictionary<int, float /* probability */> PropertiesSpellBook { get; set; }

        public IList<PropertiesAnimPart> PropertiesAnimPart { get; set; }
        public IList<PropertiesPalette> PropertiesPalette { get; set; }
        public IList<PropertiesTextureMap> PropertiesTextureMap { get; set; }

        // Properties for all world objects that typically aren't modified over the original weenie
        public ICollection<PropertiesCreateList> PropertiesCreateList { get; set; }
        public ICollection<PropertiesEmote> PropertiesEmote { get; set; }
        public HashSet<int> PropertiesEventFilter { get; set; }
        public IList<PropertiesGenerator> PropertiesGenerator { get; set; }

        // Properties for creatures
        public IDictionary<PropertyAttribute, PropertiesAttribute> PropertiesAttribute { get; set; }
        public IDictionary<PropertyAttribute2nd, PropertiesAttribute2nd> PropertiesAttribute2nd { get; set; }
        public IDictionary<CombatBodyPart, PropertiesBodyPart> PropertiesBodyPart { get; set; }
        public IDictionary<Skill, PropertiesSkill> PropertiesSkill { get; set; }

        // Properties for books
        public PropertiesBook PropertiesBook { get; set; }
        public IList<PropertiesBookPageData> PropertiesBookPageData { get; set; }

        // Biota additions over Weenie
        public IDictionary<ulong /* Character ID */, PropertiesAllegiance> PropertiesAllegiance { get; set; }
        public ICollection<PropertiesEnchantmentRegistry> PropertiesEnchantmentRegistry { get; set; }
        public IDictionary<ulong /* Player GUID */, bool /* Storage */> HousePermissions { get; set; }

        public IPrototypes Prototypes => BiotaPropertyPrototypes.Instance;
        public Biota UnderlyingContext => this;

        public static bool RespondsTo(string key) => IWeenie<Biota>.Prototype(key) != null;
        public static Type TypeOfProperty(string key) => IWeenie<Biota>.Prototype(key)?.ValueType;

        static readonly FrozenDictionary<Type, Func<Biota, IDictionaryConvertibleKey<ushort>>> Dicts = new Dictionary<Type, Func<Biota, IDictionaryConvertibleKey<ushort>>>()
        {
            { typeof(int?), (b) => b.CovariantPropertiesInt },
            { typeof(long?), (b) => b.CovariantPropertiesInt64 },
            { typeof(double?), (b) => b.CovariantPropertiesFloat },
            { typeof(bool?), (b) => b.CovariantPropertiesBool },
            { typeof(string), (b) => b.CovariantPropertiesString }

        }.ToFrozenDictionary();

        internal TVal? FetchContextProperty<TVal>(string name)
        {
            var proto = Prototypes.GetPrototype<TVal>(name);
            return proto.Fetch(this);
        }

        internal TVal? Fetch<TEnum, TVal>(ushort key)
        {
            var dict = Dicts[typeof(TVal)](this);
            var fetchResult = dict.TryFetchWithUnderlying(key, out var result);
            return (TVal)result;
        }
    }
}
