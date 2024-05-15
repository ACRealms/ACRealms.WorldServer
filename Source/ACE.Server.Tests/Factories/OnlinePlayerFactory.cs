using ACE.Entity;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects;
using ACRealms.Server.Network.TraceMessages.Messages;
using System;

namespace ACRealms.Tests.Factories
{
    internal record OnlinePlayerFactory : Factory<Player, OnlinePlayerFactory>
    {
        public CharacterFactory Character { get; init; } = new CharacterFactory();
        public OnlinePlayerFactory() { }

        protected override Func<Player> Builder() => () =>
        {
            var character = Character.Create();
            var session = FakeSessionFactory.LastValue;
            WorldManager.PlayerInitForWorld(session, new ObjectGuid(character.Id).ClientGUID, session.Account);
            var message = session.WaitForMessage<GameMessagePlayerCreate>();

            var player = PlayerManager.GetOnlinePlayer(message.Guid);
            if (player == null)
                throw new Exception("Player not found");
            session.WaitForMessage<TraceMessageEnterWorldComplete>();

            return player;
        };
    }
}
