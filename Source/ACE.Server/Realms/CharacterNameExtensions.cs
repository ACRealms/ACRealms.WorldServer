using ACE.Entity.ACRealms;
using ACE.Server.Entity;
using ACE.Server.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Realms
{
    internal class CharacterNameExtensions
    {
        public static CanonicalCharacterName FromPlayer(IPlayer player)
            => Make(player.Name, player.HomeRealmIDRaw, player.Account.AccessLevel > 1);

        internal static CanonicalCharacterName Make(string name, int? rawRealmId, bool isAdmin)
        {
            string realmName = RealmManager.GetDisplayNameForAnyRawRealmId(rawRealmId);
            return Make(name, realmName, isAdmin);
        }

        public static CanonicalCharacterName Make(string name, string realmName, bool isAdmin)
        {
            if (isAdmin)
                return new AdminCharacterName(name, realmName);
            else if (Common.ACRealms.ACRealmsConfigManager.Config.CharacterCreationOptions.CharacterNamesUniquePerHomeRealm)
                return new BasicCharacterName(name, realmName);
            else
                return new ShardedCharacterName(name, realmName);
        }
    }
}
