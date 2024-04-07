using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Numerics;
using ACE.DatLoader;
using ACE.DatLoader.FileTypes;

namespace ACE.Server.Physics.Util
{
    public class AdjustCell
    {
        public List<Common.EnvCell> EnvCells;
        private static ConcurrentDictionary<ulong, AdjustCell> AdjustCells = new ConcurrentDictionary<ulong, AdjustCell>();

        public AdjustCell(uint dungeonID, uint instance)
        {
            uint blockInfoID = dungeonID << 16 | 0xFFFE;
            var blockinfo = DatManager.CellDat.ReadFromDat<LandblockInfo>(blockInfoID);
            var numCells = blockinfo.NumCells;

            BuildEnv(dungeonID, instance, numCells);
        }

        public void BuildEnv(uint dungeonID, uint instance, uint numCells)
        {
            EnvCells = new List<Common.EnvCell>();
            uint firstCellID = 0x100;
            for (uint i = 0; i < numCells; i++)
            {
                uint cellID = firstCellID + i;
                uint blockCell = dungeonID << 16 | cellID;

                var objCell = Common.LScape.get_landcell(blockCell, instance);
                var envCell = objCell as Common.EnvCell;
                if (envCell != null)
                    EnvCells.Add(envCell);
            }
        }

        public uint? GetCell(Vector3 point)
        {
            foreach (var envCell in EnvCells)
                if (envCell.point_in_cell(point))
                    return envCell.ID;
            return null;
        }

        public static AdjustCell Get(uint dungeonID, uint instance)
        {
            AdjustCell adjustCell = null;

            AdjustCells.TryGetValue(DictKey(dungeonID, instance), out adjustCell);
            if (adjustCell == null)
            {
                adjustCell = new AdjustCell(dungeonID, instance);
                AdjustCells.TryAdd(DictKey(dungeonID, instance), adjustCell);
            }
            return adjustCell;
        }

        public static void TryRemove(uint dungeonID, uint instance)
        {
            AdjustCells.TryRemove(DictKey(dungeonID, instance), out  _);
        }

        private static ulong DictKey(uint dungeonID, uint instance)
        {
            return ((ulong)instance << 32) | ((dungeonID << 16) | 0xFFFE);
        }
    }
}
