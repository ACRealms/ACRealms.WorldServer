using System.Numerics;
using ACE.Server.PerfStats;

namespace ACE.Server.Physics.Collision
{
    public enum EnvCollisionProfileFlags
    {
        Undefined = 0x0,
        MyContact = 0x1,
    };

    public class EnvCollisionProfile
    {
#if METHODSTATISTICS
        public static readonly System.Type ThisType = typeof(EnvCollisionProfile);
#endif
        public Vector3 Velocity;
        public EnvCollisionProfileFlags Flags;

        public void SetMeInContact(bool hasContact)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "SetMeInContact(bool)");
#endif

            if (hasContact)
                Flags = EnvCollisionProfileFlags.MyContact;
            else
                Flags = EnvCollisionProfileFlags.Undefined;
        }
    }
}
