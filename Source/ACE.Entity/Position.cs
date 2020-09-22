using System;
using System.IO;
using System.Numerics;

namespace ACE.Entity
{
    public class Position
    {
        public uint ObjCellID { get; set; }

        public ushort Landblock => (ushort)(ObjCellID >> 16);

        public ushort Cell => (ushort)ObjCellID;

        public byte LandblockX => (byte)(ObjCellID >> 24);

        public byte LandblockY => (byte)(ObjCellID >> 16);

        public byte CellX => (byte)(ObjCellID >> 8);

        public byte CellY => (byte)ObjCellID;

        public bool Indoors => (ObjCellID & 0xFFFF) >= 0x100;

        public byte Instance;

        public ulong LongObjCellID => (ulong)Instance << 32 | ObjCellID;

        public Vector3 _pos;

        public Vector3 Pos
        {
            get => _pos;
            set => SetPosition(value);
        }

        public Tuple<bool, bool> SetPosition(Vector3 pos)
        {
            _pos = pos;

            var blockUpdate = SetLandblock();
            var cellUpdate = SetLandCell();

            return new Tuple<bool, bool>(blockUpdate, cellUpdate);
        }

        public Quaternion Rotation;

        public void Rotate(Vector3 dir)
        {
            Rotation = Quaternion.CreateFromYawPitchRoll(0, 0, (float)Math.Atan2(-dir.X, dir.Y));
        }

        /// <summary>
        /// Returns the normalized 2D heading direction
        /// </summary>
        public Vector3 GetCurrentDir()
        {
            return Vector3.Normalize(Vector3.Transform(Vector3.UnitY, Rotation));
        }

        public Position()
        {
            //Pos = Vector3.Zero;
            Rotation = Quaternion.Identity;
        }

        public Position(Position pos)
        {
            ObjCellID = pos.ObjCellID;
            Pos = pos.Pos;
            Rotation = pos.Rotation;

            Instance = pos.Instance;
        }

        public Position(uint blockCellID, float newPositionX, float newPositionY, float newPositionZ, float newRotationX, float newRotationY, float newRotationZ, float newRotationW, byte? instance = null)
        {
            ObjCellID = blockCellID;

            Instance = instance ?? 0;

            _pos = new Vector3(newPositionX, newPositionY, newPositionZ);
            Rotation = new Quaternion(newRotationX, newRotationY, newRotationZ, newRotationW);

            if (Cell == 0)
                SetPosition(_pos);
        }

        public Position(uint blockCellID, Vector3 position, Quaternion rotation, bool normalize = false)
        {
            ObjCellID = blockCellID;

            _pos = position;
            Rotation = rotation;

            if (Cell == 0 || normalize)
                SetPosition(_pos);
        }

        public Position(BinaryReader payload)
        {
            ObjCellID = payload.ReadUInt32();

            Pos = new Vector3(payload.ReadSingle(), payload.ReadSingle(), payload.ReadSingle());

            // packet stream isn't the same order as the quaternion constructor
            var w = payload.ReadSingle();

            Rotation = new Quaternion(payload.ReadSingle(), payload.ReadSingle(), payload.ReadSingle(), w);
        }

        public Position(float northSouth, float eastWest)
        {
            northSouth = (northSouth - 0.5f) * 10.0f;
            eastWest = (eastWest - 0.5f) * 10.0f;

            var baseX = (uint)(eastWest + 0x400);
            var baseY = (uint)(northSouth + 0x400);

            if (baseX >= 0x7F8 || baseY >= 0x7F8)
            {
                Console.WriteLine($"Position({northSouth}, {eastWest}) - bad coordinates");
                return;
            }

            float xOffset = ((baseX & 7) * 24.0f) + 12;
            float yOffset = ((baseY & 7) * 24.0f) + 12;

            ObjCellID = GetCellFromBase(baseX, baseY);

            Pos = new Vector3(xOffset, yOffset, 0);

            Rotation = Quaternion.Identity;
        }

        private uint GetCellFromBase(uint baseX, uint baseY)
        {
            byte blockX = (byte)(baseX >> 3);
            byte blockY = (byte)(baseY >> 3);
            byte cellX = (byte)(baseX & 7);
            byte cellY = (byte)(baseY & 7);

            uint block = (uint)((blockX << 8) | blockY);
            uint cell = (uint)((cellX << 3) | cellY);

            return (block << 16) | (cell + 1);
        }

        public static readonly Quaternion OneEighty = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.PI);

        public Position InFrontOf(float distance, bool rotate180 = false)
        {
            var pos = Pos + Vector3.Transform(Vector3.UnitY * distance, Rotation);

            var angle = Rotation;
            if (rotate180)
                angle *= OneEighty;

            // move the Z slightly up due to variations in ac physics engine
            pos.Z += 0.05f;

            return new Position(ObjCellID, pos, angle, true);
        }


        public static readonly int BlockLength = 192;

        /// <summary>
        /// Handles the Position crossing over landblock boundaries
        /// </summary>
        public bool SetLandblock()
        {
            if (Indoors) return false;

            var changedBlock = false;

            if (Pos.X < 0)
            {
                var blockOffset = (int)Pos.X / BlockLength - 1;
                var landblock = TransitionX(blockOffset);
                if (landblock != null)
                {
                    ObjCellID = landblock.Value;
                    _pos.X -= BlockLength * blockOffset;
                    changedBlock = true;
                }
                else
                    _pos.X = 0;
            }

            else if (Pos.X >= BlockLength)
            {
                var blockOffset = (int)Pos.X / BlockLength;
                var landblock = TransitionX(blockOffset);
                if (landblock != null)
                {
                    ObjCellID = landblock.Value;
                    _pos.X -= BlockLength * blockOffset;
                    changedBlock = true;
                }
                else
                    _pos.X = BlockLength;
            }

            if (Pos.Y < 0)
            {
                var blockOffset = (int)Pos.Y / BlockLength - 1;
                var landblock = TransitionY(blockOffset);
                if (landblock != null)
                {
                    ObjCellID = landblock.Value;
                    _pos.Y -= BlockLength * blockOffset;
                    changedBlock = true;
                }
                else
                    _pos.Y = 0;
            }

            else if (Pos.Y >= BlockLength)
            {
                var blockOffset = (int)Pos.Y / BlockLength;
                var landblock = TransitionY(blockOffset);
                if (landblock != null)
                {
                    ObjCellID = landblock.Value;
                    _pos.Y -= BlockLength * blockOffset;
                    changedBlock = true;
                }
                else
                    _pos.Y = BlockLength;
            }

            return changedBlock;
        }

        public uint? TransitionX(int blockOffset)
        {
            var newX = LandblockX + blockOffset;
            if (newX < 0 || newX > 254)
                return null;
            else
                return (uint)(newX << 24 | LandblockY << 16 | Cell);
        }

        public uint? TransitionY(int blockOffset)
        {
            var newY = LandblockY + blockOffset;
            if (newY < 0 || newY > 254)
                return null;
            else
                return (uint)(LandblockX << 24 | newY << 16 | Cell);
        }

        public static readonly int CellSide = 8;
        public static readonly int CellLength = 24;

        /// <summary>
        /// Determines the outdoor landcell for current position
        /// </summary>
        public bool SetLandCell()
        {
            if (Indoors) return false;

            var cellX = (uint)Pos.X / CellLength;
            var cellY = (uint)Pos.Y / CellLength;

            var cellID = cellX * CellSide + cellY + 1;

            var curCellID = ObjCellID & 0xFFFF;

            if (cellID == curCellID)
                return false;

            ObjCellID = (uint)((ObjCellID & 0xFFFF0000) | cellID);

            return true;
        }

        public void Serialize(BinaryWriter payload, bool writeQuaternion = true, bool writeLandblock = true)
        {
            if (writeLandblock)
                payload.Write(ObjCellID);

            payload.Write(Pos.X);
            payload.Write(Pos.Y);
            payload.Write(Pos.Z);

            if (writeQuaternion)
            {
                payload.Write(Rotation.W);
                payload.Write(Rotation.X);
                payload.Write(Rotation.Y);
                payload.Write(Rotation.Z);
            }
        }

        /// <summary>
        /// Returns the 3D distance between 2 objects
        /// </summary>
        public float DistanceTo(Position p)
        {
            if (Landblock == p.Landblock)
            {
                return Vector3.Distance(Pos, p.Pos);
            }
            else
            {
                var dx = (LandblockX - p.LandblockX) * 192 + Pos.X - p.Pos.X;
                var dy = (LandblockY - p.LandblockY) * 192 + Pos.Y - p.Pos.Y;

                var dz = Pos.Z - p.Pos.Z;

                return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            }
        }

        /// <summary>
        /// Returns the 3D squared distance between 2 objects
        /// </summary>
        public float SquaredDistanceTo(Position p)
        {
            if (Landblock == p.Landblock)
            {
                return Vector3.DistanceSquared(Pos, p.Pos);
            }
            else
            {
                var dx = (LandblockX - p.LandblockX) * 192 + Pos.X - p.Pos.X;
                var dy = (LandblockY - p.LandblockY) * 192 + Pos.Y - p.Pos.Y;

                var dz = Pos.Z - p.Pos.Z;

                return dx * dx + dy * dy + dz * dz;
            }
        }

        /// <summary>
        /// Returns the 2D distance between 2 objects
        /// </summary>
        public float Distance2D(Position p)
        {
            if (Landblock == p.Landblock)
            {
                var dx = Pos.X - p.Pos.X;
                var dy = Pos.Y - p.Pos.Y;

                return (float)Math.Sqrt(dx * dx + dy * dy);
            }
            else
            {
                var dx = (LandblockX - p.LandblockX) * 192 + Pos.X - p.Pos.X;
                var dy = (LandblockY - p.LandblockY) * 192 + Pos.Y - p.Pos.Y;

                return (float)Math.Sqrt(dx * dx + dy * dy);
            }
        }

        /// <summary>
        /// Returns the squared 2D distance between 2 objects
        /// </summary>
        public float Distance2DSquared(Position p)
        {
            if (Landblock == p.Landblock)
            {
                var dx = Pos.X - p.Pos.X;
                var dy = Pos.Y - p.Pos.Y;

                return dx * dx + dy * dy;
            }
            else
            {
                var dx = (LandblockX - p.LandblockX) * 192 + Pos.X - p.Pos.X;
                var dy = (LandblockY - p.LandblockY) * 192 + Pos.Y - p.Pos.Y;

                return dx * dx + dy * dy;
            }
        }

        public override string ToString()
        {
            return $"{ObjCellID:X8} [{Pos.X} {Pos.Y} {Pos.Z}]";
        }

        public string ToLOCString()
        {
            return $"0x{ObjCellID:X8} [{Pos.X} {Pos.Y} {Pos.Z}] {Rotation.W} {Rotation.X} {Rotation.Y} {Rotation.Z}";
        }

        public bool Equals(Position p)
        {
            return ObjCellID == p.ObjCellID && Pos == p.Pos && Rotation == p.Rotation;
        }
    }
}
