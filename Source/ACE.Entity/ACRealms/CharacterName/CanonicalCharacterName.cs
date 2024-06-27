using ACE.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Entity.ACRealms
{
    public abstract record CanonicalCharacterName(string Name, string RealmName)
    {
        /// <summary>
        /// Gets the name displayed to players sharing the same home realm
        /// </summary>
        public abstract string DisplayNameSameRealm { get; protected init; }

        /// <summary>
        /// Gets the name displayed to players who do not have the same home realm of this character
        /// </summary>
        public abstract string DisplayNameOtherRealm { get; protected init; }
    }
}
