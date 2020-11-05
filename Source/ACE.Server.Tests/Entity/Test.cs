using ACE.Database;
using ACE.Database.Models.Auth;
using ACE.Entity.Enum;
using EntityFrameworkCoreMock.NSubstitute;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Xunit;

namespace ACE.Server.Tests.Entity
{
    public class EntityTest
    {
        DatabaseMock db = new DatabaseMock();

        public EntityTest()
        {
            Initialization.InitializeForTests();
        }

        [Fact]
        public void CreateAccount_GetAccountByName_ReturnsAccount()
        {
            var newAccount = db.Auth.CreateAccount("testaccount3", "testpassword3", AccessLevel.Player, IPAddress.Parse("127.0.0.1"));
            var results = db.Auth.GetAccountByName(newAccount.AccountName);
            Assert.NotNull(results);
            Assert.True(results.AccessLevel == (uint)AccessLevel.Player);
        }

        [Fact]
        public void UpdateAccountAccessLevelToSentinelAndBackToPlayer_ReturnsAccount()
        {
            var newAccount = db.Auth.GetAccountByName("testaccount1");

            db.Auth.UpdateAccountAccessLevel(newAccount.AccountId, AccessLevel.Sentinel);
            var results = db.Auth.GetAccountByName(newAccount.AccountName);
            Assert.NotNull(results);
            Assert.True(results.AccessLevel == (uint)AccessLevel.Sentinel);

            db.Auth.UpdateAccountAccessLevel(newAccount.AccountId, AccessLevel.Player);
            var results2 = db.Auth.GetAccountByName(newAccount.AccountName);
            Assert.NotNull(results2);
            Assert.True(results2.AccessLevel == (uint)AccessLevel.Player);
        }

        [Fact]
        public void GetAccountIdByName_ReturnsAccount()
        {
            
            Account newAccount = new Account();
            newAccount.AccountName = "testaccount1";

            var id = db.Auth.GetAccountIdByName(newAccount.AccountName);
            var results = db.Auth.GetAccountById(id);
            Assert.NotNull(results);
            Assert.True(results.AccountId == id);
            Assert.True(results.AccountName == newAccount.AccountName);
        }

        [Fact]
        public void GetAccountByName_TestPassword_ReturnsMatch()
        {
            var results = db.Auth.GetAccountByName("testaccount1");
            Assert.NotNull(results);
            Assert.True(results.PasswordMatches("testpassword1"));
        }

        [Fact]
        public void GetAccountByName_TestPassword_ReturnsNoMatch()
        {
            var results = db.Auth.GetAccountByName("testaccount1");
            Assert.NotNull(results);
            Assert.False(results.PasswordMatches("testpassword2"));
        }
    }
}
