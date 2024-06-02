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
        public string HomeRealm { get; init; }

        /// <summary>
        /// False is not supported yet. Using multiple realm fixtures means that a new landblock must be loaded or the old fixture will be used.
        /// </summary>
        public bool UseUniqueInstanceID { get; } = true;
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
            if (UseUniqueInstanceID)
                player.SetProperty(ACE.Entity.Enum.Properties.PropertyBool.AttemptUniqueInstanceID, true);
            if (HomeRealm != null)
                player.HomeRealm = RealmManager.GetRealmByName(HomeRealm, includeRulesets: false).Realm.Id;

            session.WaitForMessage<TraceMessageEnterWorldComplete>();
            if (HomeRealm != null)
                session.WaitForPlayerState(p => p.CurrentLandblock?.WorldRealmID == player.HomeRealm && p.CurrentLandblock.RealmRuleset.Realm.Name == HomeRealm, timeoutInSeconds: 60, checkIntervalMs: 50);

            return player;
        };
    }
}
