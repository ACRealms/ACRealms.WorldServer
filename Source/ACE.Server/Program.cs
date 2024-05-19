using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using log4net;
using log4net.Config;

using ACE.Common;
using ACE.Database;
using ACE.DatLoader;
using ACE.Server.Command;
using ACE.Server.Managers;
using ACE.Server.Mods;
using ACE.Server.Network.Managers;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ACRealms.Tests")]


namespace ACE.Server
{
    partial class Program
    {


        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly bool IsRunningInContainer = Convert.ToBoolean(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));

        public static void Main(string[] args)
        {
            Services.ConfigureServicesForLiveEnvironment(); 
        }
    }
}
