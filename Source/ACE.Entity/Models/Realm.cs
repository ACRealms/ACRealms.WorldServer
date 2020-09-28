using System;
using System.Collections.Generic;

using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;

namespace ACE.Entity.Models
{
    public class Realm
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public RealmType Type { get; set; }
        public ushort? ParentRealmID { get; set; }


        public IDictionary<RealmPropertyBool, (bool value, bool locked)> PropertiesBool { get; set; }
        public IDictionary<RealmPropertyFloat, (double value, bool locked)> PropertiesFloat { get; set; }
        public IDictionary<RealmPropertyInt, (int value, bool locked)> PropertiesInt { get; set; }
        public IDictionary<RealmPropertyInt64, (long value, bool locked)> PropertiesInt64 { get; set; }
        public IDictionary<RealmPropertyString, (string value, bool locked)> PropertiesString { get; set; }
        public bool NeedsRefresh { get; set; }
    }
}
