
using ACE.Entity.Enum;
using ACE.Server.WorldObjects;
using System.Linq;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventViewContents : GameEventMessage
    {
        public GameEventViewContents(ISession session, Container container)
            : base(GameEventType.ViewContents, GameMessageGroup.UIQueue, session)
        {
            Writer.Write(container.Guid.ClientGUID);

            Writer.Write((uint)container.Inventory.Count);
            foreach (var inv in container.Inventory.Values.OrderBy(x => x.PlacementPosition))
            {
                Writer.Write(inv.Guid.ClientGUID);

                if (inv.WeenieType == WeenieType.Container)
                    Writer.Write((uint)ContainerType.Container);
                else if (inv.RequiresPackSlot)
                    Writer.Write((uint)ContainerType.Foci);
                else
                    Writer.Write((uint)ContainerType.NonContainer);
            }
        }
    }
}
