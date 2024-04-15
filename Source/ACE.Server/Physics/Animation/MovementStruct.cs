using ACE.Entity.Enum;
using ACE.Server.Physics.Common;

namespace ACE.Server.Physics.Animation
{
    public class MovementStruct
    {
        public MovementType Type;
        public uint Motion;
        public ulong ObjectId;
        public ulong TopLevelId;
        public PhysicsPosition Position;
        public float Radius;
        public float Height;
        public MovementParameters Params;

        public MovementStruct() { }

        public MovementStruct(MovementType type)
        {
            Type = type;
        }

        public MovementStruct(MovementType type, uint motion, MovementParameters movementParams)
        {
            Type = type;
            Motion = motion;
            Params = movementParams;
        }
    }
}
