using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ACE.Server.Tests
{
    public static class Initialization
    {
        private static bool initialized = false;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void InitializeForTests()
        {
            if (initialized)
                return;
            initialized = true;
            Common.ConfigManager.InitializeForTesting();
        }
    }
}
