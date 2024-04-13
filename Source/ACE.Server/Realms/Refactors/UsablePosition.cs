using ACE.Adapter.GDLE.Models;
using ACE.Entity.Enum;
using ACE.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Position = ACE.Entity.Position;
using Quaternion = System.Numerics.Quaternion;
using ACE.DatLoader.Entity;
using ACE.Server.WorldObjects;
using log4net;
using ACE.Server.Entity;
using ACE.Server.Physics.Common;

namespace ACE.Server.Realms
{
    public abstract class UsablePosition
    {
        protected Position Position { get; }
        public Position GetPosition() => new Position(Position);
        private UsablePosition() { }
        public UsablePosition(Position pos) { Position = new Position(pos); }

        protected UsablePosition(Position pos, uint instance) { Position = new Position(pos, instance); }

        public LandblockId LandblockId => Position.LandblockId;
        public uint Cell => Position.Cell;
        public uint LandblockShort => Position.LandblockShort;
        public uint CellX => Position.CellX;
        public uint CellY => Position.CellY;
        public uint LandblockX => Position.LandblockX;
        public uint LandblockY => Position.LandblockY;
        public uint GlobalCellX => Position.GlobalCellX;
        public uint GlobalCellY => Position.GlobalCellY;
        public Vector3 Pos => Position.Pos;
        public Quaternion Rotation => Position.Rotation;
        public float PositionX => Position.PositionX;
        public float PositionY => Position.PositionY;
        public float PositionZ => Position.PositionZ;
        public float RotationW => Position.RotationW;
        public float RotationX => Position.RotationX;
        public float RotationY => Position.RotationY;
        public float RotationZ => Position.RotationZ;
        public bool Indoors => Position.Indoors;

        /// <summary>
        /// Returns the normalized 2D heading direction
        /// </summary>
        public Vector3 GetCurrentDir() => Position.GetCurrentDir();

        /// <summary>
        /// Returns this vector as a unit vector
        /// with a length of 1
        /// </summary>
        public Vector3 Normalize(Vector3 v) => Position.Normalize(v);
      
        public void Serialize(BinaryWriter payload, PositionFlags positionFlags, int animationFrame, bool writeLandblock = true)
            => Position.Serialize(payload, positionFlags, animationFrame, writeLandblock);

        public void Serialize(BinaryWriter payload, bool writeQuaternion = true, bool writeLandblock = true)
            => Position.Serialize(payload, writeQuaternion, writeLandblock);


        /// <summary>
        /// Returns the 3D squared distance between 2 objects
        /// </summary>
        public float SquaredDistanceTo(UsablePosition p)
            => Position.SquaredDistanceTo(p.Position);

        /// <summary>
        /// Returns the 2D distance between 2 objects
        /// </summary>
        public float Distance2D(UsablePosition p)
            => Position.Distance2D(p.Position);

        /// <summary>
        /// Returns the squared 2D distance between 2 objects
        /// </summary>
        public float Distance2DSquared(UsablePosition p)
            => Position.Distance2DSquared(p.Position);

        /// <summary>
        /// Returns the 3D distance between 2 objects
        /// </summary>
        public float DistanceTo(UsablePosition p)
            => Position.DistanceTo(p.Position);

        /// <summary>
        /// Returns the offset from current position to input position
        /// </summary>
        public Vector3 GetOffset(UsablePosition p)
            => Position.GetOffset(p.Position);

        public Vector3 ToGlobal(bool skipIndoors = false) => Position.ToGlobal(skipIndoors);

        public override string ToString() => Position.ToString();

        public string ToLOCString() => Position.ToLOCString();

        public static readonly int BlockLength = Position.BlockLength;
        public static readonly int CellSide = Position.CellSide;
        public static readonly int CellLength = Position.CellLength;

        public Vector2? GetMapCoords()
        {
            // no map coords available for dungeons / indoors?
            if ((Cell & 0xFFFF) >= 0x100)
                return null;

            var globalPos = ToGlobal();

            // 1 landblock = 192 meters
            // 1 landblock = 0.8 map units

            // 1 map unit = 1.25 landblocks
            // 1 map unit = 240 meters

            var mapCoords = new Vector2(globalPos.X / 240, globalPos.Y / 240);

            // dereth is 204 map units across, -102 to +102
            mapCoords -= Vector2.One * 102;

            return mapCoords;
        }

        public string GetMapCoordStr()
        {
            var mapCoords = GetMapCoords();

            if (mapCoords == null)
                return null;

            var northSouth = mapCoords.Value.Y >= 0 ? "N" : "S";
            var eastWest = mapCoords.Value.X >= 0 ? "E" : "W";

            return string.Format("{0:0.0}", Math.Abs(mapCoords.Value.Y) - 0.05f) + northSouth + ", "
                 + string.Format("{0:0.0}", Math.Abs(mapCoords.Value.X) - 0.05f) + eastWest;
        }

        public PhysicsPosition PhysPosition()
        {
            return new PhysicsPosition(Position.Cell, new Physics.Animation.AFrame(Position.Pos, Position.Rotation));
        }
    }
}
