using ACE.Database.Adapter;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Command.Handlers;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Realms;
using ACRealms.Tests.Factories;
using ACRealms.Tests.Fixtures.Network;
using ACRealms.Tests.Helpers;
using System;
using System.Collections.Generic;
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
            var realm = RealmManager.GetRealm(1, includeRulesets: false);
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
            var realm = RealmManager.GetRealm(1, includeRulesets: false);
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

            DateTime timeContext = DateTime.Parse("2020-09-22T03:55:40Z").ToUniversalTime();
            var result = ACRealmsCommands.CompileRulesetRaw(player.Session, seed, "full", timeContext: timeContext);
            var result2 = ACRealmsCommands.CompileRulesetRaw(player.Session, seed, "full", timeContext: timeContext);

            Assert.NotEmpty(result);
            Assert.Equal(result, result2);

            int seed2;
            do { seed2 = System.Random.Shared.Next(); } while (seed2 == seed);
            result2 = ACRealmsCommands.CompileRulesetRaw(player.Session, seed2, "full", timeContext: timeContext);

            Assert.NotEqual(result, result2);
        }

        [Fact]
        public void TestRulesetCompile_ReversibleRulesetSeed()
        {
            LoadRealmFixture(FixtureName.simple_random);

            var player = new OnlinePlayerFactory()
            {
                HomeRealm = "Root",
                Character = new CharacterFactory { CharacterName = "TestRulesetCompile ReversibleRulesetSeed" }
            }.Create();

            var session = player.Session as FakeSession;
            ACRealmsCommands.HandleRulesetSeed(session);
            var msg = session.WaitForMessage<GameMessageSystemChat>((m) => m.Message.StartsWith("Ruleset seed: ")).Message;
            var seed = int.Parse(msg.Split(":")[1]);
            Assert.Equal(player.RealmRuleset.Context.RandomSeed, seed);

            Func<AppliedRuleset, List<AppliedRealmProperty<int>>> filter = (ruleset) =>
                ruleset.PropertiesInt.Keys.Intersect([
                    RealmPropertyInt.CreatureStrengthAdded,
                    RealmPropertyInt.CreatureEnduranceAdded,
                    RealmPropertyInt.CreatureCoordinationAdded,
                    RealmPropertyInt.CreatureQuicknessAdded,
                    RealmPropertyInt.CreatureFocusAdded,
                    RealmPropertyInt.CreatureSelfAdded])
                .Select(k => ruleset.PropertiesInt[k]).ToList();
            Func<AppliedRuleset, List<string>> mapPropNames = (ruleset) => filter(ruleset).Select(x => x.Options.Name).OrderBy(x => x).ToList();

            var ruleset = player.RealmRuleset;
            var propNamesOriginal = mapPropNames(ruleset);

            // Equal calls to recompile with seed should result in equal rulesets
            for (var i = 0; i < 5; i++)
            {
                var rulesetRecompiled1 = ruleset.Template.RecompileWithSeed(seed);
                var rulesetRecompiled2 = ruleset.Template.RecompileWithSeed(seed);
                Assert.Equal(seed, rulesetRecompiled1.Context.RandomSeed);
                Assert.Equal(seed, rulesetRecompiled2.Context.RandomSeed);
                Assert.Equal(mapPropNames(rulesetRecompiled1), mapPropNames(rulesetRecompiled2));
            }

            // And here, we make sure that with a trace enabled, re-compilation is still equal
            var rulesetRecompiledTraced = ruleset.Template.RecompileWithSeed(seed, Ruleset.MakeDefaultContext().WithTrace(true));
            rulesetRecompiledTraced.Context.ClearLog();
            Assert.Equal(propNamesOriginal, mapPropNames(rulesetRecompiledTraced));
            Assert.Equal(seed, rulesetRecompiledTraced.Context.RandomSeed);

            // Here, we make sure the seed displayed to the user can be accurately re-used
            for (var i = 0; i < 5; i++)
            {
                var rulesetRecompiled = ruleset.Template.RecompileWithSeed(seed);
                Assert.Equal(seed, rulesetRecompiled.Context.RandomSeed);

                var propNamesRecompiled = mapPropNames(rulesetRecompiled);
                Assert.Equal(propNamesOriginal, propNamesRecompiled);
            }

            // And here, we make sure that with a trace enabled, re-compilation is still equal
            rulesetRecompiledTraced = ruleset.Template.RecompileWithSeed(seed, Ruleset.MakeDefaultContext().WithTrace(true));
            rulesetRecompiledTraced.Context.ClearLog();
            Assert.Equal(seed, rulesetRecompiledTraced.Context.RandomSeed);

            var propNamesRecompiledTraced = mapPropNames(rulesetRecompiledTraced);
            Assert.Equal(propNamesOriginal, propNamesRecompiledTraced);
        }

        [Fact]
        public void TestRulesetCompile_CanBeHomeworld()
        {
            LoadRealmFixture(FixtureName.simple_random);

            var player = new OnlinePlayerFactory()
            {
                HomeRealm = "Root",
                Character = new CharacterFactory { CharacterName = "TestRulesetCompile CanBeHomeworld" }
            }.Create();

            //ACRealmsCommands.HandleCompileRuleset(player.Session, "all");
            //var prefix = "Logged compilation output to ";

            //foreach (var i in Enumerable.Range(0, 2))
            //{
            //    var message = ((FakeSession)player.Session).WaitForMessage<GameMessageSystemChat>(m => m.Message.StartsWith(prefix));
            //    var filename = message.Message.Substring(prefix.Length);
            //    Assert.True(File.Exists(filename));
            //    File.Copy(filename, $"TestRulesetCompileCommand-CanBeHomeworld-output-{i}.txt", true);
            //    File.Delete(filename);
            //}

            Assert.True(player.RealmRuleset.GetProperty(RealmPropertyBool.CanBeHomeworld));
        }
    }
}
