using ACE.Database;
using ACE.Database.Models.Auth;
using ACE.Entity.Enum;
using ACE.Server.Tests.Helpers.Database;
using EntityFrameworkCoreMock.NSubstitute;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace ACE.Server.Tests
{
    public class DatabaseMock
    {
        public AuthenticationDatabase Auth;

        /// <summary>
        /// Intended to run once before each test via the test class constructor
        /// </summary>
        public DatabaseMock()
        {
            Initialization.InitializeForTests();

            //Auth DB
            var options = new DbContextOptions<AuthDbContext>();
            var builder = new DbContextOptionsBuilder<AuthDbContext>(options)
                .UseSqlite($"file::memory:");

            var mock = new DbContextMock<AuthDbContext>(builder.Options);
            mock.CreateDbSetMock<Account>(ctx => ctx.Account, (e, a) => { e.AccountId = (uint)a.NextIdentity; return e.AccountId; }, DatabaseSeed.AuthAccount);

            Auth = new AuthenticationDatabase(() => mock.Object);
            Auth.CreateAccount("testaccount1", "testpassword1", AccessLevel.Player, IPAddress.Parse("127.0.0.1"));
        }
    }
}
