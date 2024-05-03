using ACE.Entity;
using System.IO;

namespace ACE.Server.Network
{
    public class ClientMessage
    {
        public class RealmBinaryReader : BinaryReader
        {
            public RealmBinaryReader(MemoryStream stream)
                : base(stream) { }

            public ObjectGuid ReadGuid(ISession session)
            {
                return new ObjectGuid(base.ReadUInt32(), session.Player.Location.Instance);
            }
        }

        public MemoryStream Data { get; }

        public RealmBinaryReader Payload { get; }

        public uint Opcode { get; }

        /// <exception cref="EndOfStreamException">stream must be at least 4 bytes in length remaining to read</exception>
        public ClientMessage(MemoryStream stream)
        {
            Data = stream;
            Payload = new RealmBinaryReader(Data);
            Opcode = Payload.ReadUInt32();
        }

        /// <exception cref="EndOfStreamException">data must be at least 4 bytes in length</exception>
        public ClientMessage(byte[] data)
        {
            Data = new MemoryStream(data);
            Payload = new RealmBinaryReader(Data);
            Opcode = Payload.ReadUInt32();
        }
    }
}
