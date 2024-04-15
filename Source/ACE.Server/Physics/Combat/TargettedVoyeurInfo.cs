using ACE.Server.Physics.Common;

namespace ACE.Server.Physics.Combat
{
    public class TargettedVoyeurInfo
    {
        public ulong ObjectID;
        public double Quantum;
        public float Radius;
        public PhysicsPosition LastSentPosition;

        public TargettedVoyeurInfo() { }

        public TargettedVoyeurInfo(ulong objectID, float radius, double quantum)
        {
            ObjectID = objectID;
            Quantum = quantum;
            Radius = radius;
        }
    }
}
