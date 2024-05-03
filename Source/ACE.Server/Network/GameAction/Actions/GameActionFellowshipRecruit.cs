using ACE.Server.Managers;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionFellowshipRecruit
    {
        [GameAction(GameActionType.FellowshipRecruit)]
        public static void Handle(ClientMessage message, ISession session)
        {
            uint newMemberGuid = message.Payload.ReadUInt32();
            var newPlayer = PlayerManager.GetOnlinePlayer(newMemberGuid);

            session.Player.FellowshipRecruit(newPlayer);
        }
    }
}
