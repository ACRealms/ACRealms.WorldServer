using System;
using System.Buffers;
using System.IO;

using ACE.Common.Cryptography;

namespace ACE.Server.Network
{
    public class PacketHeader
    {
        public const int HeaderSize = 20;

        public uint Sequence { get; set; }
        public PacketHeaderFlags Flags { get; set; }
        public uint Checksum { get; set; }
        public ushort Id { get; set; }
        public ushort Time { get; set; }
        public ushort Size { get; set; }
        public ushort Iteration { get; set; }

        public void Unpack(BinaryReader reader)
        {
            Sequence    = reader.ReadUInt32();
            Flags       = (PacketHeaderFlags)reader.ReadUInt32();
            Checksum    = reader.ReadUInt32();
            Id          = reader.ReadUInt16();
            Time        = reader.ReadUInt16();
            Size        = reader.ReadUInt16();
            Iteration   = reader.ReadUInt16();
        }

        public void Unpack(byte[] buffer, int offset = 0)
        {
            Sequence    =              (uint)(buffer[offset++] | (buffer[offset++] << 8) | (buffer[offset++] << 16) | (buffer[offset++] << 24));
            Flags       = (PacketHeaderFlags)(buffer[offset++] | (buffer[offset++] << 8) | (buffer[offset++] << 16) | (buffer[offset++] << 24));
            Checksum    =              (uint)(buffer[offset++] | (buffer[offset++] << 8) | (buffer[offset++] << 16) | (buffer[offset++] << 24));

            Id          = (ushort)(buffer[offset++] | (buffer[offset++] << 8));
            Time        = (ushort)(buffer[offset++] | (buffer[offset++] << 8));
            Size        = (ushort)(buffer[offset++] | (buffer[offset++] << 8));
            Iteration   = (ushort)(buffer[offset++] | (buffer[offset++] << 8));
        }

        public void Pack(byte[] buffer, int offset = 0)
        {
            Span<byte> stackBuffer = stackalloc byte[20];
            byte localOffset = 0;
            var sequence = Sequence;
            int iFlags = (int)Flags;
            var checksum = Checksum;
            var id = Id;
            var time = Time;
            var size = Size;
            var iteration = Iteration;

            stackBuffer[localOffset++] = (byte)sequence;
            stackBuffer[localOffset++] = (byte)(sequence >> 8);
            stackBuffer[localOffset++] = (byte)(sequence >> 16);
            stackBuffer[localOffset++] = (byte)(sequence >> 24);

            
            stackBuffer[localOffset++] = (byte)iFlags;
            stackBuffer[localOffset++] = (byte)(iFlags >> 8);
            stackBuffer[localOffset++] = (byte)(iFlags >> 16);
            stackBuffer[localOffset++] = (byte)(iFlags >> 24);

            stackBuffer[localOffset++] = (byte)checksum;
            stackBuffer[localOffset++] = (byte)(checksum >> 8);
            stackBuffer[localOffset++] = (byte)(checksum >> 16);
            stackBuffer[localOffset++] = (byte)(checksum >> 24);

            stackBuffer[localOffset++] = (byte)id;
            stackBuffer[localOffset++] = (byte)(id >> 8);

            stackBuffer[localOffset++] = (byte)time;
            stackBuffer[localOffset++] = (byte)(time >> 8);

            stackBuffer[localOffset++] = (byte)size;
            stackBuffer[localOffset++] = (byte)(size >> 8);

            stackBuffer[localOffset++] = (byte)iteration;
            stackBuffer[localOffset++] = (byte)(iteration >> 8);

            stackBuffer.CopyTo(new Span<byte>(buffer, offset, 20));
        }

        public uint CalculateHash32()
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(HeaderSize);

            try
            {
                uint original = Checksum;
                Checksum = 0xBADD70DD;

                Pack(buffer);

                var checksum = Hash32.Calculate(buffer, HeaderSize);
                Checksum = original;

                return checksum;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public bool HasFlag(PacketHeaderFlags flags) { return (flags & Flags) != 0; }

        public override string ToString()
        {
            var c = HasFlag(PacketHeaderFlags.EncryptedChecksum) ? "X" : "";

            return $"Seq: {Sequence} Id: {Id} Iter: {Iteration} {c}CRC: {Checksum} {PacketHeaderFlagsUtil.UnfoldFlags(Flags)}";
        }
    }
}
