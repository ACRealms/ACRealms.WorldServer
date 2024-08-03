using System;
using System.Collections.Generic;
using System.Numerics;
using ACE.Server.Physics.Animation;

namespace ACE.Server.Physics.BSP
{
    public class BSPLeaf: BSPNode, IEquatable<BSPLeaf>
    {
        public int LeafIdx;
        public bool Solid;

        public BSPLeaf(): base()
        {
            LeafIdx = -1;
        }

        public BSPLeaf(BSPTreeType type): base(type)
        {
        }

        public BSPLeaf(DatLoader.Entity.BSPLeaf node, Dictionary<ushort, DatLoader.Entity.Polygon> polys, DatLoader.Entity.CVertexArray vertexArray)
            : base(node, polys, vertexArray)
        {
            LeafIdx = node.LeafIndex;
            Solid = node.Solid == 1;
        }

        public override void find_walkable(SpherePath path, ref Sphere validPos, ref Polygon hitPoly, Vector3 movement, Vector3 up, ref bool changed)
        {
            if (NumPolys == 0 || !Sphere.Intersects(in validPos))
                return;

            foreach (var polygon in Polygons)
            {
                var walkable = polygon.walkable_hits_sphere(path, in validPos, up);
                var adjusted = false;
                if (walkable)
                    adjusted = polygon.adjust_sphere_to_plane(path, ref validPos, movement);

                if (walkable && adjusted)
                {
                    changed = true;
                    hitPoly = polygon;
                }
            }
        }

        public override bool hits_walkable(SpherePath path, ref readonly Sphere validPos, Vector3 up)
        {
            if (NumPolys == 0 || !Sphere.Intersects(in validPos))
                return false;

            foreach (var polygon in Polygons)
            {
                if (polygon.walkable_hits_sphere(path, in validPos, up) && polygon.check_small_walkable(in validPos, up))
                    return true;
            }
            return false;
        }

        public override bool point_intersects_solid(Vector3 point)
        {
            return NumPolys != 0;
        }

        public override bool sphere_intersects_poly(ref readonly Sphere checkPos, Vector3 movement, ref Polygon hitPoly, ref Vector3 contactPoint)
        {
            if (NumPolys == 0 || !Sphere.Intersects(in checkPos))
                return false;

            foreach (var polygon in Polygons)
            {
                if (polygon.pos_hits_sphere(in checkPos, movement, ref contactPoint, ref hitPoly))
                    return true;
            }
            return false;
        }

        public override bool sphere_intersects_solid(ref readonly Sphere checkPos, bool checkCenter)
        {
            if (NumPolys == 0) return false;
            if (checkCenter && Solid) return true;
            if (!Sphere.Intersects(in checkPos)) return false;

            foreach (var polygon in Polygons)
                if (polygon.hits_sphere(in checkPos))
                    return true;

            return false;
        }

        public override bool sphere_intersects_solid_poly(ref readonly Sphere checkPos, float radius, ref bool centerSolid, ref Polygon hitPoly, bool checkCenter)
        {
            if (NumPolys == 0) return false;

            if (checkCenter && Solid)
                centerSolid = true;

            if (!Sphere.Intersects(in checkPos))
                return centerSolid;

            foreach (var polygon in Polygons)
            {
                if (polygon.hits_sphere(in checkPos))
                {
                    hitPoly = polygon;
                    return true;
                }
            }
            return centerSolid;
        }

        public bool Equals(BSPLeaf leaf)
        {
            return base.Equals(leaf) && LeafIdx == leaf.LeafIdx && Solid == leaf.Solid;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();

            hash = (hash * 397) ^ LeafIdx.GetHashCode();
            hash = (hash * 397) ^ Solid.GetHashCode();

            return hash;
        }
    }
}
