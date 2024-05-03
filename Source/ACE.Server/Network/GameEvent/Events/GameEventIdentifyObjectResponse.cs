using ACE.Entity.Enum;
using ACE.Server.Network.Structure;
using ACE.Server.WorldObjects;

namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventIdentifyObjectResponse : GameEventMessage
    {
        public GameEventIdentifyObjectResponse(ISession session, WorldObject obj, bool success)
            : base(GameEventType.IdentifyObjectResponse, GameMessageGroup.UIQueue, session)
        {
            var appraiseInfo = new AppraiseInfo(obj, session.Player, success);

            Writer.Write(obj.Guid.ClientGUID);
            Writer.Write(appraiseInfo);
        }

        // Empty Appraisal response, for when you only have a guid and nothing else.
        public GameEventIdentifyObjectResponse(ISession session, uint objectGuid)
            : base(GameEventType.IdentifyObjectResponse, GameMessageGroup.UIQueue, session)
        {
            var appraiseInfo = new AppraiseInfo();

            Writer.Write(objectGuid);
            Writer.Write(appraiseInfo);
        }
    }
}
