using System;
using System.Collections.Generic;

namespace ACE.Database.Models.World
{
    public partial class Realm
    {
        public Realm()
        {
            RealmPropertiesBool = new HashSet<RealmPropertiesBool>();
            RealmPropertiesFloat = new HashSet<RealmPropertiesFloat>();
            RealmPropertiesInt = new HashSet<RealmPropertiesInt>();
            RealmPropertiesInt64 = new HashSet<RealmPropertiesInt64>();
            RealmPropertiesString = new HashSet<RealmPropertiesString>();
            RealmRulesetLinksLinkedRealm = new HashSet<RealmRulesetLinks>();
            RealmRulesetLinksRealm = new HashSet<RealmRulesetLinks>();
        }

        public ushort Id { get; set; }
        public ushort Type { get; set; }
        public string Name { get; set; }
        public ushort? ParentRealmId { get; set; }
        public ushort? PropertyCountRandomized { get; set; }

        public virtual ICollection<RealmPropertiesBool> RealmPropertiesBool { get; set; }
        public virtual ICollection<RealmPropertiesFloat> RealmPropertiesFloat { get; set; }
        public virtual ICollection<RealmPropertiesInt> RealmPropertiesInt { get; set; }
        public virtual ICollection<RealmPropertiesInt64> RealmPropertiesInt64 { get; set; }
        public virtual ICollection<RealmPropertiesString> RealmPropertiesString { get; set; }
        public virtual ICollection<RealmRulesetLinks> RealmRulesetLinksLinkedRealm { get; set; }
        public virtual ICollection<RealmRulesetLinks> RealmRulesetLinksRealm { get; set; }
    }
}
