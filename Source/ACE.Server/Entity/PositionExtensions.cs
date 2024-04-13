using System;
using System.Numerics;

using log4net;

using ACE.DatLoader;
using ACE.Entity;
using ACE.Entity.Enum.Properties;
using ACE.Server.Physics.Common;
using ACE.Server.Physics.Extensions;
using ACE.Server.Physics.Util;
using ACE.Server.WorldObjects;

using Position = ACE.Entity.Position;
using ACE.Server.Managers;
using ACE.Server.Realms;

namespace ACE.Server.Entity
{
    public static class PositionExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        /// <summary>
        /// Returns TRUE if outdoor position is located on walkable slope
        /// </summary>
        public static bool IsWalkable(this InstancedPosition p)
        {
            if (p.Indoors) return true;

            var landcell = (LandCell)LScape.get_landcell(p.Cell, p.Instance);

            Physics.Polygon walkable = null;
            var terrainPoly = landcell.find_terrain_poly(p.Pos, ref walkable);
            if (walkable == null) return false;

            return Physics.PhysicsObj.is_valid_walkable(walkable.Plane.Normal);
        }

        public static InstancedPosition ACEPosition(this Physics.Common.PhysicsPosition pos, uint instance)
        {
            return new InstancedPosition(pos.ObjCellID, pos.Frame.Origin, pos.Frame.Orientation, instance);
        }
    }
}
