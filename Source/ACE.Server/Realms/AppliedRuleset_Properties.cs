using ACE.Entity.Enum.RealmProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Realms
{
    partial class AppliedRuleset
    {
        public PlayerInstanceSelectMode RecallInstanceSelectMode
        {
            get { return (PlayerInstanceSelectMode)GetProperty(ACE.Entity.Enum.Properties.RealmPropertyInt.RecallInstanceSelectMode);  }
        }

        public PlayerInstanceSelectMode PortalInstanceSelectMode
        {
            get { return (PlayerInstanceSelectMode)GetProperty(ACE.Entity.Enum.Properties.RealmPropertyInt.PortalInstanceSelectMode); }
        }
    }
}
