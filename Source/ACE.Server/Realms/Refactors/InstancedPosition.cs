using ACE.Entity;
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
    public sealed class InstancedPosition : UsablePosition
    {
        public readonly uint Instance;

        public InstancedPosition(Position pos, uint instance)
            : base(pos, instance)
        {
            if (instance == 0)
                throw new ArgumentException("Instance ID may not be 0");
            Instance = instance;
        }

        public InstancedPosition(InstancedPosition pos)
            : this(pos.Position, pos.Instance) { }

        public LocalPosition AsLocalPosition()
            => new LocalPosition(Position);

        public InstancedPosition(Vector2 coordinates, uint instance)
            : this(new Position(coordinates, instance), instance) { }

        public InstancedPosition(float northSouth, float eastWest, uint instance)
            : this(new Position(northSouth, eastWest, instance), instance) { }

        public InstancedPosition(uint blockCellID, float newPositionX, float newPositionY,
                                float newPositionZ, float newRotationX, float newRotationY,
                                float newRotationZ, float newRotationW, uint instance, bool relativePos = false)
            : this(new Position(blockCellID, newPositionX, newPositionY, newPositionZ, newRotationX, newRotationY, newRotationZ, newRotationW, instance, relativePos), instance) { }

        public InstancedPosition(uint blockCellID, Vector3 position, Quaternion rotation, uint instance)
            : this(new Position(blockCellID, position, rotation, instance), instance) { }

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

        private static uint GetCell(Position p, uint instance)
        {
            var landblock = LScape.get_landblock(p.LandblockId.Raw, instance);

            // dungeons
            // TODO: investigate dungeons that are below actual traversable overworld terrain
            // ex., 010AFFFF
            //if (landblock.IsDungeon)
            if (p.Indoors)
                return GetIndoorCell(p, instance);

            // outside - could be on landscape, in building, or underground cave
            var cellID = GetOutdoorCell(p);
            var landcell = LScape.get_landcell(cellID, instance) as LandCell;

            if (landcell == null)
                return cellID;

            if (landcell.has_building())
            {
                var envCells = landcell.Building.get_building_cells(instance);
                foreach (var envCell in envCells)
                    if (envCell.point_in_cell(p.Pos))
                        return envCell.ID;
            }

            // handle underground areas ie. caves
            // get the terrain Z-height for this X/Y
            Physics.Polygon walkable = null;
            var terrainPoly = landcell.find_terrain_poly(p.Pos, ref walkable);
            if (walkable != null)
            {
                Vector3 terrainPos = p.Pos;
                walkable.Plane.set_height(ref terrainPos);

                // are we below ground? if so, search all of the indoor cells for this landblock
                if (terrainPos.Z > p.Pos.Z)
                {
                    var envCells = landblock.get_envcells();
                    foreach (var envCell in envCells)
                        if (envCell.point_in_cell(p.Pos))
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
            pos.LandblockId = new LandblockId(GetCell(pos, Instance));

            return new InstancedPosition(pos, Instance);
        }

        /// <summary>
        /// Gets an indoor cell ID for a position within a dungeon
        /// </summary>
        private static uint GetIndoorCell(Position pos, uint instance)
        {
            var adjustCell = AdjustCell.Get(pos.LandblockShort, instance);
            var envCell = adjustCell.GetCell(pos.Pos);
            if (envCell != null)
                return envCell.Value;
            else
                return pos.Cell;
        }

        /// <summary>
        /// Gets an outdoor cell ID for a position within a landblock
        /// </summary>
        private static uint GetOutdoorCell(Position p)
        {
            var cellX = (uint)p.PositionX / Position.CellLength;
            var cellY = (uint)p.PositionY / Position.CellLength;

            var cellID = cellX * Position.CellSide + cellY + 1;

            var blockCellID = (uint)((p.LandblockId.Raw & 0xFFFF0000) | cellID);
            return blockCellID;
        }
    }
}
