using System;
using System.Collections.Generic;
using System.Numerics;
using ACE.Server.PerfStats;
using ACE.Server.Physics.Common;
using ACE.Server.Physics.Animation;

namespace ACE.Server.Physics.Collision
{
    public class CollisionInfo
    {
#if METHODSTATISTICS
        public static readonly System.Type ThisType = typeof(CollisionInfo);
#endif
        public bool LastKnownContactPlaneValid;
        public Plane LastKnownContactPlane;
        public bool LastKnownContactPlaneIsWater;
        public bool ContactPlaneValid;
        public Plane ContactPlane;
        public uint ContactPlaneCellID;
        public uint LastKnownContactPlaneCellID;
        public bool ContactPlaneIsWater;
        public bool SlidingNormalValid;
        public Vector3 SlidingNormal;
        public bool CollisionNormalValid;
        public Vector3 CollisionNormal;
        public Vector3 AdjustOffset;
        public int NumCollideObject;
        public List<PhysicsObj> CollideObject;
        public PhysicsObj LastCollidedObject;
        public bool CollidedWithEnvironment;
        public int FramesStationaryFall;

        // custom for server
        public bool VerifiedRestrictions;

        public CollisionInfo()
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "ctor()");
#endif
            Init();
        }

        public void Init()
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "Init()");
#endif
            CollideObject = new List<PhysicsObj>();
        }

        public void SetContactPlane(Plane plane, bool isWater)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "SetContactPlane(Plane, bool)");
#endif
            ContactPlaneValid = true;
            ContactPlane = new Plane(plane.Normal, plane.D);
            ContactPlaneIsWater = isWater;
        }

        public void SetCollisionNormal(Vector3 normal)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "SetCollisionNormal(Vector3)");
#endif
            CollisionNormalValid = true;
            CollisionNormal = normal;   // use original?
            if (Vec.NormalizeCheckSmall(ref normal))
                CollisionNormal = Vector3.Zero;
        }

        public void SetSlidingNormal(Vector3 normal)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "SetSlidingNormal(Vector3)");
#endif
            SlidingNormalValid = true;
            SlidingNormal = new Vector3(normal.X, normal.Y, 0.0f);
            if (Vec.NormalizeCheckSmall(ref normal))
                SlidingNormal = Vector3.Zero;
        }

        public void AddObject(PhysicsObj obj, TransitionState state)
        {
#if METHODSTATISTICS
            MethodStatistics.Increment(ThisType, "AddObject(PhysicsObj, TransitionState)");
#endif
            if (CollideObject.Contains(obj))
                return;

            CollideObject.Add(obj);
            NumCollideObject++;

            if (state != TransitionState.OK)
                LastCollidedObject = obj;
        }
    }
}
