using ACE.Entity.Enum.RealmProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACE.Entity.Enum.Properties;

namespace ACE.Server.Realms
{
    partial class AppliedRuleset
    {
        public PlayerInstanceSelectMode RecallInstanceSelectMode => (PlayerInstanceSelectMode)GetProperty(RealmPropertyInt.RecallInstanceSelectMode);
        public PlayerInstanceSelectMode PortalInstanceSelectMode => (PlayerInstanceSelectMode)GetProperty(RealmPropertyInt.PortalInstanceSelectMode);
    }
}
