using ACE.Entity.Enum;

namespace ACE.Server.Network.GameMessages.Messages
{
    public class GameMessageAdminEnvirons : GameMessage
    {
        public GameMessageAdminEnvirons(ISession session, EnvironChangeType environChange = EnvironChangeType.Clear)
            : base(GameMessageOpcode.AdminEnvirons, GameMessageGroup.UIQueue, 8)
        {
            Writer.Write((uint)environChange);
        }
    }
}
