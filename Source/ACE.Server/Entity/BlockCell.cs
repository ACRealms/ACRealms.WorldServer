namespace ACE.Server.Entity
{
    public static class BlockCell
    {
        public static byte GetInstance(ulong objCellID)
        {
            return (byte)(objCellID >> 32);
        }

        public static uint GetLandblock(ulong objCellID)
        {
            return (uint)objCellID | 0xFFFF;
        }

        public static ulong GetLandblockInstance(ulong objCellID)
        {
            return objCellID | 0xFFFF;
        }

        public static ushort GetLandblockShort(ulong objCellID)
        {
            return (ushort)(objCellID >> 16);
        }

        public static byte GetLandblockX(ulong objCellID)
        {
            return (byte)(objCellID >> 24);
        }

        public static byte GetLandblockY(ulong objCellID)
        {
            return (byte)(objCellID >> 16);
        }

        public static ushort GetCell(ulong objCellID)
        {
            return (ushort)objCellID;
        }

        public static bool Indoors(ulong objCellID)
        {
            return (objCellID & 0xFFFF) >= 0x100;
        }

        public static ulong GetLongCell(uint objCellID, byte? instance = null)
        {
            byte inst = instance ?? 0;

            return (ulong)inst << 32 | objCellID;
        }
    }
}
