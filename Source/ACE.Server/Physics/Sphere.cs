using System;
using System.Numerics;

using ACE.Entity.Enum;

using ACE.Server.Physics.Animation;
using ACE.Server.Physics.Common;
using ACE.Server.Physics.Extensions;

namespace ACE.Server.Physics
{
    /// <summary>
    /// Spherical collision detection
    /// </summary>
    public class Sphere: IEquatable<Sphere>
    {
        /// <summary>
        /// The center point of the sphere
        /// </summary>
        public Vector3 Center;

        /// <summary>
        /// The radius of the sphere
        /// </summary>
        public float Radius;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Sphere()
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public Sphere(Sphere sphere)
        {
            Center = sphere.Center;
            Radius = sphere.Radius;
        }

        /// <summary>
        /// Constructs a sphere from a center point and radius
        /// </summary>
        /// <param name="center">The center point of the sphere</param>
        /// <param name="radius">The radius of the sphere</param>
        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Constructs a sphere loaded from portal.dat
        /// </summary>
        public Sphere(DatLoader.Entity.Sphere sphere)
        {
            Center = sphere.Origin;
            Radius = sphere.Radius;
        }

        public const float ThresholdMed = 1.0f / 3.0f;
        public const float ThresholdHigh = 2.0f / 3.0f;

        public static Quadrant Attack(PhysicsPosition targetPos, float targetRadius, float targetHeight, PhysicsPosition attackPos, Vector2 left, Vector2 right, float attackRadius, float attackHeight)
        {
            var center = attackPos.LocalToLocal(targetPos, Vector3.Zero);

            if (attackHeight < 0.0f || attackHeight > targetHeight)
                return Quadrant.None;

            var radsum = targetRadius + attackRadius;

            var distSq = center.LengthSquared2D();
            if (distSq > radsum * radsum)
                return Quadrant.None;

            var hitLoc = targetPos.LocalToLocal(attackPos, Vector3.Zero);

            var quadrant = hitLoc.X <= 0.0f ? Quadrant.Left : Quadrant.Right;

            quadrant |= hitLoc.Y > 0.0f ? Quadrant.Front : Quadrant.Back;

            if (attackHeight < targetHeight * ThresholdMed)
                quadrant |= Quadrant.Low;
            else if (attackHeight < targetHeight * ThresholdHigh)
                quadrant |= Quadrant.Medium;
            else
                quadrant |= Quadrant.High;

            // 2d cross product?
            var attack_ht = center.Y * left.X - center.X * left.Y;
            var right_dist = center.X * right.Y - center.Y * right.X;

            if (attack_ht <= 0.0f && right_dist <= 0.0f)
                return quadrant;

            if (left.X * right.Y - left.Y * right.X >= 0.0f)
            {
                if (right_dist * attack_ht <= 0.0f || attack_ht <= targetRadius || right_dist <= targetRadius)
                    return quadrant;
                else
                    return Quadrant.None;
            }

            if (attack_ht < 0.0f)
            {
                if (right_dist <= targetRadius)
                    return quadrant;
                else
                    return Quadrant.None;
            }

            if (right_dist >= 0.0f)
            {
                if (distSq <= targetRadius * targetRadius)
                    return quadrant;
                else
                    return Quadrant.None;
            }

            if (attack_ht < 0.0f)
            {
                if (right_dist <= targetRadius)
                    return quadrant;
                else
                    return Quadrant.None;
            }

            if (attack_ht <= targetRadius)
                return quadrant;
            else
                return Quadrant.None;
        }

        /// <summary>
        /// Redirects a sphere to be on collision course towards a point
        /// </summary>
        /// <param name="transition">The transition information for the sphere</param>
        /// <param name="checkPos">The spherical point to redirect towards</param>
        /// <param name="disp">Currently doesn't seem to be used?</param>
        /// <param name="radsum">The sum of the sphere and spherical point radii</param>
        /// <param name="sphereNum">Used as an offset in path.GlobalCurrCenter to determine movement</param>
        /// <returns>The TransitionState either collided or adjusted</returns>
        [Obsolete("Use CollideWithPoint without disp")]
        public TransitionState CollideWithPoint(Transition transition, Sphere checkPos, Vector3 disp, float radsum, int sphereNum)
        {
            return CollideWithPoint(transition, checkPos, radsum, sphereNum);
        }

        /// <summary>
        /// Redirects a sphere to be on collision course towards a point
        /// </summary>
        /// <param name="transition">The transition information for the sphere</param>
        /// <param name="checkPos">The spherical point to redirect towards</param>
        /// <param name="disp">Currently doesn't seem to be used?</param>
        /// <param name="radsum">The sum of the sphere and spherical point radii</param>
        /// <param name="sphereNum">Used as an offset in path.GlobalCurrCenter to determine movement</param>
        /// <returns>The TransitionState either collided or adjusted</returns>
        public TransitionState CollideWithPoint(Transition transition, Sphere checkPos, float radsum, int sphereNum)
        {
            return CollideWithPoint(Center, transition, checkPos, radsum, sphereNum);
        }

        /// <summary>
        /// Redirects a sphere to be on collision course towards a point
        /// </summary>
        /// <param name="transition">The transition information for the sphere</param>
        /// <param name="checkPos">The spherical point to redirect towards</param>
        /// <param name="disp">Currently doesn't seem to be used?</param>
        /// <param name="radsum">The sum of the sphere and spherical point radii</param>
        /// <param name="sphereNum">Used as an offset in path.GlobalCurrCenter to determine movement</param>
        /// <returns>The TransitionState either collided or adjusted</returns>
        public static TransitionState CollideWithPoint(Vector3 center, Transition transition, Sphere checkPos, float radsum, int sphereNum)
        {
            var obj = transition.ObjectInfo;
            var path = transition.SpherePath;
            var collisions = transition.CollisionInfo;

            var gCenter = path.GlobalCurrCenter[sphereNum].Center;
            var globalOffset = gCenter - center;

            // if set to PerfectClip, does a more precise check
            if (obj.State.HasFlag(ObjectInfoState.PerfectClip))
            {
                var blockOffset = LandDefs.GetBlockOffset(path.CurPos.ObjCellID, path.CheckPos.ObjCellID);
                var checkOffset = checkPos.Center - gCenter + blockOffset;
                var collisionTime = FindTimeOfCollision(checkOffset, globalOffset, radsum + PhysicsGlobals.EPSILON);
                if (collisionTime < PhysicsGlobals.EPSILON || collisionTime > 1.0f)
                    return TransitionState.Collided;
                else
                {
                    var collisionOffset = checkOffset * (float)collisionTime - checkOffset;
                    var old_disp = collisionOffset + checkPos.Center - center;
                    var invRad = 1.0f / radsum;
                    var collision_normal = old_disp * invRad;
                    collisions.SetCollisionNormal(collision_normal);
                    path.AddOffsetToCheckPos(old_disp, checkPos.Radius);
                    return TransitionState.Adjusted;
                }
            }
            else
            {
                if (!Vec.NormalizeCheckSmall(ref globalOffset))
                    collisions.SetCollisionNormal(globalOffset);

                return TransitionState.Collided;
            }
        }

        /// <summary>
        /// Collision detection from legacy implementation
        /// </summary>
        public static bool CollidesWithSphere(Vector3 otherSphere, float radsum)
        {
            // original implementation with FPU flag

            // is touching/equal considered a collision here?
            return otherSphere.LengthSquared() <= radsum * radsum;
        }

        /// <summary>
        /// Finds the percentage along this sphere's movement path
        ///  if/ when it collides with another sphere
        /// </summary>
        /// <param name="movement">The movement vector for the current sphere</param>
        /// <param name="spherePos">The position of the other sphere</param>
        /// <param name="radSum">The sum of the radii between this sphere and the other sphere</param>
        /// <returns>A 0-1 interval along the movement path for the time of collision, or -1 non-collision</returns>
        /// <remarks>Verify this could be different from original AC, which seems to return a negative interval?</remarks>
        public static double FindTimeOfCollision(Vector3 movement, Vector3 spherePos, float radSum)
        {
            var distSq = movement.LengthSquared();
            if (distSq < PhysicsGlobals.EPSILON) return -1;

            var nonCollide = spherePos.LengthSquared() - radSum * radSum;
            if (nonCollide < PhysicsGlobals.EPSILON) return -1;

            var similar = -Vector3.Dot(spherePos, movement);
            var nonCollideB = similar * similar - nonCollide * distSq;
            if (nonCollideB < 0) return -1;

            var cDist = Math.Sqrt(nonCollideB);
            if (similar - cDist < 0)
                return -1 * (cDist + similar) / distSq;
            else
                return -1 * (similar - cDist) / distSq;
        }

        /// <summary>
        /// Returns true if this sphere intersects with another sphere
        /// </summary>
        public bool Intersects(Sphere sphere)
        {
            var delta = sphere.Center - Center;
            var radSum = Radius + sphere.Radius;
            return delta.LengthSquared() < radSum * radSum;
        }

        /// <summary>
        /// Determines if this sphere collides with any other spheres during its transitions
        /// </summary>
        public TransitionState IntersectsSphere(PhysicsPosition position, float scale, Transition transition, bool isCreature)
        {
            var globPos = transition.SpherePath.CheckPos.LocalToGlobal(position, Center * scale);
            return IntersectsSphere(globPos, Radius * scale, transition, isCreature);
        }

        /// <summary>
        /// Determines if this sphere collides with anything during its transition<para />
        /// Note: For high load scenarios, consider the static function to avoid excess heap usage: IntersectsSphere(Vector3 center, float radius, Transition transition, bool isCreature)
        /// </summary>
        /// <param name="transition">The transition path for this sphere</param>
        /// <param name="isCreature">Flag indicates if this sphere is a player / monster</param>
        /// <returns>The collision result for this transition path</returns>
        public TransitionState IntersectsSphere(Transition transition, bool isCreature)
        {
            return IntersectsSphere(Center, Radius, transition, isCreature);
        }

        /// <summary>
        /// Determines if this sphere collides with anything during its transition
        /// </summary>
        /// <param name="transition">The transition path for this sphere</param>
        /// <param name="isCreature">Flag indicates if this sphere is a player / monster</param>
        /// <returns>The collision result for this transition path</returns>
        public static TransitionState IntersectsSphere(Vector3 center, float radius, Transition transition, bool isCreature)
        {
            var globSphere = transition.SpherePath.GlobalSphere[0];
            var disp = globSphere.Center - center;

            Sphere globSphere_ = null;
            Vector3 disp_ = Vector3.Zero;

            if (transition.SpherePath.NumSphere > 1)
            {
                globSphere_ = transition.SpherePath.GlobalSphere[1];
                disp_ = globSphere_.Center - center;
            }

            var radsum = globSphere.Radius + radius - PhysicsGlobals.EPSILON;

            if (transition.SpherePath.ObstructionEthereal || transition.SpherePath.InsertType == InsertType.Placement)
            {
                if (disp.LengthSquared() <= radsum * radsum)
                    return TransitionState.Collided;

                if (transition.SpherePath.NumSphere > 1)
                {
                    if (CollidesWithSphere(disp_, radsum))
                        return TransitionState.Collided;
                }
                return TransitionState.OK;
            }

            if (transition.SpherePath.StepDown)
            {
                if (isCreature)
                    return TransitionState.OK;

                return StepSphereDown(center, radius, transition, globSphere, ref disp, radsum);
            }

            if (transition.SpherePath.CheckWalkable)
            {
                if (CollidesWithSphere(disp, radsum))
                    return TransitionState.Collided;

                if (transition.SpherePath.NumSphere > 1)
                {
                    if (CollidesWithSphere(disp_, radsum))
                        return TransitionState.Collided;
                }
                return TransitionState.OK;
            }

            if (!transition.SpherePath.Collide)
            {
                if (transition.ObjectInfo.State.HasFlag(ObjectInfoState.Contact) || transition.ObjectInfo.State.HasFlag(ObjectInfoState.OnWalkable))
                {
                    if (CollidesWithSphere(disp, radsum))
                        return StepSphereUp(center, transition, disp, radsum);

                    if (transition.SpherePath.NumSphere > 1)
                    {
                        if (CollidesWithSphere(disp_, radsum))
                            return SlideSphere(center, transition, globSphere_, 1);
                    }
                    return TransitionState.OK;
                }
                else if (transition.ObjectInfo.State.HasFlag(ObjectInfoState.PathClipped))
                {
                    if (CollidesWithSphere(disp, radsum))
                        return CollideWithPoint(center, transition, globSphere, radsum, 0);
                }
                else
                {
                    if (CollidesWithSphere(disp, radsum))
                        return LandOnSphere(center, transition);

                    if (transition.SpherePath.NumSphere > 1)
                    {
                        if (CollidesWithSphere(disp_, radsum))
                            return CollideWithPoint(center, transition, globSphere_, radsum, 1);
                    }
                }
                return TransitionState.OK;
            }

            if (isCreature)
                return TransitionState.OK;

            if (!CollidesWithSphere(disp, radsum))
            {
                if (transition.SpherePath.NumSphere > 1)
                {
                    if (!CollidesWithSphere(disp_, radsum))
                        return TransitionState.OK;
                }
            }

            // handles movement interpolation
            var blockOffset = transition.SpherePath.GetCurPosCheckPosBlockOffset();
            var movement = transition.SpherePath.GlobalCurrCenter[0].Center - globSphere.Center - blockOffset;
            radsum += PhysicsGlobals.EPSILON;
            var lenSq = movement.LengthSquared();
            var diff = -Vector3.Dot(movement, disp);
            if (Math.Abs(lenSq) < PhysicsGlobals.EPSILON)
                return TransitionState.Collided;

            var t = Math.Sqrt(diff * diff - (disp.LengthSquared() - radsum * radsum) * lenSq) + diff;   // solve for t
            if (t > 1)
                t = diff * 2 - t;
            var time = (float)t / lenSq;
            var timecheck = (1 - time) * transition.SpherePath.WalkInterp;
            if (timecheck >= transition.SpherePath.WalkInterp || timecheck < -0.1f)
                return TransitionState.Collided;

            movement *= time;
            disp = (disp + movement) / radsum;

            if (!transition.SpherePath.IsWalkableAllowable(disp.Z))
                return TransitionState.OK;

            var disp2 = globSphere.Center - disp * globSphere.Radius;

            var contactPlane = new Plane(disp, -Vector3.Dot(disp, disp2));
            transition.CollisionInfo.SetContactPlane(contactPlane, true);
            transition.CollisionInfo.ContactPlaneCellID = transition.SpherePath.CheckPos.ObjCellID;
            transition.SpherePath.WalkInterp = timecheck;
            transition.SpherePath.AddOffsetToCheckPos(movement, globSphere.Radius);

            return TransitionState.Adjusted;
        }

        /// <summary>
        /// Handles the collision when an object lands on a sphere
        /// </summary>
        [Obsolete("Use LandOnShere without checkPos, disp, radsum")]
        public TransitionState LandOnSphere(Transition transition, Sphere checkPos, Vector3 disp, float radsum)
        {
            return LandOnSphere(transition);
        }

        /// <summary>
        /// Handles the collision when an object lands on a sphere
        /// </summary>
        public TransitionState LandOnSphere(Transition transition)
        {
            return LandOnSphere(Center, transition);
        }

        /// <summary>
        /// Handles the collision when an object lands on a sphere
        /// </summary>
        public static TransitionState LandOnSphere(Vector3 center, Transition transition)
        {
            var path = transition.SpherePath;

            var collisionNormal = path.GlobalCurrCenter[0].Center - center;

            if (Vec.NormalizeCheckSmall(ref collisionNormal))
                return TransitionState.Collided;
            else
            {
                path.SetCollide(collisionNormal);
                path.WalkableAllowance = PhysicsGlobals.LandingZ;
                return TransitionState.Adjusted;
            }
        }

        /// <summary>
        /// Attempts to slide the sphere from a collision
        /// </summary>
        [Obsolete("Use SlideSphere without radsum")]
        public TransitionState SlideSphere(Transition transition, Vector3 disp, float radsum, int sphereNum)
        {
            return SlideSphere(transition, disp, sphereNum);
        }

        /// <summary>
        /// Attempts to slide the sphere from a collision
        /// </summary>
        public TransitionState SlideSphere(Transition transition, Vector3 disp, int sphereNum)
        {
            return SlideSphere(Center, transition, disp, sphereNum);
        }

        /// <summary>
        /// Attempts to slide the sphere from a collision
        /// </summary>
        public static TransitionState SlideSphere(Vector3 center, Transition transition, Vector3 disp, int sphereNum)
        {
            var path = transition.SpherePath;
            var collisions = transition.CollisionInfo;

            var globSphere = path.GlobalSphere[sphereNum];

            var collisionNormal = path.GlobalCurrCenter[sphereNum].Center - center;
            if (Vec.NormalizeCheckSmall(ref collisionNormal))
                return TransitionState.Collided;

            collisions.SetCollisionNormal(collisionNormal);

            var contactPlane = collisions.ContactPlaneValid ? collisions.ContactPlane : collisions.LastKnownContactPlane;
            var skid_dir = contactPlane.Normal;
            //var direction = Vector3.Cross(skid_dir, collisionNormal);
            var direction = Vector3.Cross(collisionNormal, skid_dir);

            var blockOffset = LandDefs.GetBlockOffset(path.CurPos.ObjCellID, path.CheckPos.ObjCellID);
            var globOffset = globSphere.Center - path.GlobalCurrCenter[sphereNum].Center + blockOffset;
            var dirLenSq = direction.LengthSquared();
            if (dirLenSq >= PhysicsGlobals.EPSILON)
            {
                skid_dir = Vector3.Dot(globOffset, direction) * direction;

                var invDirLenSq = 1.0f / dirLenSq;

                //skid_dir *= invDirLenSq * invDirLenSq;
                skid_dir *= invDirLenSq;
                direction = skid_dir;

                // only x?
                //if (direction.X * direction.X < PhysicsGlobals.EPSILON)
                if (direction.LengthSquared() < PhysicsGlobals.EPSILON)
                    return TransitionState.Collided;

                direction -= globOffset;

                path.AddOffsetToCheckPos(direction, globSphere.Radius);
                return TransitionState.Slid;
            }

            if (Vector3.Dot(skid_dir, disp) < 0.0f)
                return TransitionState.Collided;

            direction = -Vector3.Dot(globOffset, collisionNormal) * collisionNormal;
            path.AddOffsetToCheckPos(direction, globSphere.Radius);
            return TransitionState.Slid;
        }

        /// <summary>
        /// Attempts to slide a sphere from a collision
        /// </summary>
        [Obsolete("Use SlideSphere without disp and radsum")]
        public TransitionState SlideSphere(Transition transition, Sphere checkPos, Vector3 disp, float radsum, int sphereNum)
        {
            return SlideSphere(transition, checkPos, sphereNum);
        }

        /// <summary>
        /// Attempts to slide a sphere from a collision
        /// </summary>
        public TransitionState SlideSphere(Transition transition, Sphere checkPos, int sphereNum)
        {
            return SlideSphere(Center, transition, checkPos, sphereNum);
        }

        /// <summary>
        /// Attempts to slide a sphere from a collision
        /// </summary>
        public static TransitionState SlideSphere(Vector3 center, Transition transition, Sphere checkPos, int sphereNum)
        {
            var globalCenter = transition.SpherePath.GlobalCurrCenter[sphereNum].Center;

            var collisionNormal = globalCenter - center;

            if (Vec.NormalizeCheckSmall(ref collisionNormal))
                return TransitionState.Collided;
            else
                return checkPos.SlideSphere(transition, ref collisionNormal, globalCenter);
        }

        /// <summary>
        /// Attempts to slide a sphere from a collision
        /// </summary>
        public TransitionState SlideSphere(Transition transition, ref Vector3 collisionNormal, Vector3 currPos)
        {
            var path = transition.SpherePath;
            var collisions = transition.CollisionInfo;

            if (collisionNormal.Equals(Vector3.Zero))
            {
                var halfOffset = (currPos - Center) * 0.5f;
                path.AddOffsetToCheckPos(halfOffset, Radius);
                return TransitionState.Adjusted;
            }

            collisions.SetCollisionNormal(collisionNormal);

            var blockOffset = LandDefs.GetBlockOffset(path.CurPos.ObjCellID, path.CheckPos.ObjCellID);
            var gDelta = blockOffset + (Center - currPos);

            var contactPlane = collisions.ContactPlaneValid ? collisions.ContactPlane : collisions.LastKnownContactPlane;
            var direction = Vector3.Cross(collisionNormal, contactPlane.Normal);
            var dirLenSq = direction.LengthSquared();
            if (dirLenSq >= PhysicsGlobals.EPSILON)
            {
                var diff = Vector3.Dot(direction, gDelta);
                var invDirLenSq = 1.0f / dirLenSq;
                var offset = direction * diff * invDirLenSq;
                if (offset.LengthSquared() < PhysicsGlobals.EPSILON)
                    return TransitionState.Collided;

                offset -= gDelta;
                path.AddOffsetToCheckPos(offset, Radius);
                return TransitionState.Slid;
            }

            if (Vector3.Dot(collisionNormal, contactPlane.Normal) >= 0.0f)
            {
                var diff = Vector3.Dot(collisionNormal, gDelta);
                var offset = -collisionNormal * diff;
                path.AddOffsetToCheckPos(offset, Radius);
                return TransitionState.Slid;
            }

            collisionNormal = -gDelta;
            if (!Vec.NormalizeCheckSmall(ref collisionNormal))
                collisions.SetCollisionNormal(collisionNormal);

            return TransitionState.OK;
        }

        /// <summary>
        /// Attempts to move the sphere down from a collision
        /// </summary>
        public TransitionState StepSphereDown(Transition transition, Sphere checkPos, ref Vector3 disp, float radsum)
        {
            return StepSphereDown(Center, Radius, transition, checkPos, ref disp, radsum);
        }

        /// <summary>
        /// Attempts to move the sphere down from a collision
        /// </summary>
        public static TransitionState StepSphereDown(Vector3 center, float radius, Transition transition, Sphere checkPos, ref Vector3 disp, float radsum)
        {
            var path = transition.SpherePath;
            var collisions = transition.CollisionInfo;

            if (!CollidesWithSphere(disp, radsum))
            {
                if (path.NumSphere <= 1)
                    return TransitionState.OK;

                var disp_ = path.GlobalSphere[1].Center - center;
                if (!CollidesWithSphere(disp_, radsum))
                    return TransitionState.OK;
            }

            var stepDown = path.StepDownAmt * path.WalkInterp;
            if (Math.Abs(stepDown) < PhysicsGlobals.EPSILON)
                return TransitionState.Collided;

            radsum += PhysicsGlobals.EPSILON;
            var val = Math.Sqrt(radsum * radsum - (disp.X * disp.X + disp.Y * disp.Y));
            var scaledStep = (float)(val - disp.Z) / stepDown;
            var timecheck = (1.0f - scaledStep) * path.WalkInterp;
            if (timecheck >= path.WalkInterp || timecheck < -0.1f)
                return TransitionState.Collided;

            var interp = stepDown * scaledStep;
            var invRadSum = 1.0f / radsum;

            // modifies disp?
            //var _disp = new Vector3(disp.X, disp.Y, disp.Z + interp) * invRadSum;
            disp = new Vector3(disp.X, disp.Y, disp.Z + interp) * invRadSum;

            if (disp.Z <= path.WalkableAllowance)
                return TransitionState.OK;

            var scaledDisp = disp * radius + center;
            var restPlane = new Plane(disp, -Vector3.Dot(disp, scaledDisp));

            collisions.SetContactPlane(restPlane, true);
            collisions.ContactPlaneCellID = path.CheckPos.ObjCellID;
            path.WalkInterp = timecheck;

            var offset = new Vector3(0, 0, interp);
            path.AddOffsetToCheckPos(offset, checkPos.Radius);

            return TransitionState.Adjusted;
        }

        /// <summary>
        /// Attempts to move the sphere up from a collision
        /// </summary>
        [Obsolete("Use override without checkPos")]
        public TransitionState StepSphereUp(Transition transition, Sphere checkPos, Vector3 disp, float radsum)
        {
            return StepSphereUp(transition, disp, radsum);
        }

        /// <summary>
        /// Attempts to move the sphere up from a collision
        /// </summary>
        public TransitionState StepSphereUp(Transition transition, Vector3 disp, float radsum)
        {
            return StepSphereUp(Center, transition, disp, radsum);
        }

        /// <summary>
        /// Attempts to move the sphere up from a collision
        /// </summary>
        public static TransitionState StepSphereUp(Vector3 center, Transition transition, Vector3 disp, float radsum)
        {
            radsum += PhysicsGlobals.EPSILON;
            if (transition.ObjectInfo.StepUpHeight < radsum - disp.Z)
                return SlideSphere(center, transition, disp, 0);
            else
            {
                var globCenter = transition.SpherePath.GlobalCurrCenter[0].Center;
                var collisionNormal = globCenter - center;
                if (transition.StepUp(collisionNormal))
                    return TransitionState.OK;
                else
                    return transition.SpherePath.StepUpSlide(transition);
            }
        }

        /// <summary>
        /// Detects if a ray intersects with a sphere
        /// </summary>
        /// <param name="ray">A ray is defined by a start point, a unit direction, and a length</param>
        /// <param name="timeOfIntersection">out parameter, the length of the ray when it hit the sphere</param>
        /// <returns>True if ray intersected, otherwise false.</returns>
        /// <remarks>
        /// - If the start point of the ray inside sphere, it is not considered an intersection.
        /// - If the sphere is behind the ray start point, with the ray direction pointing away from the sphere,
        ///   it can still return true for intersection, with timeOfIntersection as a negative value.
        /// </remarks>
        public bool SphereIntersectsRay(Ray ray, out double timeOfIntersection)
        {
            timeOfIntersection = 0;

            var distSq = ray.Dir.LengthSquared();
            if (distSq < PhysicsGlobals.EPSILON) return false;      // dir should be unit vector, redundant?

            // detect intersection
            var delta = ray.Point - Center;
            var c = delta.LengthSquared() - Radius * Radius;
            if (c <= 0) return false;

            // detect point of intersection
            var b = -Vector3.Dot(delta, ray.Dir);
            var d = b * b - c * distSq;
            if (d < 0) return false;

            var dist = Math.Sqrt(d);
            if (b <= dist)
                timeOfIntersection = (b + dist) / distSq;
            else
                timeOfIntersection = (b - dist) / distSq;

            return true;
        }

        public override string ToString()
        {
            return string.Format("Center: {0} Radius: {1}", Center, Radius);
        }

        public bool Equals(Sphere sphere)
        {
            return Center.X == sphere.Center.X && Center.Y == sphere.Center.Y && Center.Z == sphere.Center.Z && Radius == sphere.Radius;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Center, Radius);
        }
    }
}
