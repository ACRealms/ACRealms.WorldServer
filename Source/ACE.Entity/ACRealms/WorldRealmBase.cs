using ACE.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Entity.ACRealms
{
    public abstract class WorldRealmBase(Realm realm, RulesetBase template)
    {
        public Realm Realm { get; } = realm;


        // Temporary until we can get the class moved to this project
        internal RulesetBase TemplateBase { get; private init; } = template;
    }
}
