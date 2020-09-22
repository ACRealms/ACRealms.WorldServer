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


        public IDictionary<RealmPropertyBool, bool> PropertiesBool { get; set; }
        public IDictionary<RealmPropertyFloat, double> PropertiesFloat { get; set; }
        public IDictionary<RealmPropertyInt, int> PropertiesInt { get; set; }
        public IDictionary<RealmPropertyInt64, long> PropertiesInt64 { get; set; }
        public IDictionary<RealmPropertyString, string> PropertiesString { get; set; }
        public bool NeedsRefresh { get; set; }
    }
}
