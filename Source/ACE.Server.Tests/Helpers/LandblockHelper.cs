using ACE.Server.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Tests.Helpers
{
    public static class LandblockHelper
    {
        /// <summary>
        /// Gets a new landblock instance every time. 
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ACE.Server.Entity.Landblock LoadLandblock(ACE.Server.Realms.WorldRealm realm, ushort landblockId)
        {
            lock (LandblockManager.landblockMutex)
            {
                var lbid = new ACE.Entity.LandblockId(landblockId);
                uint iid;
                do
                {
                    ushort shortiid = (ushort)ACE.Common.ThreadSafeRandom.Next(1, ushort.MaxValue);
                    iid = ACE.Entity.Position.InstanceIDFromVars(realm.Realm.Id, shortiid, isTemporaryRuleset: false);
                }
                while (LandblockManager.IsLoaded(lbid, iid));

                return LandblockManager.GetLandblock(new ACE.Entity.LandblockId(landblockId), iid, null, false, wait: true);
            }
        }
    }
}
