global using BinaryWriter = ACE.Server.Network.GameMessages.RealmsBinaryWriter;
using ACE.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Network.GameMessages
{
    public class RealmsBinaryWriter : System.IO.BinaryWriter
    {
        public RealmsBinaryWriter(MemoryStream stream)
            : base(stream) { }


#pragma warning disable CS0809
        // This won't actually get reported in the build process, hence the need for a Roslyn extension. It will get shown on hover, however.
        [Obsolete("Use WriteNonGuidULong instead. If writing a guid, use WriteGuid", true)] 
        public override void Write(ulong value) 
        {
            base.Write(value);
        }
#pragma warning restore CS0809

        public void WriteNonGuidULong(ulong value) => base.Write(value);

        public void WriteGuid(ObjectGuid guid) { Write(guid.ClientGUID); }
    }
}
