using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public partial class Realm
    {
        public Realm()
        {
            RealmPropertiesBool = new List<RealmPropertiesBool>();
            RealmPropertiesFloat = new List<RealmPropertiesFloat>();
            RealmPropertiesInt = new List<RealmPropertiesInt>();
            RealmPropertiesInt64 = new List<RealmPropertiesInt64>();
            RealmPropertiesString = new List<RealmPropertiesString>();
            RealmRulesetLinksLinkedRealm = new List<RealmRulesetLinks>();
            RealmRulesetLinksRealm = new List<RealmRulesetLinks>();
        }

        public ushort Id { get; set; }
        public ushort Type { get; set; }
        public string Name { get; set; }
        public ushort? ParentRealmId { get; set; }
        public ushort? PropertyCountRandomized { get; set; }

        public virtual IList<RealmPropertiesBool> RealmPropertiesBool { get; set; }
        public virtual IList<RealmPropertiesFloat> RealmPropertiesFloat { get; set; }
        public virtual IList<RealmPropertiesInt> RealmPropertiesInt { get; set; }
        public virtual IList<RealmPropertiesInt64> RealmPropertiesInt64 { get; set; }
        public virtual IList<RealmPropertiesString> RealmPropertiesString { get; set; }
        public virtual IList<RealmRulesetLinks> RealmRulesetLinksLinkedRealm { get; set; }
        public virtual IList<RealmRulesetLinks> RealmRulesetLinksRealm { get; set; }
    }
}
