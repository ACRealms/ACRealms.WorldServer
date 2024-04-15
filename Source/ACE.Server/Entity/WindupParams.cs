using ACE.Server.WorldObjects;

namespace ACE.Server.Entity
{
    public class WindupParams
    {
        public ulong TargetGuid;
        public uint SpellId;
        //public bool BuiltInSpell;
        public WorldObject CasterItem;

        public WindupParams(ulong targetGuid, uint spellId, WorldObject casterItem)
        {
            TargetGuid = targetGuid;
            SpellId = spellId;
            //BuiltInSpell = builtInSpell;
            CasterItem = casterItem;
        }

        public override string ToString()
        {
            return $"TargetGuid: {TargetGuid:X16}, SpellID: {SpellId}, CasterItem: {CasterItem?.Name}";
        }
    }
}
