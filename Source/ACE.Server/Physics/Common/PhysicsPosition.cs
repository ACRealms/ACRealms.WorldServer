using System;
using System.Numerics;

using ACE.Entity.Enum;
using ACE.Server.PerfStats;
using ACE.Server.Physics.Animation;
using ACE.Server.Physics.Extensions;
using ACE.Server.Physics.Util;

namespace ACE.Server.Physics.Common
{
    public class PhysicsPosition: IEquatable<PhysicsPosition>
    {
#if METHODSTATISTICS
        public static readonly Type ThisType = typeof(PhysicsPosition);
#endif

        public uint ObjCellID;
        public AFrame Frame;

        public uint Landblock => ObjCellID >> 16;

        public uint LandblockX => ObjCellID >> 24;

        public uint LandblockY => (ObjCellID >> 16) & 0xFF;

        public PhysicsPosition()
        {
            Init();
        }

        public PhysicsPosition(uint objCellID)
        {
            ObjCellID = objCellID;
            Init();
        }

        public PhysicsPosition(uint objCellID, AFrame frame)
        {
            ObjCellID = objCellID;
            Frame = frame;
        }

        public PhysicsPosition(PhysicsPosition p)
        {
            if (p == null)
            {
                Console.WriteLine("Position copy constructor - null");
                Frame = new AFrame();
                return;
            }

            ObjCellID = p.ObjCellID;
            Frame = new AFrame(p.Frame);
        }

        public PhysicsPosition(Realms.InstancedPosition p)
        {
            ObjCellID = p.Cell;
            Frame = new AFrame(p.Pos, p.Rotation);
        }

        public void Init()
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "Init()");
#endif
            Frame = new AFrame();
        }

        public Vector3 LocalToLocal(PhysicsPosition pos, Vector3 offset)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "LocalToLocal(PhysicsPosition, Vector3)");
#endif
            var cellOffset = pos.Frame.LocalToGlobal(offset);
            var blockOffset = LandDefs.GetBlockOffset(ObjCellID, pos.ObjCellID);

            return Frame.GlobalToLocal(blockOffset + cellOffset);
        }

        public Vector3 LocalToGlobal(Vector3 point)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "LocalToGlobal(Vector3)");
#endif
            return Frame.LocalToGlobal(point);
        }

        public Vector3 LocalToGlobal(PhysicsPosition pos, Vector3 point)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "LocalToGlobal(PhysicsPosition, Vector3)");
#endif
            var cellOffset = pos.Frame.LocalToGlobal(point);
            var blockOffset = LandDefs.GetBlockOffset(ObjCellID, pos.ObjCellID);

            return blockOffset + cellOffset;
        }

        public Vector3 LocalToGlobalVec(Vector3 point)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "LocalToGlobalVec(Vector3)");
#endif
            return Frame.LocalToGlobalVec(point);
        }

        public float Distance(PhysicsPosition pos)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "Distance(PhysicsPosition)");
#endif
            return GetOffset(pos).Length();
        }

        public float DistanceSquared(PhysicsPosition pos)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "DistanceSquared(PhysicsPosition)");
#endif
            return GetOffset(pos).LengthSquared();
        }

        public static double CylinderDistance(float radius, float height, PhysicsPosition pos, float otherRadius, float otherHeight, PhysicsPosition otherPos)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "CylinderDistance(float, float, PhysicsPosition, float, float, PhysicsPosition)");
#endif
            var offset = pos.GetOffset(otherPos);
            var reach = offset.Length() - (radius + otherRadius);

            var diffZ = pos.Frame.Origin.Z <= otherPos.Frame.Origin.Z ? otherPos.Frame.Origin.Z - (pos.Frame.Origin.Z + height) :
                pos.Frame.Origin.Z - (otherPos.Frame.Origin.Z + otherHeight);

            if (diffZ > 0 && reach > 0)
                return Math.Sqrt(diffZ * diffZ + reach * reach);
            else if (diffZ < 0 && reach < 0)
                return -Math.Sqrt(diffZ * diffZ + reach * reach);
            else
                return reach;
        }

        // custom, based on above
        public static double CylinderDistanceSq(float radius, float height, PhysicsPosition pos, float otherRadius, float otherHeight, PhysicsPosition otherPos)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "CylinderDistanceSq(float, float, PhysicsPosition, float, float, PhysicsPosition)");
#endif
            var offset = pos.GetOffset(otherPos);
            var reach = offset.Length() - (radius + otherRadius);

            var diffZ = pos.Frame.Origin.Z <= otherPos.Frame.Origin.Z ? otherPos.Frame.Origin.Z - (pos.Frame.Origin.Z + height) :
                pos.Frame.Origin.Z - (otherPos.Frame.Origin.Z + otherHeight);

            if (diffZ > 0 && reach > 0)
                return diffZ * diffZ + reach * reach;
            else if (diffZ < 0 && reach < 0)
                return -(diffZ * diffZ + reach * reach);
            else
                return reach * reach;
        }

        public static float CylinderDistanceNoZ(float radius, PhysicsPosition pos, float otherRadius, PhysicsPosition otherPos)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "CylinderDistanceNoZ(float, PhysicsPosition, float, PhysicsPosition)");
#endif
            var offset = pos.GetOffset(otherPos);
            return offset.Length() - (radius + otherRadius);
        }

        public const float ThresholdMed = 1.0f / 3.0f;
        public const float ThresholdHigh = 2.0f / 3.0f;

        public Quadrant DetermineQuadrant(float height, PhysicsPosition position)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "DetermineQuadrant(float, PhysicsPosition)");
#endif
            var hitpoint = LocalToLocal(position, Vector3.Zero);

            var quadrant = hitpoint.X < 0.0f ? Quadrant.Left : Quadrant.Right;

            quadrant |= hitpoint.Y >= 0.0f ? Quadrant.Front : Quadrant.Back;

            if (hitpoint.Z < height * ThresholdMed)
                quadrant |= Quadrant.Low;
            else if (hitpoint.Z < height * ThresholdHigh)
                quadrant |= Quadrant.Medium;
            else
                quadrant |= Quadrant.High;

            return quadrant;
        }

        public Vector3 GlobalToLocalVec(Vector3 point)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "GlobalToLocalVec(Vector3)");
#endif
            return Frame.GlobalToLocalVec(point);
        }

        public Vector3 GetOffset(PhysicsPosition pos)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "GetOffset(PhysicsPosition)");
#endif
            var blockOffset = LandDefs.GetBlockOffset(ObjCellID, pos.ObjCellID);
            var globalOffset = blockOffset + pos.Frame.Origin - Frame.Origin;

            return globalOffset;
        }

        public Vector3 GetOffset(PhysicsPosition pos, Vector3 rotation)
        {
            var blockOffset = LandDefs.GetBlockOffset(ObjCellID, pos.ObjCellID);
            return blockOffset + pos.LocalToGlobal(rotation) - Frame.Origin;
        }

        public Vector3 GetOrigin()
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "GetOrigin()");
#endif
            return Frame.Origin;
        }

        public AFrame Subtract(PhysicsPosition pos)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "Subtract(PhysicsPosition)");
#endif
            Frame.Subtract(pos.Frame);
            return Frame;
        }

        public void add_offset(Vector3 offset)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "add_offset(Vector3)");
#endif
            Frame.Origin += offset;
        }

        public void adjust_to_outside()
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "adjust_to_outside()");
#endif
            LandDefs.AdjustToOutside(this);
        }

        public float heading(PhysicsPosition position)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "heading(PhysicsPosition)");
#endif
            var dir = GetOffset(position);
            dir.Z = 0.0f;

            if (Vec.NormalizeCheckSmall(ref dir))
                return 0.0f;

            return (450.0f - ((float)Math.Atan2(dir.Y, dir.X)).ToDegrees()) % 360.0f;
        }

        public float heading_diff(PhysicsPosition position)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "heading_diff(PhysicsPosition)");
#endif
            return heading(position) - Frame.get_heading();
        }

        public uint GetCell(uint blockCellID, uint instance)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "GetCell(uint, uint)");
#endif
            // is originating cell indoor or outdoor?
            if ((blockCellID & 0xFFFF) >= 0x100)
                return GetIndoorCell(blockCellID, instance);
            else
                return GetOutdoorCell(blockCellID, instance);
        }

        public uint GetOutdoorCell(uint blockCellID, uint instance)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "GetOutdoorCell(uint, uint)");
#endif
            var cellX = (uint)(Frame.Origin.X / LandDefs.CellLength);
            var cellY = (uint)(Frame.Origin.Y / LandDefs.CellLength);

            var cellID = (uint)(cellX * LandDefs.BlockSide + cellY + 1);

            var newBlockCellID = blockCellID & 0xFFFF0000 | cellID;

            //return LScape.get_landcell(blockCellID);
            return newBlockCellID;
            //return cellID;
        }

        public uint GetIndoorCell(uint blockCellID, uint instance)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "GetIndoorCell(uint, uint)");
#endif
            var dungeonID = blockCellID >> 16;

            var adjustCell = AdjustCell.Get(dungeonID, instance);
            if (adjustCell == null)
            {
                //Console.WriteLine("Position: couldn't find ObjCellID for indoor cell " + blockCellID.ToString("X8"));
                //return LScape.get_landcell(ObjCellID);
                return ObjCellID;
            }
            var newCell = adjustCell.GetCell(Frame.Origin);
            if (newCell == null)
            {
                //Console.WriteLine("Position: couldn't find new cell for indoor cell " + blockCellID.ToString("X8"));
                //return LScape.get_landcell(ObjCellID);
                return ObjCellID;
            }
            //return LScape.get_landcell(newCell.Value);
            return newCell.Value;
        }

        /// <summary>
        /// Returns the squared 2D distance between 2 positions
        /// </summary>
        public float Distance2DSquared(PhysicsPosition p)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "Distance2DSquared(PhysicsPosition)");
#endif
            if (Landblock == p.Landblock)
            {
                var dx = Frame.Origin.X - p.Frame.Origin.X;
                var dy = Frame.Origin.Y - p.Frame.Origin.Y;
                return dx * dx + dy * dy;
            }
            else
            {
                var dx = ((int)LandblockX - p.LandblockX) * 192 + Frame.Origin.X - p.Frame.Origin.X;
                var dy = ((int)LandblockY - p.LandblockY) * 192 + Frame.Origin.Y - p.Frame.Origin.Y;
                return dx * dx + dy * dy;
            }
        }

        public bool Equals(PhysicsPosition pos)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "Equals(PhysicsPosition)");
#endif
            return ObjCellID == pos.ObjCellID && Frame.Equals(pos.Frame);
        }

        public override string ToString()
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "ToString()");
#endif
            return $"0x{ObjCellID:X8} {Frame}";
        }

        public string ShortLoc()
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "ShortLoc()");
#endif
            return $"0x{ObjCellID:X8} [{Frame.Origin.X} {Frame.Origin.Y} {Frame.Origin.Z}]";
        }
    }
}
