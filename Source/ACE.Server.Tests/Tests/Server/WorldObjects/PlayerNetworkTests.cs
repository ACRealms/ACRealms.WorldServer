using ACE.Server.Factories;
using ACE.Server.Managers;
using ACRealms.Tests.Factories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ACRealms.Tests.Server.WorldObjects
{
    [Collection(nameof(WorldCollection))]
    public class PlayerNetworkTests
    {
        [Fact]
        public void PlayerCanEnterWorld()
        {
            var player = OnlinePlayerFactory.Make();
            Assert.NotNull(player);
        }
    }
}
