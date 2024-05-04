using ACE.Database.Models.Auth;
using ACE.Entity.Enum;
using ACE.Server.Network;
using ACRealms.Tests.Fixtures.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Tests.Factories
{
    internal class FakeSessionFactory : Factory<FakeSession, FakeSessionFactory>
    {
        internal static uint? SessionRequestingForAccount { get; private set; }

        public Account Account { get; init; }

        public override Func<FakeSession> Builder() => () =>
        {
            var account = Account ?? AccountFactory.Make();

            return MakeTestSession(account);
        };

        private static FakeSession MakeTestSession(Account account)
        {
            try
            {
                SessionRequestingForAccount = account.AccountId;
                var session = (FakeSession)FakeNetworkManager.Instance.FindOrCreateSession(null, null);
                session.SetAccount(account.AccountId, account.AccountName, (AccessLevel)account.AccessLevel);
                return session;
            }
            finally
            {
                SessionRequestingForAccount = null;
            }
        }
    }
}
