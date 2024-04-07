using System;
using System.Numerics;

using log4net;

using ACE.DatLoader;
using ACE.Entity;
using ACE.Entity.Enum.Properties;
using ACE.Server.Physics.Common;
using ACE.Server.Physics.Extensions;
using ACE.Server.Physics.Util;
using ACE.Server.WorldObjects;

using Position = ACE.Entity.Position;
using ACE.Server.Managers;
using ACE.Server.Realms;

namespace ACE.Server.Entity
{
    public static class PositionExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Vector3 ToGlobal(this Position p, bool skipIndoors = false)
        {
            // TODO: Is this necessary? It seemed to be loading rogue physics landblocks. Commented out 2019-04 Mag-nus
            //var landblock = LScape.get_landblock(p.LandblockId.Raw);

            // TODO: investigate dungeons that are below actual traversable overworld terrain
            // ex., 010AFFFF
            //if (landblock.IsDungeon)
            if (p.Indoors && skipIndoors)
                return p.Pos;

            var x = p.LandblockId.LandblockX * Position.BlockLength + p.PositionX;
            var y = p.LandblockId.LandblockY * Position.BlockLength + p.PositionY;
            var z = p.PositionZ;

            return new Vector3(x, y, z);
        }

        public static Vector2? GetMapCoords(this LocalPosition pos)
        {
            // no map coords available for dungeons / indoors?
            if ((pos.Cell & 0xFFFF) >= 0x100)
                return null;

            var globalPos = pos.ToGlobal();

            // 1 landblock = 192 meters
            // 1 landblock = 0.8 map units

            // 1 map unit = 1.25 landblocks
            // 1 map unit = 240 meters

            var mapCoords = new Vector2(globalPos.X / 240, globalPos.Y / 240);

            // dereth is 204 map units across, -102 to +102
            mapCoords -= Vector2.One * 102;

            return mapCoords;
        }

        public static string GetMapCoordStr(this UsablePosition pos)
        {
            var mapCoords = pos.GetMapCoords();

            if (mapCoords == null)
                return null;

            var northSouth = mapCoords.Value.Y >= 0 ? "N" : "S";
            var eastWest = mapCoords.Value.X >= 0 ? "E" : "W";

            return string.Format("{0:0.0}", Math.Abs(mapCoords.Value.Y) - 0.05f) + northSouth + ", "
                 + string.Format("{0:0.0}", Math.Abs(mapCoords.Value.X) - 0.05f) + eastWest;
        }

        public static void AdjustMapCoords(this LocalPosition pos)
        {
            // adjust Z to terrain height
            pos.PositionZ = pos.GetTerrainZ();

            // adjust to building height, if applicable
            var sortCell = LScape.get_landcell(pos.Cell, pos.Instance) as SortCell;
            if (sortCell != null && sortCell.has_building())
            {
                var building = sortCell.Building;

                var minZ = building.GetMinZ(pos.Instance);

                if (minZ > 0 && minZ < float.MaxValue)
                    pos.PositionZ += minZ;

                pos.LandblockId = new LandblockId(pos.GetCell());
            }
        }

        public static void Translate(this LocalPosition pos, uint blockCell)
        {
            var newBlockX = blockCell >> 24;
            var newBlockY = (blockCell >> 16) & 0xFF;

            var xDiff = (int)newBlockX - pos.LandblockX;
            var yDiff = (int)newBlockY - pos.LandblockY;

            //pos.Origin.X -= xDiff * 192;
            pos.PositionX -= xDiff * 192;
            //pos.Origin.Y -= yDiff * 192;
            pos.PositionY -= yDiff * 192;

            //pos.ObjCellID = blockCell;
            pos.LandblockId = new LandblockId(blockCell);
        }

        public static void FindZ(this LocalPosition pos)
        {
            var envCell = DatManager.CellDat.ReadFromDat<DatLoader.FileTypes.EnvCell>(pos.Cell);
            pos.PositionZ = envCell.Position.Origin.Z;
        }

        public static float GetTerrainZ(this LocalPosition p)
        {
            var landblock = LScape.get_landblock(p.LandblockId.Raw, p.Instance);

            var cellID = GetOutdoorCell(p);
            var landcell = (LandCell)LScape.get_landcell(cellID, p.Instance);

            if (landcell == null)
                return p.Pos.Z;

            Physics.Polygon walkable = null;
            if (!landcell.find_terrain_poly(p.Pos, ref walkable))
                return p.Pos.Z;

            Vector3 terrainPos = p.Pos;
            walkable.Plane.set_height(ref terrainPos);

            return terrainPos.Z;
        }

        /// <summary>
        /// Returns TRUE if outdoor position is located on walkable slope
        /// </summary>
        public static bool IsWalkable(this InstancedPosition p)
        {
            if (p.Indoors) return true;

            var landcell = (LandCell)LScape.get_landcell(p.Cell, p.Instance);

            Physics.Polygon walkable = null;
            var terrainPoly = landcell.find_terrain_poly(p.Pos, ref walkable);
            if (walkable == null) return false;

            return Physics.PhysicsObj.is_valid_walkable(walkable.Plane.Normal);
        }

        /// <summary>
        /// Returns TRUE if current cell is a House cell
        /// </summary>
        public static bool IsRestrictable(this LocalPosition p, Landblock landblock)
        {
            var cell = landblock.IsDungeon ? p.Cell : p.GetOutdoorCell();

            return HouseCell.HouseCells.ContainsKey(cell);
        }

        public static Position ACEPosition(this Physics.Common.PhysicsPosition pos, uint instance)
        {
            return new Position(pos.ObjCellID, pos.Frame.Origin, pos.Frame.Orientation, instance);
        }

        public static Physics.Common.PhysicsPosition PhysPosition(this Position pos)
        {
            return new Physics.Common.PhysicsPosition(pos.Cell, new Physics.Animation.AFrame(pos.Pos, pos.Rotation));
        }


        public static InstancedPosition ToInstancedPosition(this Position pos, uint instanceId)
        {
            return new InstancedPosition { Position = pos, Instance = instanceId };
        }

        public static InstancedPosition ToInstancedPosition(this LocalPosition pos, uint instanceId)
        {
            return new InstancedPosition { Position = pos.Position, Instance = instanceId };
        }

      
    }
}
