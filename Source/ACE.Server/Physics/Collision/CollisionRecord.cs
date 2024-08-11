using ACE.Server.PerfStats;

namespace ACE.Server.Physics.Collision
{
    public class CollisionRecord
    {
#if METHODSTATISTICS
        public static readonly System.Type ThisType = typeof(CollisionRecord);
#endif

        public double TouchedTime;
        public bool Ethereal;

        public CollisionRecord(double touchedTime, bool ethereal)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "ctor(double, bool)");
#endif
            TouchedTime = touchedTime;
            Ethereal = ethereal;
        }
    }
}
