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

        private static bool DefaultSelector<TMessage>(TMessage message)
            where TMessage : GameMessage => true;
        
        public TMessage WaitForMessage<TMessage>(Func<TMessage, bool> selector = null, double timeoutInSeconds = 1.0)
            where TMessage : GameMessage
        {
            selector ??= DefaultSelector;

            int timeout = System.Diagnostics.Debugger.IsAttached ? int.MaxValue : (int)(timeoutInSeconds * 1000);

            Exception ex = null;
            var task = Task.Run(() =>
            {
                var type = typeof(TMessage);
                while (!MessagesSent.ContainsKey(type))
                    Thread.Sleep(10);
                do
                {
                    while(!MessagesSent[type].IsEmpty)
                    {
                        MessagesSent[type].TryDequeue(out var message);
                        if (selector((TMessage)message))
                            return (TMessage)message;
                    }
                    Thread.Sleep(10);
                } while (true);
            });
            task.Wait(timeout);

            if (!task.IsCompletedSuccessfully)
                throw new GameMessageNotSentException();

            if (ex != null)
                throw ex;

            return task.Result;
        }

        public void WaitForPlayerState(Func<Player, bool> selector, double timeoutInSeconds = 1.0, uint checkIntervalMs = 10)
        {
            if (Player == null)
                throw new InvalidOperationException("Player not found on session");
            int timeout = System.Diagnostics.Debugger.IsAttached ? int.MaxValue : (int)(timeoutInSeconds * 1000);

            Exception ex = new InvalidOperationException("The task did not complete successfully");
            var task = Task.Run(() =>
            {
                try
                {
                    do
                    {
                        if (Player == null)
                            throw new InvalidOperationException("Player not found on session");

                        if (selector(Player))
                        {
                            ex = null;
                            return;
                        }
                        Thread.Sleep((int)checkIntervalMs);
                    }
                    while (true);
                }
                catch (Exception e)
                {
                    ex = e;
                    return;
                }
            });
            task.Wait(timeout);

            if (ex != null)
                throw ex;
        }
    }
}
