using ACE.Common.ACRealms;
using ACE.Server.Network.Managers;
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
        public static bool ACEMigrationInProgress => PropertyManager.GetBool("acrsystem_enable_ace_migration_mode_at_next_startup", true).Item;
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
            if (!ACEMigrationInProgress)
                return;

            ValidateRunnable();

            if (HouseManager.Initialized)
                throw new InvalidOperationException("RealmsFromACESetup Stage 1 must start before the HouseManager is initialized.");
            if (!PropertyManager.Initialized)
                throw new InvalidOperationException("RealmsFromACESetup Stage 1 must start after the PropertyManager is initialized.");

            if (!TryMarkMigrationComplete())
            {
                Stage1MigrationRequired = true;
                ACE.Entity.ACRealms.RealmsFromACESetupHelper.UnsafeInstanceIDTemporarilyAllowed = true;
            }

            return;
        }

        public static void RunStage1IfRequired()
        {
            Stage1QueriedDuringStartup = true;
            if (!ACEMigrationInProgress)
                return;
            if (!Stage1MigrationRequired)
                return;
            if (!Stage1QueriedDuringStartup)
                throw new InvalidOperationException("RealmsFromACESetup.QueryStage1 must be executed first");

            ValidateRunnable();

            if (!HouseManager.Initialized)
                throw new InvalidOperationException("RealmsFromACESetup.RunStage1IfRequired must start after the HouseManager is initialized.");
            if (RealmManager.Realms == null || RealmManager.Realms.Count == 0)
                throw new InvalidOperationException("RealmsFromACESetup.RunStage1IfRequired must start after RealmManager is initialized.");
            if (NetworkManager.Instance != null)
                throw new InvalidOperationException("RealmsFromACESetup.RunStage1IfRequired must start before the NetworkManager is initialized.");

            DoSetupStage1();

            return;
        }

        private static void DoSetupStage1()
        {
            if (!ACRealmsConfigManager.Config.CharacterMigrationOptions.AutoAssignHomeRealm)
                throw new ConfigurationErrorsException(" !!!!!  Detected players without a home realm !!!!! - Config.realms.js must set CharacterMigrationOptions.AutoAssignHomeRealm to true, and AutoAssignToRealm to a valid realm. These characters will be automatically assigned to that realm and their housing will be moved.");

            var realmname = ACRealmsConfigManager.Config.CharacterMigrationOptions.AutoAssignToRealm;
            var realm = RealmManager.GetRealmByName(realmname, false);
            if (realm == null)
                throw new ConfigurationErrorsException("Config.realms.js defined an Invalid Realm for CharacterMigrationOptions.AutoAssignToRealm");

            Command.Handlers.ACRealms.ACRealmsAdminCommands.HandleMassTransferHomeRealm(null, $"0,{realm.Realm.Id}");

            if (!TryMarkMigrationComplete())
            {
                var players = PlayerManager.GetAllPlayers().Where(x => (x.GetProperty(ACE.Entity.Enum.Properties.PropertyInt.HomeRealm) ?? 0) == 0);
                Console.WriteLine("Migration did not complete successfully. The following players still show a null home realm:");
                foreach (var player in players)
                {
                    Console.WriteLine($"{player.Name}: {player.GetProperty(ACE.Entity.Enum.Properties.PropertyInt.HomeRealm)?.ToString() ?? "null"}");
                }
                throw new InvalidOperationException("Failed to complete RealmsFromACESetup.");
            }
        }

        private static bool TryMarkMigrationComplete()
        {
            var complete = IsMigrationComplete();
            if (complete)
                MarkMigrationComplete();
            return complete;
        }

        private static void MarkMigrationComplete()
        {
            Console.WriteLine("Congratulations! The server is ready to use AC Realms.");
            PropertyManager.ModifyBool("acrsystem_enable_ace_migration_mode_at_next_startup", false);
            ACE.Entity.ACRealms.RealmsFromACESetupHelper.UnsafeInstanceIDTemporarilyAllowed = false;
            Stage1MigrationRequired = false;
        }

        private static bool IsMigrationComplete() => !NullHomeRealmPlayersPresent();

        private static bool NullHomeRealmPlayersPresent() =>
            PlayerManager.GetAllPlayers().Any(x => (x.GetProperty(ACE.Entity.Enum.Properties.PropertyInt.HomeRealm) ?? 0) == 0);
    }
}
