global using BenchmarkDotNet.Attributes;
using ACE.Server.Managers;
using System.Globalization;
using System.Text;

namespace ACRealms.Tests.Benchmarks.Tests
{
    public abstract class ACRBenchmark
    {
        internal static Services? Services { get; set; }

#if DEBUG
        const string LocalRelativeConfigDir = ".";
#else
        const string LocalRelativeConfigDir = @"..\..\..\..";
#endif

        [GlobalSetup]
        public virtual void Setup()
        {
            if (WorldManager.WorldActive)
                Teardown();

            Startup.RelativeConfigPath = LocalRelativeConfigDir;
            Services = new Services();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [GlobalCleanup]
        public void Teardown()
        {
            try
            {
                Services?.Dispose();
            }
            catch (Exception) { }
            Services = null;
        }
    }
}
