using ACE.Common.Performance;
using ACE.Common;
using ACE.Database;
using ACE.Database.Models.Shard;
using ACE.DatLoader;
using ACE.Entity.Enum;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Network.Enum;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.GameMessages;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Network.Managers;
using ACE.Server.WorldObjects;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ACRealms.Tests.Fixtures.Network
{
    public class GameMessageNotSentException : Exception { }

    public class FakeSession : SessionBase, ISession
    {
        // Here we guarantee that no IP address can be sent to
        public override IPEndPoint EndPointS2C
        {
            get => null;
            protected set { }
        }

        public ConcurrentDictionary<Type, ConcurrentQueue<GameMessage>> MessagesSent { get; } = new ConcurrentDictionary<Type, ConcurrentQueue<GameMessage>>();

        public FakeSession(IPEndPoint endPoint, ushort clientId, ushort serverId)
            : base(endPoint)
        {
            Network = new FakeNetworkSession(this, clientId, serverId);
        }

        public void LogMessageSent(GameMessage message)
        {
            var type = message.GetType();
            var queue = MessagesSent.GetOrAdd(type, new ConcurrentQueue<GameMessage>());
            queue.Enqueue(message);
        }

        public TMessage WaitForMessage<TMessage>(double timeoutInSeconds = 1.0)
            where TMessage : GameMessage
        {
            Exception ex = null;
            var task = Task.Run(() =>
            {
                var type = typeof(TMessage);
                while (!MessagesSent.ContainsKey(type) || MessagesSent[type].IsEmpty)
                {
                    Thread.Sleep(10);
                }
                MessagesSent[type].TryDequeue(out var message);
                if (message == null)
                {
                    ex = new InvalidOperationException("Another thread dequeued this session's message log. Ensure only one test thread is running for a given session.");
                    return null;
                }
                return message;
            });
            task.Wait((int)(timeoutInSeconds * 1000));

            if (!task.IsCompletedSuccessfully)
                throw new GameMessageNotSentException();

            if (ex != null)
                throw ex;

            return (TMessage)task.Result;
        }
    }
}
