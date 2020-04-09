using System;

namespace ACE.Server.Entity
{
    public class CombatModeLogEntry
    {
        public DateTime Timestamp;
        public string Message;

        public string StackTrace;

        public CombatModeLogEntry(string message)
        {
            Timestamp = DateTime.UtcNow;
            Message = message;

            StackTrace = Environment.StackTrace;
        }

        public override string ToString()
        {
            return $"{Timestamp.ToString("yyyy-MM-dd hh:mm:ss,fff")} - {Message}\n{StackTrace}";
        }
    }
}
