using ACE.Common.ACRealms;
using ACE.Database;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Server.Network.Managers;
using ACE.Server.WorldObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Managers.ACRealms
{
    internal static class RealmsFromACESetup
    {
        public static bool ACEMigrationQueued => PropertyManager.GetBool("acrsystem_enable_ace_migration_mode_at_next_startup", true).Item;
        public static bool ACEMigrationInProgress => Stage1MigrationRequired;
        public static bool StatusQueriedDuringStartup => Stage1QueriedDuringStartup;
        public static bool Stage1MigrationRequired { get; private set; }
        private static bool Stage1QueriedDuringStartup { get; set; }

        private static void ValidateRunnable()
        {
            if (WorldManager.WorldStatus == WorldManager.WorldStatusState.Open)
                throw new InvalidOperationException("RealmsFromACESetup may only be run while no players are online and the world is closed.");
            if (PlayerManager.GetOnlineCount() > 0)
                throw new InvalidOperationException("RealmsFromACESetup may only be run while no players are online and the world is closed.");
        }

        public static void QueryStage1()
        {
            Stage1QueriedDuringStartup = true;
            if (!ACEMigrationQueued)
                return;

            ValidateRunnable();

            if (HouseManager.Initialized)
                throw new InvalidOperationException("RealmsFromACESetup Stage 1 must start before the HouseManager is initialized.");
            if (!PropertyManager.Initialized)
                throw new InvalidOperationException("RealmsFromACESetup Stage 1 must start after the PropertyManager is initialized.");

            if (!IsStage1Complete)
            {
                Stage1MigrationRequired = true;
                ACE.Entity.ACRealms.RealmsFromACESetupHelper.UnsafeInstanceIDTemporarilyAllowed = true;
            }

            return;
        }

        // Returns false if a failure requires server shutdown
        public static bool DoMigrationsIfRequired()
        {
            if (!ACEMigrationQueued) return true;

            bool success = RunStage1IfRequired();
            if (!success) return false;

            success = RunStage2IfRequired();
            if (!success) return false;

            // wait for flush to database
            int shardQueueCount;
            int lastQueueCount = 0;
            while ((shardQueueCount = DatabaseManager.Shard.QueueCount) > 0)
            {
                if (shardQueueCount < lastQueueCount) // some progress made
                   Console.Write(".");
                lastQueueCount = shardQueueCount;
                System.Threading.Thread.Sleep(10);
            }

            if (IsMigrationComplete)
            {
                SignalMigrationComplete();
                return true;
            }
            else
            {
                Console.WriteLine("Warning: Failed to complete RealmsFromACESetup!");
                ServerManager.DoShutdownNow();
                return false;
            }
        }

        private static bool RunStage1IfRequired()
        {
            Stage1QueriedDuringStartup = true;
            if (!ACEMigrationQueued)
                return true;
            if (!Stage1MigrationRequired)
                return true;
            if (!Stage1QueriedDuringStartup)
                throw new InvalidOperationException("RealmsFromACESetup.QueryStage1 must be executed first");

            ValidateRunnable();

            if (!HouseManager.Initialized)
                throw new InvalidOperationException("RealmsFromACESetup.RunStage1IfRequired must start after the HouseManager is initialized.");
            if (RealmManager.Realms == null || RealmManager.Realms.Count == 0)
                throw new InvalidOperationException("RealmsFromACESetup.RunStage1IfRequired must start after RealmManager is initialized.");

            return DoSetupStage1();
        }

        private static bool RunStage2IfRequired()
        {
            if (!IsStage2Complete)
                return DoSetupStage2();
            return true;
        }

        private static bool DoSetupStage1()
        {
            if (!ACRealmsConfigManager.Config.CharacterMigrationOptions.AutoAssignHomeRealm)
                throw new ConfigurationErrorsException(" !!!!!  Detected players without a home realm !!!!! - Config.realms.js must set CharacterMigrationOptions.AutoAssignHomeRealm to true, and AutoAssignToRealm to a valid realm. These characters will be automatically assigned to that realm and their housing will be moved.");

            var realmname = ACRealmsConfigManager.Config.CharacterMigrationOptions.AutoAssignToRealm;
            var realm = RealmManager.GetRealmByName(realmname, false);
            if (realm == null)
                throw new ConfigurationErrorsException("Config.realms.js defined an Invalid Realm for CharacterMigrationOptions.AutoAssignToRealm");

            Command.Handlers.ACRealms.ACRealmsAdminCommands.HandleMassTransferHomeRealm(null, $"0,{realm.Realm.Id}");

            if (!IsStage1Complete)
            {
                var players = PlayerManager.GetAllPlayers().Where(x => (x.GetProperty(ACE.Entity.Enum.Properties.PropertyInt.HomeRealm) ?? 0) == 0);
                Console.WriteLine("Migration did not complete successfully. The following players still show a null home realm:");
                foreach (var player in players)
                {
                    Console.WriteLine($"{player.Name}: {player.GetProperty(ACE.Entity.Enum.Properties.PropertyInt.HomeRealm)?.ToString() ?? "null"}");
                }
                Console.WriteLine("Warning: Failed to complete RealmsFromACESetup!");
                ServerManager.DoShutdownNow();
                return false;
            }

            return TryCompleteStage1();
        }

        // Forces removal of DB slumlords in case they somehow weren't removed during stage 1
        private static bool DoSetupStage2()
        {
            HouseManager.PauseForACEMigrationFinalization();

            var biotas = UndeadSlumLords.ToList();
            var guids = biotas.Select(biota => new ObjectGuid(biota.Id));
            var landblockKeys = guids.Select(x => (x.StaticObjectLandblock.Value, x.Instance.Value)).Distinct().ToList();

            // Be really sure the landblock is not there
            foreach(var (lbid, iid) in landblockKeys)
            {
                var lbidQualified = new LandblockId(lbid);

                if (LandblockManager.IsLoaded(lbidQualified, iid))
                {
                    var lb = LandblockManager.GetLandblockUnsafe(lbidQualified, iid);
                    if (lb != null)
                        throw new InvalidOperationException("Found unexpected landblock still loaded during Stage 2 ACE Migration.");
                }
            }

            foreach (var biota in biotas)
            {
                DatabaseManager.Shard.BaseDatabase.RemoveBiota(biota.Id);
            }

            return true;
        }

        private static bool TryCompleteStage1()
        {
            var complete = IsStage1Complete;
            if (complete)
                return CompleteStage1();
            else
                return false;
        }

        private static bool CompleteStage1()
        {
            LandblockManager.UnloadingAfterACEMigration = true;
            
            while (true)
            {
                System.Threading.Thread.Sleep(2000);
                if (!LandblockManager.UnloadingAfterACEMigration)
                    break;
                Console.WriteLine("Waiting for housing landblocks to unload after migration...");
            }

            if (!IsStage1Complete)
            {
                Console.WriteLine("Internal error: ACE Migration Stage 1 did not complete successfully");
                ServerManager.DoShutdownNow();
                return false;
            }

            return true;
        }

        private static void SignalMigrationComplete()
        {
            Console.WriteLine("Congratulations! The server is ready to use AC Realms.");
            PropertyManager.ModifyBool("acrsystem_enable_ace_migration_mode_at_next_startup", false);
            ACE.Entity.ACRealms.RealmsFromACESetupHelper.UnsafeInstanceIDTemporarilyAllowed = false;
            Stage1MigrationRequired = false;
        }

        private static bool IsMigrationComplete => IsStage1Complete && IsStage2Complete;

        private static bool IsStage1Complete => !NullHomeRealmPlayersPresent();
        private static bool NullHomeRealmPlayersPresent() =>
            PlayerManager.GetAllPlayers().Any(x => (x.GetProperty(ACE.Entity.Enum.Properties.PropertyInt.HomeRealm) ?? 0) == 0);

        private static IEnumerable<Database.Models.Shard.Biota> UndeadSlumLords =>
            DatabaseManager.Shard.BaseDatabase.GetBiotasByType(WeenieType.SlumLord).Where(x => new ObjectGuid(x.Id).Instance == null || new ObjectGuid(x.Id).Instance == 0);

        private static bool IsStage2Complete => !UndeadSlumLords.Any();
    }
}
