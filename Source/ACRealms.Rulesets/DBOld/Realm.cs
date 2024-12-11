using System;
using System.Collections.Generic;

namespace ACRealms.Rulesets.DBOld
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



        //[NotMapped]
        public string ParentRealmName { get; set; }

      //  [NotMapped]
        public Dictionary<ushort, Realm> Descendents = new Dictionary<ushort, Realm>();

        public void SetId(ushort value)
        {
            this.Id = value;
            foreach (var item in RealmPropertiesBool)
                item.RealmId = value;
            foreach (var item in RealmPropertiesInt)
                item.RealmId = value;
            foreach (var item in RealmPropertiesInt64)
                item.RealmId = value;
            foreach (var item in RealmPropertiesString)
                item.RealmId = value;
            foreach (var item in RealmPropertiesFloat)
                item.RealmId = value;
        }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
