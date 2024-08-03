using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;

using ACE.Entity;
using ACE.Server.Entity;
using ACE.Server.Managers;
using ACE.Server.Physics.Util;

namespace ACE.Server.Physics.Common
{
    //REALMS TODO: Fix caching used here 
    public static class LScape
    {
        //public static int MidRadius = 5;
        //public static int MidWidth = 11;

        private static readonly object landblockMutex = new();
        /// <summary>
        /// This is not used if PhysicsEngine.Instance.Server is true
        /// </summary>
        private static ConcurrentDictionary<ulong, Landblock> Landblocks = new ConcurrentDictionary<ulong, Landblock>();
        //public static Dictionary<uint, Landblock> BlockDrawList = new Dictionary<uint, Landblock>();

        //public static uint LoadedCellID;
        //public static uint ViewerCellID;
        //public static int ViewerXOffset;
        //public static int ViewerYOffset;
        //public static GameSky GameSky;
        //public static Surface LandscapeDetailSurface;
        //public static Surface EnvironmentDetailSurface;
        //public static Surface BuildingDetailSurface;
        //public static Surface ObjectDetailSurface;

        //public static float AmbientLevel = 0.4f;
        //public static Vector3 Sunlight = new Vector3(1.2f, 0, 0.5f);

        //public static bool SetMidRadius(int radius)
        //{
        //    if (radius < 1 || Landblocks == null)
        //        return false;

        //    MidRadius = radius;
        //    MidWidth = 2 * radius + 1;
        //    return true;
        //}

        //public static int LandblocksCount => Landblocks.Count;

        /// <summary>
        /// Loads the backing store landblock structure<para />
        /// This function is thread safe
        /// </summary>
        /// <param name="blockCellID">Any landblock + cell ID within the landblock</param>
        public static Landblock get_landblock(uint blockCellID, uint instance)
        {
            var landblockID = blockCellID | 0xFFFF;
            var instancedLandblockID = LandblockKey(landblockID, instance);

            if (PhysicsEngine.Instance.Server)
            {
                var lbid = new LandblockId(landblockID);
                var lbmLandblock = LandblockManager.GetLandblock(lbid, instance, null, false, false);

                return lbmLandblock.PhysicsLandblock;
            }

            // client implementation
            /*if (Landblocks == null || Landblocks.Count == 0)
                return null;

            if (!LandDefs.inbound_valid_cellid(cellID) || cellID >= 0x100)
                return null;

            var local_lcoord = LandDefs.blockid_to_lcoord(LoadedCellID);
            var global_lcoord = LandDefs.gid_to_lcoord(cellID);

            var xDiff = ((int)global_lcoord.Value.X + 8 * MidRadius - (int)local_lcoord.Value.X) / 8;
            var yDiff = ((int)global_lcoord.Value.Y + 8 * MidRadius - (int)local_lcoord.Value.Y) / 8;

            if (xDiff < 0 || yDiff < 0 || xDiff < MidWidth || yDiff < MidWidth)
                return null;

            return Landblocks[yDiff + xDiff * MidWidth];*/

            // check if landblock is already cached
            if (Landblocks.TryGetValue(instancedLandblockID, out var landblock))
                return landblock;

            lock (landblockMutex)
            {
                // check if landblock is already cached, this time under the lock.
                if (Landblocks.TryGetValue(instancedLandblockID, out landblock))
                    return landblock;

                // if not, load into cache
                landblock = new Landblock(DBObj.GetCellLandblock(landblockID), instance);
                if (Landblocks.TryAdd(landblockID, landblock))
                    landblock.PostInit();
                else
                    Landblocks.TryGetValue(landblockID, out landblock);

                return landblock;
            }
        }

        public static bool unload_landblock(uint dungeonID, uint instance)
        {
            if (PhysicsEngine.Instance.Server)
            {
                // todo: Instead of ACE.Server.Entity.Landblock.Unload() calling this function, it should be calling PhysicsLandblock.Unload()
                // todo: which would then call AdjustCell.AdjustCells.Remove()

                AdjustCell.TryRemove(dungeonID, instance);
                return true;
            }

            var result = Landblocks.TryRemove(LandblockKey(dungeonID, instance), out _);
            // todo: Like mentioned above, the following function should be moved to ACE.Server.Physics.Common.Landblock.Unload()
            AdjustCell.TryRemove(dungeonID, instance);
            return result;
        }

        /// <summary>
        /// Gets the landcell from a landblock. If the cell is an indoor cell and hasn't been loaded, it will be loaded.<para />
        /// This function is thread safe
        /// </summary>
        public static ObjCell get_landcell(uint blockCellID, uint instance)
        {
            //Console.WriteLine($"get_landcell({blockCellID:X8}");

            var landblock = get_landblock(blockCellID, instance);
            if (landblock == null)
                return null;

            var cellID = blockCellID & 0xFFFF;
            ObjCell cell;

            // outdoor cells
            if (cellID < 0x100)
            {
                var lcoord = LandDefs.gid_to_lcoord(blockCellID, false);
                if (lcoord == null) return null;
                var landCellIdx = ((int)lcoord.Value.Y % 8) + ((int)lcoord.Value.X % 8) * LandblockStruct.SideCellCount;
                landblock.LandCells.TryGetValue(landCellIdx, out cell);
            }
            // indoor cells
            else
            {
                if (landblock.LandCells.TryGetValue((int)cellID, out cell))
                    return cell;

                lock (landblock.LandCellMutex)
                {
                    if (landblock.LandCells.TryGetValue((int)cellID, out cell))
                        return cell;

                    cell = DBObj.GetEnvCell(blockCellID);
                    cell.CurLandblock = landblock;

                    landblock.LandCells.TryAdd((int)cellID, cell);
                    var envCell = (EnvCell)cell;
                    envCell.PostInit();
                }
            }
            return cell;
        }


        private static ulong LandblockKey(uint blockCellID, uint instance)
        {
            return ((ulong)instance << 32) | (blockCellID | 0xFFFF);
        }
    }
}
