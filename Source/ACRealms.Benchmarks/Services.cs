using Microsoft.Extensions.Hosting;

namespace ACRealms.Tests.Benchmarks
{
    internal class Services : IDisposable
    {
        internal readonly ACRealmsTestService TestService;

        public void Dispose()
        {
            TestService.Dispose();
        }

        public Services()
        {
            var builder = Host.CreateApplicationBuilder();

            var startup = new Startup();
            startup.ConfigureHostApplicationBuilder(builder);
            
            var host = builder.Build();
            TestService = new ACRealmsTestService(host.Services);
        }
    }
}
