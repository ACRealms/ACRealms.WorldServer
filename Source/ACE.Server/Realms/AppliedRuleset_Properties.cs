using ACE.Entity.Enum.RealmProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACE.Entity.Enum.Properties;
using ACRealms.RealmProps.Enums;

namespace ACE.Server.Realms
{
    partial class AppliedRuleset
    {
        public PlayerInstanceSelectMode RecallInstanceSelectMode => (PlayerInstanceSelectMode)Props.Core.Instance.RecallInstanceSelectMode(this);
        public PlayerInstanceSelectMode PortalInstanceSelectMode => (PlayerInstanceSelectMode)Props.Core.Instance.PortalInstanceSelectMode(this);
    }
}
