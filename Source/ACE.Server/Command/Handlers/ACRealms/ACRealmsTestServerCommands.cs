using ACE.Database;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Models;
using ACE.Server.Entity;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Realms;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace ACE.Server.Command.Handlers.ACRealms
{
    public static class ACRealmsTestServerCommands
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

        //acr-test-create-accounts masstest test 1 100 Masstest
        //ACHeadlessClient.exe -d C:\ACE\Dats -u masstest -p test -c "[RealmSelector] Masstest" -h 127.0.0.1 --port 9000 --stresstestmin=1 --stresstestmax=100
        [CommandHandler("acr-test-create-accounts", AccessLevel.Admin, CommandHandlerFlag.None, 5,
            "Creates stress test accounts.",
            "Account Prefix, Password, Account Min Index(0-9999), Account Max Index (0-9999), Character Name Prefix (10-20 char limit)")]
        public static void HandleACRTestCreateAccounts(ISession session, params string[] parameters)
        {
            if (!Common.ACRealms.ACRealmsConfigManager.Config.TestServer)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Creating stress testing accounts requires TestServer=true in Config.realms.js.", ChatMessageType.Broadcast);
                return;
            }

            var accountPrefix = parameters[0].Trim();
            var password = parameters[1].Trim();
            var minIndex = uint.Parse(parameters[2]);
            var maxIndex = uint.Parse(parameters[3]);
            var characterPrefix = string.Join(" ", parameters.Skip(4).ToList()).Trim();

            if (minIndex > 9999 || maxIndex > 9999)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Account index must be from 0-9999", ChatMessageType.Broadcast);
                return;
            }

            if (minIndex > maxIndex)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Account min index must be <= max index", ChatMessageType.Broadcast);
                return;
            }

            if (!Regex.IsMatch(characterPrefix, "^[a-zA-Z]+([-' ]?[a-zA-Z]+)*$"))
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Invalid Character Name Prefix, must be a valid AC character name. {characterPrefix}", ChatMessageType.Broadcast);
                return;
            }
            if (characterPrefix.Length < 1)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Invalid Character Name Prefix.", ChatMessageType.Broadcast);
                return;
            }

            if (characterPrefix.Length > 10 && minIndex > 1000)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Character name prefixes shall be no more than 10 characters if the index is > 1000.", ChatMessageType.Broadcast);
                return;
            }

            if (characterPrefix.Length > 20)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Character name prefixes shall be no more than 20 characters.", ChatMessageType.Broadcast);
                return;
            }

            if (!Regex.IsMatch(accountPrefix, "^[a-z]+$"))
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Invalid Account Prefix, must be lowercase letters only. You Entered: {accountPrefix}", ChatMessageType.Broadcast);
                return;
            }
            if (accountPrefix.Length < 1 || accountPrefix.Length > 40)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Invalid Account Name Prefix {accountPrefix}: must be between 1 and 40 characters.", ChatMessageType.Broadcast);
                return;
            }

            if (!Regex.IsMatch(password, "^[a-zA-Z0-9]+$"))
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Passwords for stress test accounts may only use alphanumeric characters.", ChatMessageType.Broadcast);
                return;
            }

            if (password.Length < 1 || password.Length > 40)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Passwords for stress test accounts must be between 1 and 40 characters.", ChatMessageType.Broadcast);
                return;
            }

            static string Roman(uint n)
            {
                
                var s = ""; uint[] v = [1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1];
                for (var i = 0; i < 13; i++)
                {
                    while (n >= v[i])
                    {
                        n -= v[i];
                        s += "M CM D CD C XC L XL X IX V IV I".Split()[i];
                    }
                }
                return s;
            }

            static string FullCharName(string prefix, uint idx) => idx == 0 ? prefix : $"{prefix} {Roman(idx)}";
            static string FullAccountName(string prefix, uint idx) => $"{prefix}{idx.ToString().PadLeft(4, '0')}";

            var accountNames = DatabaseManager.Authentication.GetAccountNamesStartingWithPrefix(accountPrefix);
            foreach (var name in accountNames)
            {
                if (name == accountPrefix)
                    continue;

                var suffix = name.Substring(accountPrefix.Length);
                if (uint.TryParse(suffix, out uint idx))
                {
                    if (idx >= minIndex && idx <= maxIndex)
                    {
                        if (FullAccountName(accountPrefix, idx) != name)
                            continue;

                        CommandHandlerHelper.WriteOutputInfo(session, $"Account {name} is already in use. Choose a different minIndex and maxIndex that is not " +
                            $"inclusive of {idx}, or choose a differnt account prefix.", ChatMessageType.Broadcast);
                        return;
                    }
                }
            }

            uint currentIdx = minIndex;
            try
            {
                var weenie = DatabaseManager.World.GetCachedWeenie("human");

                for (currentIdx = minIndex; currentIdx <= maxIndex; currentIdx++)
                {
                    var accountName = FullAccountName(accountPrefix, currentIdx);
                    var charName = FullCharName(characterPrefix, currentIdx);
                    CommandHandlerHelper.WriteOutputInfo(session, $"Creating account {accountName} with character {charName}.", ChatMessageType.Broadcast);
                    var account = DatabaseManager.Authentication.CreateAccount(accountName, password, AccessLevel.Player, System.Net.IPAddress.Parse("127.0.0.1"));
                    var guid = GuidManager.NewPlayerGuid();
                    var player = Factories.PlayerFactoryEx.Create275HeavyWeapons(weenie, guid, account.AccountId, charName);

                    PlayerManager.IsCharacterNameAvailableForCreation(charName).ContinueWith(characterNameTask =>
                    {
                        if (!characterNameTask.IsCompletedSuccessfully)
                        {
                            CommandHandlerHelper.WriteOutputInfo(session, $"Error querying DB.", ChatMessageType.Broadcast);
                            return;
                        }
                        var isAvailable = characterNameTask.Result;

                        if (!isAvailable)
                        {
                            CommandHandlerHelper.WriteOutputInfo(session, $"Character name in use.", ChatMessageType.Broadcast);
                            return;
                        }

                        var possessions = player.GetAllPossessions();
                        var possessedBiotas = new System.Collections.ObjectModel.Collection<(Biota biota, ReaderWriterLockSlim rwLock)>();
                        foreach (var possession in possessions)
                            possessedBiotas.Add((possession.Biota, possession.BiotaDatabaseLock));

                        // We must await here -- 
                        DatabaseManager.Shard.AddCharacterInParallel(player.Biota, player.BiotaDatabaseLock, possessedBiotas, player.Character, player.CharacterDatabaseLock, saveSuccess =>
                        {
                            if (!saveSuccess)
                            {
                                CommandHandlerHelper.WriteOutputInfo(session, $"Error saving character.", ChatMessageType.Broadcast);
                                return;
                            }

                            PlayerManager.AddOfflinePlayer(player);
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                CommandHandlerHelper.WriteOutputInfo(session, $"Unhandled error creating character: {ex.Message}");
                log.Error(ex.ToString());
                return;
            }

            CommandHandlerHelper.WriteOutputInfo(session, $"Finished!", ChatMessageType.Broadcast);
        }
    }
}
