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
    public class AccountTests
    {
        AuthenticationDatabase authDb { get; } = DatabaseManager.Authentication;

        [Fact]
        public void CreateAccount_GetAccountByName_ReturnsAccount()
        {
            var newAccount = authDb.CreateAccount("testaccount1", "testpassword1", AccessLevel.Player, IPAddress.Parse("127.0.0.1"));

            var results = authDb.GetAccountByName(newAccount.AccountName);
            Assert.NotNull(results);
            Assert.True(results.AccessLevel == (uint)AccessLevel.Player);
        }

        [Fact]
        public void UpdateAccountAccessLevelToSentinelAndBackToPlayer_ReturnsAccount()
        {
            Account newAccount = new Account();
            newAccount.AccountName = "testaccount1";

            authDb.UpdateAccountAccessLevel(1, AccessLevel.Sentinel);
            var results = authDb.GetAccountByName(newAccount.AccountName);
            Assert.NotNull(results);
            Assert.True(results.AccessLevel == (uint)AccessLevel.Sentinel);

            authDb.UpdateAccountAccessLevel(1, AccessLevel.Player);
            var results2 = authDb.GetAccountByName(newAccount.AccountName);
            Assert.NotNull(results2);
            Assert.True(results2.AccessLevel == (uint)AccessLevel.Player);
        }

        [Fact]
        public void GetAccountIdByName_ReturnsAccount()
        {
            Account newAccount = new Account();
            newAccount.AccountName = "testaccount1";

            var id = authDb.GetAccountIdByName(newAccount.AccountName);
            var results = authDb.GetAccountById(id);
            Assert.NotNull(results);
            Assert.True(results.AccountId == id);
            Assert.True(results.AccountName == newAccount.AccountName);
        }

        [Fact]
        public void GetAccountByName_TestPassword_ReturnsMatch()
        {
            var results = authDb.GetAccountByName("testaccount1");
            Assert.NotNull(results);
            Assert.True(results.PasswordMatches("testpassword1"));
        }

        [Fact]
        public void GetAccountByName_TestPassword_ReturnsNoMatch()
        {
            var results = authDb.GetAccountByName("testaccount1");
            Assert.NotNull(results);
            Assert.False(results.PasswordMatches("testpassword2"));
        }
    }
}
