using ACE.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Database.Models.World
{
    public abstract class RealmPropertiesBase
    {
        public ushort RealmId { get; set; }
        public ushort Type { get; set; }
        public bool Locked { get; set; }
        public double? Probability { get; set; }
        public virtual Realm Realm { get; set; }

        public abstract AppliedRealmProperty ConvertRealmProperty();
    }
}
