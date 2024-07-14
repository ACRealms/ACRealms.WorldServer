global using ACE.Server.Realms;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Realms
{
    public static class LogExtensions
    {
        const bool NoDebug = true;
        public static void DebugLazy(this ILog logger, Func<string> loggerFunc)
        {
            if (NoDebug || !logger.IsDebugEnabled)
                return;
            logger.Debug(loggerFunc());
        }
    }
}
