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
    internal class FakeNetworkSession : INetworkSession
    {
        public ushort ClientId { get; }

        public ushort ServerId { get; }

        public long TimeoutTick { get; set; }
        public bool sendResync { get; set; }

        public SessionConnectionData ConnectionData => throw new NotImplementedException();

        public FakeNetworkSession(ISession session, ConnectionListener connectionListener, ushort clientId, ushort serverId)
        {
            ClientId = clientId;
            ServerId = serverId;

            // New network auth session timeouts will always be low.
            TimeoutTick = DateTime.UtcNow.AddSeconds(AuthenticationHandler.DefaultAuthTimeout).Ticks;
        }

        public void EnqueueSend(params GameMessage[] messages)
        {
        }

        public void EnqueueSend(params ServerPacket[] packets)
        {
        }

        public void ProcessPacket(ClientPacket packet)
        {
        }

        public void ReleaseResources()
        {
        }

        public void Update()
        {
        }
    }
}
