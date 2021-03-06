using System.Collections.Generic;

using ACE.Common;
using ACE.Server.WorldObjects;

namespace ACE.Server.Factories.Entity
{
    public class MutationFilter
    {
        public List<Mutation> Mutations = new List<Mutation>();

        // MutationFilter -> Mutation -> MutationOutcome -> EffectList -> Effect
        
        public bool TryMutate(WorldObject wo, int tier)
        {
            var rng = ThreadSafeRandom.Next(0.0f, 1.0f);

            var success = true;

            foreach (var mutation in Mutations)
                success &= mutation.TryMutate(wo, tier, rng);

            return success;
        }
    }
}
