using ACE.Database.Models.Auth;
using ACE.Database.Models.Shard;
using ACE.Entity;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.WorldObjects;
using ACRealms.Tests.Fixtures.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Tests.Factories
{
    internal class OnlinePlayerFactory : Factory<Player, OnlinePlayerFactory>
    {
        public Account Account { get; init; }
        public Character Character { get; init; }
        public FakeSession Session { get; init; }

        public OnlinePlayerFactory() { }

        public override Func<Player> Builder() => () =>
        {
            var account = Account ?? new AccountFactory().Create();
            var session = Session ?? new FakeSessionFactory() { Account = account }.Create();
            var character = Character ?? new CharacterFactory() { Account = account, Session = session }.Create();
            WorldManager.PlayerInitForWorld(session, new ObjectGuid(character.Id).ClientGUID, session.Account);
            var message = session.WaitForMessage<GameMessagePlayerCreate>();

            var player = PlayerManager.GetOnlinePlayer(message.Guid);
            if (player == null)
                throw new Exception("Player not found");

            return player;
        };
    }
}
