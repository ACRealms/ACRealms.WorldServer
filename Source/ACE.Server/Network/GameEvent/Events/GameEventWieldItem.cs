using ACE.Entity.Enum;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventWieldItem : GameEventMessage
    {
        public GameEventWieldItem(ISession session, uint objectId, EquipMask newLocation)
            : base(GameEventType.WieldObject, GameMessageGroup.UIQueue, session, 12)
        {
            Writer.Write(objectId);
            Writer.Write((int)newLocation);
        }
    }
}
