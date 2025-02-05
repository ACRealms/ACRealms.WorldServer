using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
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
        public IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesInt =>
            new CovariantReadOnlyDictionaryPolyKeyStructValue<PropertyInt, ushort, int>(PropertiesInt?.AsReadOnly());
        public IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesInt64 =>
            new CovariantReadOnlyDictionaryPolyKeyStructValue<PropertyInt64, ushort, long>(PropertiesInt64?.AsReadOnly());
        public IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesFloat =>
            new CovariantReadOnlyDictionaryPolyKeyStructValue<PropertyFloat, ushort, double>(PropertiesFloat?.AsReadOnly());
        public IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesBool =>
            new CovariantReadOnlyDictionaryPolyKeyStructValue<PropertyBool, ushort, bool>(PropertiesBool?.AsReadOnly());
        public IDictionaryConvertibleKeyObjectValue<ushort> CovariantPropertiesString =>
            new CovariantReadOnlyDictionaryPolyKeyObjectValue<PropertyString, ushort, string>(PropertiesString?.AsReadOnly());
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

        public static bool RespondsTo(string key) => IWeenie<Biota>.Prototype(key) != null;
        public static Type TypeOfProperty(string key) => IWeenie<Biota>.Prototype(key)?.ValueType;

        static readonly FrozenDictionary<Type, Func<Biota, IDictionaryConvertibleKeyStructValue<ushort>>> ValueDicts = new Dictionary<Type, Func<Biota, IDictionaryConvertibleKeyStructValue<ushort>>>()
        {
            { typeof(int), static (b) => b.CovariantPropertiesInt },
            { typeof(long), static (b) => b.CovariantPropertiesInt64 },
            { typeof(double), static (b) => b.CovariantPropertiesFloat },
            { typeof(bool), static (b) => b.CovariantPropertiesBool },
        }.ToFrozenDictionary();

        static readonly FrozenDictionary<Type, Func<Biota, IDictionaryConvertibleKeyObjectValue<ushort>>> ObjectDicts = new Dictionary<Type, Func<Biota, IDictionaryConvertibleKeyObjectValue<ushort>>>()
        {
            { typeof(string), static (b) => b.CovariantPropertiesString }
        }.ToFrozenDictionary();


        #region IResolvableContext
        IPrototypes IResolvableContext.Prototypes => BiotaPropertyPrototypes.Instance;
        IResolvableContext IResolvableContext.UnderlyingContext => this;
        bool IResolvableContext.TryFetchValue(IPrototype prototype, out ValueType result)// Type valueType, ValueType key, out ValueType result)
        {
            var dict = ValueDicts[prototype.ValueType](this);
            return dict.TryFetchValueWithUnderlying((ushort)prototype.UntypedRawKey, out result);
        }

        bool IResolvableContext.TryFetchObject(IPrototype prototype, out object result)
        {
            var dict = ObjectDicts[prototype.ValueType](this);
            return dict.TryFetchObjectWithUnderlying((ushort)prototype.UntypedRawKey, out result);
        }
        #endregion IResolvableContext
    }
}
