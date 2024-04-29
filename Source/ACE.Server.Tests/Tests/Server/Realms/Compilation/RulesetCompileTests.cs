using ACE.Database.Adapter;
using ACE.Entity.Enum.Properties;
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

        [Fact]
        public void TestServerPropertyFallback()
        {
            var att = RealmConverter.PropertyDefinitionsFloat[RealmPropertyFloat.Spellcasting_Max_Angle];
            Assert.NotNull(att.DefaultFromServerProperty);
            var serverPropOrig = PropertyManager.GetDouble(att.DefaultFromServerProperty).Item;

            PropertyManager.ModifyDouble(att.DefaultFromServerProperty, 1);
            var serverProp = PropertyManager.GetDouble(att.DefaultFromServerProperty).Item;
            Assert.NotEqual(serverProp, att.DefaultValue);

            LoadRealmFixture(FixtureName.simple);
            var realm = RealmManager.GetRealm(1);
            var lb = LandblockHelper.LoadLandblock(realm, 0x8903);

            Assert.Equal(1, lb.RealmRuleset.GetProperty(RealmPropertyFloat.Spellcasting_Max_Angle));

            PropertyManager.ModifyDouble(att.DefaultFromServerProperty, 2);
            Assert.Equal(2, lb.RealmRuleset.GetProperty(RealmPropertyFloat.Spellcasting_Max_Angle));

            PropertyManager.ModifyDouble(att.DefaultFromServerProperty, serverPropOrig);
        }
    }
}
