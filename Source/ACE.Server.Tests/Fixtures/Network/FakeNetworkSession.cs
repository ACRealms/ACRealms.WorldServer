using ACE.Server.Network;
using ACE.Server.Network.GameMessages;
using ACE.Server.Network.Handlers;
using ACE.Server.Network.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Tests.Fixtures.Network
{
    public class FakeNetworkSession : NetworkSessionBase
    {
        public FakeSession FakeSession => (FakeSession)session;

        public FakeNetworkSession(ISession session, ushort clientId, ushort serverId)
            : base(session, clientId, serverId)
        {
        }

        protected override void SendPacketRaw(ServerPacket packet)
        {
        }

        public override void EnqueueSend(params GameMessage[] messages)
        {
            if (isReleased) // Session has been removed
                return;

            foreach (var message in messages)
                FakeSession.LogMessageSent(message);
        }

        public override void EnqueueSend(IEnumerable<GameMessage> messages)
        {
            if (isReleased) // Session has been removed
                return;

            foreach (var message in messages)
                FakeSession.LogMessageSent(message);
        }
    }
}
