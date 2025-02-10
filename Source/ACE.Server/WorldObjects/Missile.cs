using System;

using ACE.Entity;
using ACE.Entity.Models;
using ACE.Server.Realms;

namespace ACE.Server.WorldObjects
{
    public class Missile : Stackable
    {
        /// <summary>
        /// A new biota be created taking all of its values from weenie.
        /// </summary>
        public Missile(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
        {
            SetEphemeralValues();
        }

        /// <summary>
        /// Restore a WorldObject from the database.
        /// </summary>
        public Missile(Biota biota) : base(biota)
        {
            SetEphemeralValues();
        }

        private void SetEphemeralValues()
        {
        }
    }
}
