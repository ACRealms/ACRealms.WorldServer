using System;
using System.IO;
using ACE.Common;
using ACE.Database.Models.Auth;
using ACE.Entity.Enum;
using System.Net;
using ACE.Database;
using Xunit;

namespace ACRealms.Tests.Database
{
    [CollectionDefinition("Dependency Injection")]
    public class AccountTests : IDisposable
    {
        AuthenticationDatabase authDb { get; } = DatabaseManager.Authentication;

        Account account;
        public AccountTests()
        {
            account = authDb.CreateAccount("testaccount1", "testpassword1", AccessLevel.Player, IPAddress.Parse("127.0.0.1"));
        }

        public void Dispose()
        {
            authDb.DeleteAccount(account);
        }

        [Fact]
        public void CreateAccount_GetAccountByName_ReturnsAccount()
        {
            var results = authDb.GetAccountByName(account.AccountName);
            Assert.NotNull(results);
            Assert.True(results.AccessLevel == (uint)AccessLevel.Player);
        }

        [Fact]
        public void UpdateAccountAccessLevelToSentinelAndBackToPlayer_ReturnsAccount()
        {
            Account newAccount = authDb.CreateAccount("testaccount02", "testpassword2", AccessLevel.Player, IPAddress.Parse("127.0.0.1"));
            newAccount.AccountName = "testaccount2";
            authDb.UpdateAccount(newAccount);

            authDb.UpdateAccountAccessLevel(newAccount.AccountId, AccessLevel.Sentinel);
            var results = authDb.GetAccountByName(newAccount.AccountName);
            Assert.NotNull(results);
            Assert.True(results.AccessLevel == (uint)AccessLevel.Sentinel);

            authDb.UpdateAccountAccessLevel(newAccount.AccountId, AccessLevel.Player);
            var results2 = authDb.GetAccountByName(newAccount.AccountName);
            Assert.NotNull(results2);
            Assert.True(results2.AccessLevel == (uint)AccessLevel.Player);
        }

        [Fact]
        public void GetAccountIdByName_ReturnsAccount()
        {
            var id = authDb.GetAccountIdByName(account.AccountName);
            var results = authDb.GetAccountById(id);
            Assert.NotNull(results);
            Assert.Equal(account.AccountId, id);
            Assert.Equal(id, results.AccountId);
            Assert.Equal(account.AccountName, results.AccountName);
            Assert.True(results.PasswordMatches("testpassword1"));
            Assert.False(results.PasswordMatches("testpassword2"));
        }
    }
}
