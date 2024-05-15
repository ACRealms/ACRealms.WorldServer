using ACE.Database;
using ACE.Database.Models.Auth;
using ACE.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Tests.Factories
{
    internal record AccountFactory : Factory<Account, AccountFactory>
    {
        public string AccountName { get; init; }
        public string AccountPassword { get; init; }
        public AccessLevel AccessLevel { get; init; }

        public AccountFactory() { }

        protected override Func<Account> Builder() => () =>
        {
            return DatabaseManager.Authentication.CreateAccount(AccountName ?? $"factoryaccount{CurrentIndex}",
                AccountPassword ?? $"password",
                AccessLevel,
                IPAddress.Parse("127.0.0.1"));
        };
    }
}
