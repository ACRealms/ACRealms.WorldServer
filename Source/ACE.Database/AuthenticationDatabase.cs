using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;

using log4net;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using ACE.Database.Models.Auth;
using ACE.Entity.Enum;
using System.Collections.Generic;
using System;
using System.Net;

namespace ACE.Database
{

    public interface IAuthenticationDatabase
    {
        bool Exists(bool retryUntilFound);
        Account CreateAccount(string name, string password, AccessLevel accessLevel, IPAddress address);
        Account GetAccountById(uint accountId);
        int GetAccountCount();
        Account GetAccountByName(string accountName);
        List<string> GetListofAccountsByAccessLevel(AccessLevel accessLevel);
        void UpdateAccount(Account account);
        uint GetAccountIdByName(string accountName);
        List<string> GetListofBannedAccounts();
        bool UpdateAccountAccessLevel(uint accountId, AccessLevel accessLevel);
    }

    public class AuthenticationDatabase : IAuthenticationDatabase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Func<AuthDbContext> contextFactory;

        public AuthenticationDatabase()
        {
            contextFactory = () => new AuthDbContext();
        }

        public AuthenticationDatabase(Func<AuthDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public bool Exists(bool retryUntilFound)
        {
            var config = Common.ConfigManager.Config.MySql.Authentication;

            for (; ; )
            {
                using (var context = contextFactory())
                {
                    if (((RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>()).Exists())
                    {
                        log.Debug($"Successfully connected to {config.Database} database on {config.Host}:{config.Port}.");
                        return true;
                    }
                }

                log.Error($"Attempting to reconnect to {config.Database} database on {config.Host}:{config.Port} in 5 seconds...");

                if (retryUntilFound)
                    Thread.Sleep(5000);
                else
                    return false;
            }
        }


        public int GetAccountCount()
        {
            using (var context = contextFactory())
                return context.Account.Count();
        }

        /// <exception cref="MySqlException">Account with name already exists.</exception>
        public Account CreateAccount(string name, string password, AccessLevel accessLevel, IPAddress address)
        {
            var account = new Account();

            account.AccountName = name;
            account.SetPassword(password);
            account.SetSaltForBCrypt();
            account.AccessLevel = (uint)accessLevel;

            account.CreateTime = DateTime.UtcNow;
            account.CreateIP = address.GetAddressBytes();

            using (var context = contextFactory())
            {
                context.Account.Add(account);

                context.SaveChanges();
            }

            return account;
        }

        /// <summary>
        /// Will return null if the accountId was not found.
        /// </summary>
        public Account GetAccountById(uint accountId)
        {
            using (var context = contextFactory())
            {
                return context.Account
                    .AsNoTracking()
                    .FirstOrDefault(r => r.AccountId == accountId);
            }
        }

        /// <summary>
        /// Will return null if the accountName was not found.
        /// </summary>
        public Account GetAccountByName(string accountName)
        {
            using (var context = contextFactory())
            {
                return context.Account
                    .AsNoTracking()
                    .FirstOrDefault(r => r.AccountName == accountName);
            }
        }

        /// <summary>
        /// id will be 0 if the accountName was not found.
        /// </summary>
        public uint GetAccountIdByName(string accountName)
        {
            using (var context = contextFactory())
            {
                var result = context.Account
                    .AsNoTracking()
                    .FirstOrDefault(r => r.AccountName == accountName);

                return (result != null) ? result.AccountId : 0;
            }
        }

        public void UpdateAccount(Account account)
        {
            using (var context = contextFactory())
            {
                context.Entry(account).State = EntityState.Modified;

                context.SaveChanges();
            }
        }

        public bool UpdateAccountAccessLevel(uint accountId, AccessLevel accessLevel)
        {
            using (var context = contextFactory())
            {
                var account = context.Account
                    .First(r => r.AccountId == accountId);

                if (account == null)
                    return false;

                account.AccessLevel = (uint)accessLevel;

                context.SaveChanges();
            }

            return true;
        }

        public List<string> GetListofAccountsByAccessLevel(AccessLevel accessLevel)
        {
            using (var context = contextFactory())
            {
                var results = context.Account
                    .AsNoTracking()
                    .Where(r => r.AccessLevel == Convert.ToUInt32(accessLevel)).ToList();

                var result = new List<string>();
                foreach (var account in results)
                    result.Add(account.AccountName);

                return result;
            }
        }

        public List<string> GetListofBannedAccounts()
        {
            using (var context = contextFactory())
            {
                var results = context.Account
                    .AsNoTracking()
                    .Where(r => r.BanExpireTime > DateTime.UtcNow).ToList();

                var result = new List<string>();
                foreach (var account in results)
                {
                    var bannedbyAccount = account.BannedByAccountId.Value > 0 ? $"account {GetAccountById(account.BannedByAccountId.Value).AccountName}" : "CONSOLE";
                    result.Add($"{account.AccountName} -- banned by {bannedbyAccount} until server time {account.BanExpireTime.Value.ToLocalTime():MMM dd yyyy  h:mmtt}{(!string.IsNullOrWhiteSpace(account.BanReason) ? $" -- Reason: {account.BanReason}" : "")}");
                }

                return result;
            }
        }
    }
}
