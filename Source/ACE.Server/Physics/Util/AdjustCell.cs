using System.Collections.Generic;
using System.Numerics;
using ACE.DatLoader;
using ACE.DatLoader.FileTypes;

namespace ACE.Server.Physics.Util
{
    public class AdjustCell
    {
        public List<Common.EnvCell> EnvCells { get; set; }
        public static Dictionary<ulong, AdjustCell> AdjustCells { get; set; } = new Dictionary<ulong, AdjustCell>();

        public AdjustCell(ulong iDungeonID)
        {
            var dungeonID = (ushort)iDungeonID;

            uint blockInfoID = (uint)((dungeonID << 16) | 0xFFFE);
            var blockinfo = DatManager.CellDat.ReadFromDat<LandblockInfo>(blockInfoID);
            var numCells = blockinfo.NumCells;

            BuildEnv(iDungeonID, numCells);
        }

        public void BuildEnv(ulong iDungeonID, uint numCells)
        {
            EnvCells = new List<Common.EnvCell>();
            uint firstCellID = 0x100;
            for (uint i = 0; i < numCells; i++)
            {
                uint cellID = firstCellID + i;
                ulong iBlockCell = iDungeonID << 16 | cellID;

                var objCell = Common.LScape.get_landcell(iBlockCell);
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

        public static AdjustCell Get(ulong iDungeonID)
        {
            AdjustCell adjustCell = null;
            AdjustCells.TryGetValue(iDungeonID, out adjustCell);
            if (adjustCell == null)
            {
                adjustCell = new AdjustCell(iDungeonID);
                AdjustCells.Add(iDungeonID, adjustCell);
            }
            return adjustCell;
        }
    }
}
