using ACE.Database.Adapter;
using ACE.Entity.Enum.Properties;
using ACE.Server.Command.Handlers;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;
using ACRealms.Tests.Factories;
using ACRealms.Tests.Fixtures.Network;
using ACRealms.Tests.Helpers;
using System.IO;
using System.Linq;
using Xunit;
using static ACRealms.Tests.Helpers.RealmFixtures;

namespace ACRealms.Tests.Server.Realms.Compilation
{
    [Collection(nameof(WorldCollection))]
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

        [Fact]
        public void TestRulesetCompileCommand()
        {
            LoadRealmFixture(FixtureName.simple);

            var player = new OnlinePlayerFactory()
            {
                HomeRealm = "Modern Realm",
                Character = new CharacterFactory { CharacterName = "TestRulesetCompileCommand" }
            }.Create();

            ACRealmsCommands.HandleCompileRuleset(player.Session, "all", "19191919");
            var prefix = "Logged compilation output to ";
            
            foreach (var i in Enumerable.Range(0, 1))
            {
                var message = ((FakeSession)player.Session).WaitForMessage<GameMessageSystemChat>(m => m.Message.StartsWith(prefix));
                var filename = message.Message.Substring(prefix.Length);
                Assert.True(File.Exists(filename));
                File.Copy(filename, $"TestRulesetCompileCommand-Simple-output-{i}.txt", true);
                File.Delete(filename);
            }
        }

        [Fact]
        public void TestRulesetCompileCommand_Coverage()
        {
            LoadRealmFixture(FixtureName.coverage);

            var player = new OnlinePlayerFactory()
            {
                HomeRealm = "Compose Realm 1",
                Character = new CharacterFactory { CharacterName = "TestRulesetCompileCommand Coverage" }
            }.Create();

            ACRealmsCommands.HandleCompileRuleset(player.Session, "all", "19191919");
            var prefix = "Logged compilation output to ";

            foreach (var i in Enumerable.Range(0, 2))
            {
                var message = ((FakeSession)player.Session).WaitForMessage<GameMessageSystemChat>(m => m.Message.StartsWith(prefix));
                var filename = message.Message.Substring(prefix.Length);
                Assert.True(File.Exists(filename));
                File.Copy(filename, $"TestRulesetCompileCommand-Coverage-output-{i}.txt", true);
                File.Delete(filename);
            }
        }

        [Fact]
        public void TestRulesetCompile_DeterministicRandomization()
        {
            LoadRealmFixture(FixtureName.coverage);

            var player = new OnlinePlayerFactory()
            {
                HomeRealm = "Compose Realm 1",
                Character = new CharacterFactory { CharacterName = "TestRulesetCompile Deterministic" }
            }.Create();
            var seed = System.Random.Shared.Next();

            var result = ACRealmsCommands.CompileRulesetRaw(player.Session, seed, "full");
            var result2 = ACRealmsCommands.CompileRulesetRaw(player.Session, seed, "full");
            Assert.NotEmpty(result);
            Assert.Equal(result, result2);
        }
    }
}
