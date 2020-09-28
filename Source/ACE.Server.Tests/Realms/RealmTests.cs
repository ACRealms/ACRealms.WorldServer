
using System.Collections.Generic;
using System.Numerics;
using ACE.Entity;
using Xunit;

namespace ACE.Server.Tests.Realm
{
    public class RealmTests
    {
        [Fact]
        public void Position_InstanceIDConversion()
        {
            Position.ParseInstanceID(0x80020001, out var istemp, out var realmid, out var shortinstanceid);
            Assert.True(istemp);
            Assert.Equal(1, shortinstanceid);
            Assert.Equal(2, realmid);

            Assert.Equal(0x80020001, Position.InstanceIDFromVars(realmid, shortinstanceid, istemp));
        }
    }
}
