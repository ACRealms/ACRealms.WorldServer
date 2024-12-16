using ACE.DatLoader;
using ACE.Entity;
using ACE.Server.Managers;
using ACE.Server.Network.GameAction.Actions;
using ACE.Server.Physics.Common;
using ACE.Server.Physics.Extensions;
using ACE.Server.Physics.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Realms
{
    public class NullRealmException : Exception
    {
        public NullRealmException()
            : base("Instance ID may not be 0") { }
    }

    public sealed class InstancedPosition : UsablePosition
    {
        public readonly uint Instance;

        public InstancedPosition(Position pos, uint instance)
            : base(pos, instance)
        {
            if (instance == 0 && !ACE.Entity.ACRealms.RealmsFromACESetupHelper.UnsafeInstanceIDTemporarilyAllowed)
                throw new NullRealmException();
            Instance = instance;
        }

        public InstancedPosition(LocalPosition pos, uint instance)
            : this(pos.GetPosition(), instance) { }

        public InstancedPosition(InstancedPosition pos, uint instance)
            : this(pos.Position, instance) { }

        public InstancedPosition(InstancedPosition pos)
            : this(pos.Position, pos.Instance) { }

        public LocalPosition AsLocalPosition()
            => new LocalPosition(Position);

        public InstancedPosition(Vector2 coordinates, uint instance)
            : this(new Position(coordinates), instance) { }

        public InstancedPosition(float northSouth, float eastWest, uint instance)
            : this(new Position(northSouth, eastWest), instance) { }

        public InstancedPosition(uint blockCellID, float newPositionX, float newPositionY,
                                float newPositionZ, float newRotationX, float newRotationY,
                                float newRotationZ, float newRotationW, uint instance, bool relativePos = false)
            : this(new Position(blockCellID, newPositionX, newPositionY, newPositionZ, newRotationX, newRotationY, newRotationZ, newRotationW, instance, relativePos), instance) { }

        public InstancedPosition(uint blockCellID, Vector3 position, Quaternion rotation, uint instance)
            : this(new Position(blockCellID, position, rotation), instance) { }

        public ulong InstancedLandblock => Position.InstancedLandblock;

        // REALMS: New fields/props on existing entity classes should be marked as private where possible
        private ulong LongObjCellID => (ulong)Instance << 32 | Cell;
        private ulong LongLandblockID => LongObjCellID | 0xFFFF;

        public bool Equals(InstancedPosition p) => Position.Equals(p.Position) && Instance == p.Instance;

        //public void SetToDefaultRealmInstance(ushort newRealmId)
        //{
        //    Instance = InstanceIDFromVars(newRealmId, 0, false);
        //}

        public static uint InstanceIDFromVars(ushort realmId, ushort shortInstanceId, bool isTemporaryRuleset)
        {
            if (realmId > 0x7FFF)
                throw new ArgumentOutOfRangeException(nameof(realmId));
            uint result = ((uint)realmId) << 16;
            result |= (uint)shortInstanceId;
            if (isTemporaryRuleset)
                result |= 0x80000000;
            return result;
        }

        public static void ParseInstanceID(uint instanceId, out bool isTemporaryRuleset, out ushort realmId, out ushort shortInstanceId)
        {
            shortInstanceId = (ushort)(instanceId & 0xFFFF);
            ushort left = (ushort)(instanceId >> 16);
            isTemporaryRuleset = (left & 0x8000) == 0x8000;
            realmId = (ushort)(left & 0x7FFF);
        }

        /// <summary>
        /// Returns null if it is not a permanent realm
        /// </summary>
        internal WorldRealm WorldRealm
        {
            get
            {
                ParseInstanceID(Instance, out var ephemeral, out var realmId, out _);
                if (IsEphemeralRealm)
                    return null;
                return RealmManager.GetRealm(realmId, false);
            }
        }

        public ushort RealmID
        {
            get
            {
                ParseInstanceID(Instance, out var _a, out var realmId, out var _b);
                return realmId;
            }
        }

        public bool IsEphemeralRealm
        {
            get
            {
                ParseInstanceID(this.Instance, out var result, out var _a, out var _b);
                return result;
            }
        }

        public uint GetCell()
        {
            var landblock = LScape.get_landblock(LandblockId.Raw, Instance);

            // dungeons
            // TODO: investigate dungeons that are below actual traversable overworld terrain
            // ex., 010AFFFF
            //if (landblock.IsDungeon)
            if (Indoors)
                return GetIndoorCell();

            // outside - could be on landscape, in building, or underground cave
            var cellID = GetOutdoorCell();
            var landcell = LScape.get_landcell(cellID, Instance) as LandCell;

            if (landcell == null)
                return cellID;

            if (landcell.has_building())
            {
                var envCells = landcell.Building.get_building_cells(Instance);
                foreach (var envCell in envCells)
                    if (envCell.point_in_cell(Pos))
                        return envCell.ID;
            }

            // handle underground areas ie. caves
            // get the terrain Z-height for this X/Y
            Physics.Polygon walkable = null;
            var terrainPoly = landcell.find_terrain_poly(Pos, ref walkable);
            if (walkable != null)
            {
                Vector3 terrainPos = Pos;
                walkable.Plane.set_height(ref terrainPos);

                // are we below ground? if so, search all of the indoor cells for this landblock
                if (terrainPos.Z > Pos.Z)
                {
                    var envCells = landblock.get_envcells();
                    foreach (var envCell in envCells)
                        if (envCell.point_in_cell(Pos))
                            return envCell.ID;
                }
            }

            return cellID;
        }

        public InstancedPosition InFrontOf(double distanceInFront, bool rotate180 = false)
        {
            float qw = RotationW; // north
            float qz = RotationZ; // south

            double x = 2 * qw * qz;
            double y = 1 - 2 * qz * qz;

            var heading = Math.Atan2(x, y);
            var dx = -1 * Convert.ToSingle(Math.Sin(heading) * distanceInFront);
            var dy = Convert.ToSingle(Math.Cos(heading) * distanceInFront);

            // move the Z slightly up and let gravity pull it down.  just makes things easier.
            var bumpHeight = 0.05f;
            Position pos;
            if (rotate180)
            {
                var rotate = new Quaternion(0, 0, qz, qw) * Quaternion.CreateFromYawPitchRoll(0, 0, (float)Math.PI);
                pos = new Position(LandblockId.Raw, PositionX + dx, PositionY + dy, PositionZ + bumpHeight, 0f, 0f, rotate.Z, rotate.W, Instance);
            }
            else
                pos = new Position(LandblockId.Raw, PositionX + dx, PositionY + dy, PositionZ + bumpHeight, 0f, 0f, qz, qw, Instance);

            var pos2 = new InstancedPosition(pos, Instance);
            pos2 = pos2.SetLandblockId(new LandblockId(pos2.GetCell()));

            return new InstancedPosition(pos2, Instance);
        }

        /// <summary>
        /// Gets an indoor cell ID for a position within a dungeon
        /// </summary>
        public uint GetIndoorCell()
        {
            var adjustCell = AdjustCell.Get(LandblockShort, Instance);
            var envCell = adjustCell.GetCell(Pos);
            if (envCell != null)
                return envCell.Value;
            else
                return Cell;
        }

        /// <summary>
        /// Gets an outdoor cell ID for a position within a landblock
        /// </summary>
        public uint GetOutdoorCell()
        {
            var cellX = (uint)PositionX / CellLength;
            var cellY = (uint)PositionY / CellLength;

            var cellID = cellX * CellSide + cellY + 1;

            var blockCellID = (uint)((LandblockId.Raw & 0xFFFF0000) | cellID);
            return blockCellID;
        }

        /// <summary>
        /// Returns TRUE if current cell is a House cell
        /// </summary>
        public bool IsRestrictable(Landblock landblock)
        {
            var cell = landblock.IsDungeon ? Cell : GetOutdoorCell();

            return Entity.HouseCell.HouseCells.ContainsKey(cell);
        }

        #region Mutators

        public InstancedPosition AddPos(Vector3 pos)
        {
            var newPos = new Position(Position);
            newPos.Pos += pos;
            return new InstancedPosition(newPos, Instance);
        }

        public InstancedPosition SetPos(Vector3 pos)
        {
            var newPos = new Position(Position);
            newPos.Pos = pos;
            return new InstancedPosition(newPos, Instance);
        }

        public InstancedPosition SetLandblockId(LandblockId id)
        {
            var newPos = new Position(Position);
            newPos.LandblockId = id;
            return new InstancedPosition(newPos, Instance);
        }

        public InstancedPosition SetRotation(Quaternion rotation)
        {
            var newPos = new Position(Position);
            newPos.Rotation = rotation;
            return new InstancedPosition(newPos, Instance);
        }

        public float GetTerrainZ()
        {
            var landblock = LScape.get_landblock(Position.LandblockId.Raw, Instance);

            var cellID = GetOutdoorCell();
            var landcell = (LandCell)LScape.get_landcell(cellID, Instance);

            if (landcell == null)
                return Position.Pos.Z;

            Physics.Polygon walkable = null;
            if (!landcell.find_terrain_poly(Position.Pos, ref walkable))
                return Position.Pos.Z;

            Vector3 terrainPos = Position.Pos;
            walkable.Plane.set_height(ref terrainPos);

            return terrainPos.Z;
        }

        public InstancedPosition AdjustMapCoords()
        {
            var pos = new Position(Position);
            // adjust Z to terrain height
            pos.PositionZ = GetTerrainZ();

            // adjust to building height, if applicable
            var sortCell = LScape.get_landcell(pos.Cell, pos.Instance) as SortCell;
            if (sortCell != null && sortCell.has_building())
            {
                var building = sortCell.Building;

                var minZ = building.GetMinZ(pos.Instance);

                if (minZ > 0 && minZ < float.MaxValue)
                    pos.PositionZ += minZ;


                pos.LandblockId = new LandblockId(new InstancedPosition(pos, Instance).GetCell());
            }
            return new InstancedPosition(pos, Instance);
        }

        public InstancedPosition SetPositionZ(float positionZ)
        {
            var pos = new Position(Position);
            pos.PositionZ = positionZ;
            return new InstancedPosition(pos, Instance);
        }

        public InstancedPosition Translate(uint blockCell)
        {
            return new LocalPosition(Position).Translate(blockCell).AsInstancedPosition(Instance);
        }

        public InstancedPosition SetPositions(float? positionX, float? positionY, float? positionZ, Quaternion? rotation = null)
        {
            var pos = new Position(Position);
            pos.PositionX = positionX.GetValueOrDefault(pos.PositionX);
            pos.PositionY = positionY.GetValueOrDefault(pos.PositionY);
            pos.PositionZ = positionZ.GetValueOrDefault(pos.PositionZ);
            pos.Rotation = rotation.GetValueOrDefault(pos.Rotation);
            return new InstancedPosition(pos, Instance);
        }
        public InstancedPosition Rotate(Vector3 dir)
        {
            return SetRotation(Quaternion.CreateFromYawPitchRoll(0, 0, (float)Math.Atan2(-dir.X, dir.Y)));
        }
        public InstancedPosition FindZ()
        {
            var envCell = DatManager.CellDat.ReadFromDat<DatLoader.FileTypes.EnvCell>(Cell);
            return SetPositionZ(envCell.Position.Origin.Z);
        }

        #endregion
    }
}
