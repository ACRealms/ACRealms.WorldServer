using ACE.Database.Models.Auth;
using ACE.Entity.Enum;
using ACRealms.Tests.Fixtures.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ACRealms.Tests.Factories
{
    internal record FakeSessionFactory : Factory<FakeSession, FakeSessionFactory>
    {
        internal static ThreadLocal<uint?> SessionRequestingForAccount { get; private set; } = new ThreadLocal<uint?>();

        public AccountFactory Account { get; init; } = new AccountFactory();

        protected override Func<FakeSession> Builder() => () =>
        {
            var account = Account.Create();

            return MakeTestSession(account);
        };

        private static FakeSession MakeTestSession(Account account)
        {
            try
            {
                SessionRequestingForAccount.Value = account.AccountId;
                var session = (FakeSession)FakeNetworkManager.Instance.FindOrCreateSession(null, null);
                session.SetAccount(account.AccountId, account.AccountName, (AccessLevel)account.AccessLevel);
                return session;
            }
            finally
            {
                SessionRequestingForAccount.Value = null;
            }
        }
    }
}
