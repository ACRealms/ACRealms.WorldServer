namespace ACE.Server.Network.GameEvent.Events
{
    public class GameEventCombatCommenceAttack : GameEventMessage
    {
        public GameEventCombatCommenceAttack(ISession session)
            : base(GameEventType.CombatCommenceAttack, GameMessageGroup.UIQueue, session, 4)
        {
        }
    }
}
