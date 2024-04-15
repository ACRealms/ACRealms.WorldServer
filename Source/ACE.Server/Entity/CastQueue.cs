using ACE.Server.WorldObjects;

namespace ACE.Server.Entity
{
    /// <summary>
    /// questionable if this was ever in retail
    /// </summary>
    public class CastQueue
    {
        public CastQueueType Type;
        public ulong TargetGuid;
        public uint SpellId;
        //public bool BuiltInSpell;
        public WorldObject CasterItem;

        public CastQueue(CastQueueType type, ulong targetGuid, uint spellId, WorldObject casterItem)
        {
            Type = type;
            TargetGuid = targetGuid;
            SpellId = spellId;
            //BuiltInSpell = builtInSpell;
            CasterItem = casterItem;
        }
    }

    public enum CastQueueType
    {
        Targeted,
        Untargeted
    }
}
