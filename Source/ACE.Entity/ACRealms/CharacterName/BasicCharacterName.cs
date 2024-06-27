using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Entity.ACRealms
{
    public record BasicCharacterName(string Name, string RealmName)
        : CanonicalCharacterName(Name, RealmName)
    {
        public override string DisplayNameSameRealm { get; protected init; } = Name;
        public override string DisplayNameOtherRealm { get; protected init; } = Name;
    }
}
