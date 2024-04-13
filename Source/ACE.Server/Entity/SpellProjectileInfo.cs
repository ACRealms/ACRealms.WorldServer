using System.Numerics;
using ACE.Entity;
using ACE.Server.Realms;
using ACE.Server.WorldObjects;

namespace ACE.Server.Entity
{
    public class SpellProjectileInfo
    {
        public SpellProjectile SpellProjectile;

        public InstancedPosition CasterPos;
        public InstancedPosition TargetPos;

        public Vector3? CachedVelocity;

        public Physics.Common.PhysicsPosition StartPos;

        public SpellProjectileInfo(SpellProjectile spellProjectile)
        {
            var caster = spellProjectile.ProjectileSource;
            var target = spellProjectile.ProjectileTarget;

            SpellProjectile = spellProjectile;

            if (caster?.Location != null)
                CasterPos = new InstancedPosition(caster.Location);

            if (target?.Location != null)
                TargetPos = new InstancedPosition(target.Location);

            if (spellProjectile.PhysicsObj.Position != null)
                StartPos = new Physics.Common.PhysicsPosition(spellProjectile.PhysicsObj.Position);

            CachedVelocity = target?.PhysicsObj.CachedVelocity;
        }

        public override string ToString()
        {
            var caster = SpellProjectile.ProjectileSource;
            var target = SpellProjectile.ProjectileTarget;

            var info = $"Caster: {caster.Name} ({caster.Guid})\n";
            info += $"CasterPos: {CasterPos.ToLOCString()}\n";
            info += $"Spell: {SpellProjectile.Spell.Id} - {SpellProjectile.Spell.Name}\n";
            info += $"Velocity: {SpellProjectile.Velocity}\n";
            info += $"CachedVelocity: {CachedVelocity}\n";
            info += $"StartPos: {SpellProjectile.SpawnPos.ToLOCString()}\n";
            info += $"ActualStartPos: {StartPos}\n";
            info += $"EndPos: {SpellProjectile.Location.ToLOCString()}\n";

            if (target != null)
            {
                info += $"Target: {target.WeenieClassId} - {target.Name} ({target.Guid})\n";
                info += $"TargetPos: {TargetPos.ToLOCString()}";
            }

            return info;
        }
    }
}
