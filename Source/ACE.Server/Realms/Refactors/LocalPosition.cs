using ACE.Database.Adapter;
using ACE.Database.Models.Auth;
using ACE.Database.Models.World;
using ACE.Entity;
using ACE.Entity.Enum.Properties;
using ACE.Entity.Enum.RealmProperties;
using ACE.Server.Managers;
using ACE.Server.Network.GameAction.Actions;
using ACE.Server.WorldObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Realms
{
    public sealed class LocalPosition : UsablePosition
    {
        public LocalPosition() : this(new Position()) { }

        public LocalPosition(Position pos)
            : base(pos) { }

        public LocalPosition(LocalPosition pos)
            : this(pos.Position) { }

        //public InstancedPosition ToInstancedPosition();

        public LocalPosition(uint blockCellID, float newPositionX, float newPositionY,
                        float newPositionZ, float newRotationX, float newRotationY,
                        float newRotationZ, float newRotationW, bool relativePos = false)
        : this(new Position(blockCellID, newPositionX, newPositionY, newPositionZ, newRotationX, newRotationY, newRotationZ, newRotationW, 0, relativePos)) { }

        public LocalPosition(uint blockCellID, Vector3 position, Quaternion rotation)
            : this(new Position(blockCellID, position, rotation)) { }

        public LocalPosition(BinaryReader reader)
            : this(new Position(reader)) { }

        public bool Equals(LocalPosition p) => Position.Equals(p.Position);

        public InstancedPosition AsInstancedPosition(uint instanceId) => new InstancedPosition(Position, instanceId);
        public InstancedPosition AsInstancedPosition(Player player, PlayerInstanceSelectMode mode, PlayerInstanceSelectMode backupMode = PlayerInstanceSelectMode.HomeRealm)
        {
            if (mode == backupMode && mode != PlayerInstanceSelectMode.HomeRealm)
                return AsInstancedPosition(player, mode, PlayerInstanceSelectMode.HomeRealm);

            uint instanceId;
            switch (mode)
            {
                case PlayerInstanceSelectMode.Undefined:
                    throw new ArgumentException("mode is undefined");
                case PlayerInstanceSelectMode.Same:
                    instanceId = player.Location.Instance; break;
                case PlayerInstanceSelectMode.SameIfSameLandblock:
                    if (player.Location.LandblockShort == LandblockShort)
                        instanceId = player.Location.Instance;
                    else
                        return AsInstancedPosition(player, backupMode);
                    break;
                case PlayerInstanceSelectMode.RealmDefaultInstanceID:
                    // Warning! May have unintended results if using from an ephemeral realm
                    instanceId = player.RealmRuleset.GetDefaultInstanceID(player, this); break;
                case PlayerInstanceSelectMode.HomeRealm:
                    var realm = RealmManager.GetRealm(player.HomeRealm, includeRulesets: false);
                    instanceId = realm.StandardRules.GetDefaultInstanceID(player, this);
                    break;
                case PlayerInstanceSelectMode.PersonalRealm:
                    var hideoutRealm = RealmManager.GetReservedRealm(ReservedRealm.hideout);
                    instanceId = hideoutRealm.StandardRules.GetDefaultInstanceID(player, this);
                    break;
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
                    instanceId = obj.RealmRuleset.GetDefaultInstanceID(obj.Location?.AsLocalPosition()); break;
                default: throw new NotImplementedException();
            }
            
            return new InstancedPosition(Position, instanceId);
        }

        public LocalPosition Translate(uint blockCell)
        {
            var newBlockX = blockCell >> 24;
            var newBlockY = (blockCell >> 16) & 0xFF;

            var xDiff = (int)newBlockX - LandblockX;
            var yDiff = (int)newBlockY - LandblockY;

            var pos = new Position(Position);
            //pos.Origin.X -= xDiff * 192;
            pos.PositionX -= xDiff * 192;
            //pos.Origin.Y -= yDiff * 192;
            pos.PositionY -= yDiff * 192;

            //pos.ObjCellID = blockCell;
            pos.LandblockId = new LandblockId(blockCell);
            return new LocalPosition(pos);
        }
    }
}
