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

        IDictionary<PropertyBool, bool> propertiesBool;
        public IDictionary<PropertyBool, bool> PropertiesBool
        {
            get => propertiesBool;
            set
            {
                propertiesBool = value;
                CovariantPropertiesBool = new CovariantReadOnlyDictionaryPolyKeyStructValue<PropertyBool, ushort, bool>(value.AsReadOnly());
            }
        }
        public IDictionary<PropertyDataId, uint> PropertiesDID { get; set; }

        IDictionary<PropertyFloat, double> propertiesFloat;
        public IDictionary<PropertyFloat, double> PropertiesFloat
        {
            get => propertiesFloat;
            set
            {
                propertiesFloat = value;
                CovariantPropertiesFloat = new CovariantReadOnlyDictionaryPolyKeyStructValue<PropertyFloat, ushort, double>(value.AsReadOnly());
            }
        }
        public IDictionary<PropertyInstanceId, ulong> PropertiesIID { get; set; }

        IDictionary<PropertyInt, int> propertiesInt;
        public IDictionary<PropertyInt, int> PropertiesInt
        {
            get => propertiesInt;
            set
            {
                propertiesInt = value;
                CovariantPropertiesInt = new CovariantReadOnlyDictionaryPolyKeyStructValue<PropertyInt, ushort, int>(value.AsReadOnly());
            }
        }
        IDictionary<PropertyInt64, long> propertiesInt64;
        public IDictionary<PropertyInt64, long> PropertiesInt64
        {
            get => propertiesInt64;
            set
            {
                propertiesInt64 = value;
                CovariantPropertiesInt64 = new CovariantReadOnlyDictionaryPolyKeyStructValue<PropertyInt64, ushort, long>(value.AsReadOnly());
            }
        }
        IDictionary<PropertyString, string> propertiesString;
        public IDictionary<PropertyString, string> PropertiesString
        {
            get => propertiesString;
            set
            {
                propertiesString = value;
                CovariantPropertiesString = new CovariantReadOnlyDictionaryPolyKeyObjectValue<PropertyString, ushort, string>(value.AsReadOnly());
            }
        }

        private IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesInt { get; set; }
        private IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesInt64 { get; set; }
        private IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesFloat { get; set; }
        private IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesBool { get; set; }
        private IDictionaryConvertibleKeyObjectValue<ushort> CovariantPropertiesString { get; set; }
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
            if (dict == null)
            {
                result = null;
                return false;
            }
            return dict.TryFetchValueWithUnderlying((ushort)prototype.UntypedRawKey, out result);
        }

        bool IResolvableContext.TryFetchObject(IPrototype prototype, out object result)
        {
            var dict = ObjectDicts[prototype.ValueType](this);
            if (dict == null)
            {
                result = null;
                return false;
            }
            return dict.TryFetchObjectWithUnderlying((ushort)prototype.UntypedRawKey, out result);
        }
        #endregion IResolvableContext
    }
}
