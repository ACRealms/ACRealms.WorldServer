using ACE.Server.Managers;
using ACE.Server.Network;
using ACRealms.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using static ACRealms.Tests.Helpers.RealmFixtures;

namespace ACRealms.Tests.Server.Realms.Compilation
{
    public class RulesetCompileTests
    {
        [Fact]
        public void CanLoadBasicRealmFixture()
        {
            LoadRealmFixture(FixtureName.simple);
            var realm = RealmManager.GetRealm(1);
            Assert.NotNull(realm);
            var lb = LandblockHelper.LoadLandblock(realm, 0x7308);
            Assert.NotNull(lb);
            var ulgrimsHouse = lb.GetObject(new ACE.Entity.ObjectGuid(0x77308009, lb.Instance));
            Assert.Equal("Ulgrim's House", ulgrimsHouse.Name);
        }
    }
}
