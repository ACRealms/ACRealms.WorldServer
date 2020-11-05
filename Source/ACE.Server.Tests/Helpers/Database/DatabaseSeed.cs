using ACE.Database.Models.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACE.Server.Tests.Helpers.Database
{
    public static class DatabaseSeed
    {
        public static List<Account> AuthAccount = new List<Account>
        {
            new Account{ AccountName = "RF1", AccountId = 1 },
            new Account{ AccountName = "RF2", AccountId = 2 },
        };
    }
}
