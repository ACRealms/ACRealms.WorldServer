using ACE.Database.Adapter;
using ACE.Database.Models.World;
using ACE.Entity;
using ACE.Server.Managers;
using ACE.Server.Network.GameAction.Actions;
using ACE.Server.WorldObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Realms
{
    public sealed class LocalPosition : UsablePosition
    {
        public LocalPosition() : this(new Position()) { }

        public LocalPosition(Position pos)
            : base(pos)
        { }

        //public InstancedPosition ToInstancedPosition();

        public LocalPosition(uint blockCellID, float newPositionX, float newPositionY,
                        float newPositionZ, float newRotationX, float newRotationY,
                        float newRotationZ, float newRotationW, bool relativePos = false)
        : this(new Position(blockCellID, newPositionX, newPositionY, newPositionZ, newRotationX, newRotationY, newRotationZ, newRotationW, 0, relativePos)) { }

        public bool Equals(LocalPosition p) => Position.Equals(p.Position);

        public InstancedPosition AsInstancedPosition(Player player, PlayerInstanceSelectMode mode)
        {
            uint instanceId;
            switch (mode)
            {
                case PlayerInstanceSelectMode.Undefined:
                    throw new ArgumentException("mode is undefined");
                case PlayerInstanceSelectMode.Same:
                    instanceId = player.Location.Instance; break;
                case PlayerInstanceSelectMode.RealmDefaultInstanceID:
                    instanceId = player.RealmRuleset.GetDefaultInstanceID(); break;
                case PlayerInstanceSelectMode.HomeRealm:
                    instanceId = Position.InstanceIDFromVars(player.HomeRealm, 0, false); break;
                case PlayerInstanceSelectMode.PerRuleset:
                    throw new NotImplementedException();
                default: throw new NotImplementedException();
            }

            return new InstancedPosition(Position, instanceId);
        }

        public InstancedPosition AsInstancedPosition(WorldObject obj, WorldObjectInstanceSelectMode mode)
        {
            uint instanceId;
            switch (mode)
            {
                case WorldObjectInstanceSelectMode.Undefined:
                    throw new ArgumentException("mode is undefined");
                case WorldObjectInstanceSelectMode.Same:
                    instanceId = obj.Location.Instance; break;
                case WorldObjectInstanceSelectMode.RealmDefaultInstanceID:
                    instanceId = obj.RealmRuleset.GetDefaultInstanceID(); break;
                case WorldObjectInstanceSelectMode.PerRuleset:
                    throw new NotImplementedException();
                default: throw new NotImplementedException();
            }
            
            return new InstancedPosition(Position, instanceId);
        }
    }
}
