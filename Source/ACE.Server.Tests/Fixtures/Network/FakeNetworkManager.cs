using ACE.Server.Entity;
using ACE.Server.Entity.Actions;
using ACE.Server.Network;
using ACE.Server.Network.Enum;
using ACE.Server.Network.Managers;
using ACE.Server.WorldObjects;
using ACRealms.Tests.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Tests.Fixtures.Network
{
    public class FakeNetworkManager : NetworkManagerBase, INetworkManager
    {
        public static FakeNetworkManager Instance => (FakeNetworkManager)NetworkManager.Instance;
        
        protected override ISession MakeSession(ConnectionListener _connectionListener, IPEndPoint endPoint, ushort clientID)
        {
            return new FakeSession(null, clientID, ServerId);
        }

        public override ISession FindOrCreateSession(ConnectionListener connectionListener, IPEndPoint endPoint)
        {
            if (FakeSessionFactory.SessionRequestingForAccount.Value == null)
                throw new InvalidOperationException("Must be called from FakeSessionFactory");
            var aid = FakeSessionFactory.SessionRequestingForAccount.Value;

            ISession session;

            sessionLock.EnterUpgradeableReadLock();
            try
            {
                session = sessionMap.SingleOrDefault(s => s != null && aid.Equals(s.AccountId));
                if (session == null)
                {
                    sessionLock.EnterWriteLock();
                    try
                    {
                        for (ushort i = 0; i < sessionMap.Length; i++)
                        {
                            if (sessionMap[i] == null)
                            {
                                log.DebugFormat("Creating new session for {0} with id {1}", aid, i);
                                session = MakeSession(connectionListener, endPoint, i);
                                sessionMap[i] = session;
                                break;
                            }
                        }
                    }
                    finally
                    {
                        sessionLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                sessionLock.ExitUpgradeableReadLock();
            }

            return session;
        }
    }

}
