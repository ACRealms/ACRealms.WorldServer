using System;
using System.Collections.Generic;

using log4net;

using ACE.Server.WorldObjects;

namespace ACE.Server.Entity
{
    public class CombatModeLog
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Player Player;

        public List<CombatModeLogEntry> Messages;

        public CombatModeLog(Player player)
        {
            Player = player;

            Messages = new List<CombatModeLogEntry>();
        }

        public void Add(string message)
        {
            Messages.Add(new CombatModeLogEntry(message));

            TryPrune();
        }

        public static TimeSpan PruneTime = TimeSpan.FromSeconds(15);

        public void TryPrune()
        {
            var idx = -1;
            for (var i = 0; i < Messages.Count - 5; i++)
            {
                var message = Messages[i];
                if (DateTime.UtcNow - message.Timestamp > PruneTime)
                    idx = i;
                else
                    break;
            }
            if (idx != -1)
                Messages.RemoveRange(0, idx + 1);
        }

        public void ShowState()
        {
            for (var i = Messages.Count - 1; i >= 0; i--)
            {
                var message = Messages[i];
                log.Error(message);
            }
        }
    }
}
