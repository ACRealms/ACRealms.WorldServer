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
        }

        public ushort Id { get; set; }
        public ushort Type { get; set; }
        public string Name { get; set; }
        public ushort? ParentRealmId { get; set; }

        public virtual ICollection<RealmPropertiesBool> RealmPropertiesBool { get; set; }
        public virtual ICollection<RealmPropertiesFloat> RealmPropertiesFloat { get; set; }
        public virtual ICollection<RealmPropertiesInt> RealmPropertiesInt { get; set; }
        public virtual ICollection<RealmPropertiesInt64> RealmPropertiesInt64 { get; set; }
        public virtual ICollection<RealmPropertiesString> RealmPropertiesString { get; set; }
    }
}
