using log4net;
using System.IO;

namespace ACE.Server.Network.GameMessages
{
    public class RealmsBinaryWriter : System.IO.BinaryWriter
    {
        public RealmsBinaryWriter(MemoryStream stream)
            : base(stream) { }

        public override void Write(ulong value)
        {
            base.Write(value);
        }
    }

    public abstract class GameMessage
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public GameMessageOpcode Opcode { get; private set; }

        public System.IO.MemoryStream Data { get; private set; }

        public GameMessageGroup Group { get; private set; }

        protected BinaryWriter Writer { get; private set; }

        protected GameMessage(GameMessageOpcode opCode, GameMessageGroup group)
        {
            Opcode = opCode;

            Group = group;

            Data = new System.IO.MemoryStream();

            Writer = new RealmsBinaryWriter(Data);

            if (Opcode != GameMessageOpcode.None)
                Writer.Write((uint)Opcode);
        }
    }
}
