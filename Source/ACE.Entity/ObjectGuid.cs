namespace ACE.Entity
{
    public enum GuidType
    {
        Undef,
        Player,
        Static,
        Dynamic,
    }

    public struct ObjectGuid
    {
        public static readonly ObjectGuid Invalid = new ObjectGuid(0);

        /* These are not GUIDs
        public static uint WeenieMin { get; } = 0x00000001;
        public static uint WeenieMax { get; } = 0x000F423F; // 999,999 */

        // 0x01000001 and 0x422C91BC Only PCAP'd 9 GUID's found in this range 

        public static uint PlayerMin { get; } = 0x50000001;
        public static uint PlayerMax { get; } = 0x5FFFFFFF;

        // 0x60000000 No PCAP'd GUID's in this range

        // PY 16 has these ranges 0x70003000 - 0x7FADA053
        // They are organized by landblock where 0x7AABB000 is landblock AABB
        // These represent items that come from the World db
        public static uint StaticObjectMin { get; } = 0x70000000;
        public static uint StaticObjectMax { get; } = 0x7FFFFFFF;

        // These represent items are generated in the world. Some of them will be saved to the Shard db.
        public static uint DynamicMin { get; } = 0x80000000;
        public static uint DynamicMax { get; } = 0xFFFFFFFE; // Ends at E because uint.Max is reserved for "invalid"

        public static bool IsPlayer(ulong guid) { return (TranslateToClientGuid(guid) >= PlayerMin && TranslateToClientGuid(guid) <= PlayerMax); }
        public static bool IsStatic(ulong guid) { return (TranslateToClientGuid(guid) >= StaticObjectMin && TranslateToClientGuid(guid) <= StaticObjectMax); }
        public static bool IsDynamic(ulong guid) { return (TranslateToClientGuid(guid) >= DynamicMin && TranslateToClientGuid(guid) <= DynamicMax); }

        public ulong Full { get; }
        public uint ClientGUID => TranslateToClientGuid(Full);
        public uint Low => (uint)(Full & 0xFFFFFF);
        public uint High => (uint)(Full >> 56);
        public uint? Instance
        {
            get
            {
                if (Type != GuidType.Static)
                    return null;
                var instance = (uint)(Full >> 32);
                if (instance == 0)
                    return null;
                return instance;
            }
        }
        
        public GuidType Type { get; }

        public static uint TranslateToClientGuid(ulong fullGuid) => (uint)(fullGuid & 0xFFFFFFFF);
        public static ulong TranslateToServerGuid(uint clientGuid, uint instance)
        {
            if (!IsStatic(clientGuid))
                return clientGuid;
            return ((ulong)instance << 32) | clientGuid;
        }

        public ObjectGuid(ulong full)
        {
            Full = full;
            var clientGUID = TranslateToClientGuid(full);
            if (IsPlayer(clientGUID))
                Type = GuidType.Player;
            else if (IsStatic(clientGUID))
                Type = GuidType.Static;
            else if (IsDynamic(clientGUID))
                Type = GuidType.Dynamic;
            else
                Type = GuidType.Undef;

            if (Type != GuidType.Static)
                Full = clientGUID;
        }

        public ObjectGuid(uint clientGUID, uint instance)
            : this(TranslateToServerGuid(clientGUID, instance)) { }

        public ObjectGuid(ulong clientOrFullGuid, uint instance)
            : this(TranslateToServerGuid((uint)(clientOrFullGuid & 0xFFFFFFFF), instance)) { }

        public bool IsPlayer()
        {
            return Type == GuidType.Player;
        }

        public bool IsStatic()
        {
            return Type == GuidType.Static;
        }

        public ushort? StaticObjectLandblock
        {
            get
            {
                if (Type != GuidType.Static)
                    return null;
                return (ushort)((ClientGUID >> 12) & (uint)0xFFFF);
            }
        }
        public ushort? StaticObjectIndex
        {
            get
            {
                if (Type != GuidType.Static)
                    return null;
                return (ushort)(ClientGUID & (uint)0xFFF);
            }
        }
        public bool IsDynamic()
        {
            return Type == GuidType.Dynamic;
        }

        public static bool operator ==(ObjectGuid g1, ObjectGuid g2)
        {
            return g1.Full == g2.Full;
        }

        public static bool operator !=(ObjectGuid g1, ObjectGuid g2)
        {
            return g1.Full != g2.Full;
        }

        public override bool Equals(object obj)
        {
            return obj is ObjectGuid guid && guid == this;
        }

        public override int GetHashCode()
        {
            return Full.GetHashCode();
        }

        public override string ToString()
        {
            if (Instance == 0 || Instance == null)
                return Full.ToString("X8");
            else
                return Full.ToString("X16");
        }
    }
}
