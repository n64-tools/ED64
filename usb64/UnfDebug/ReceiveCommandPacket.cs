using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unf
{
    public class ReceiveCommandPacket //should be internal?
    {
        public enum CommandType : int
        {
            Text = 0x01,
            Binary = 0x02,
            FramebufferInfo = 0x03,
            FramebufferBytes = 0x04
        }

        public const string DEFAULT_PACKET_HEADER = "DMA@";
        public const string DEFAULT_PACKET_FOOTER = "CMPH";

        public string Header { get; set; } = DEFAULT_PACKET_HEADER;
        public CommandType Type { get; set; }
        public byte[] Body { get; set; }

        public string Footer { get; set; } = DEFAULT_PACKET_FOOTER;

        public byte[] Encode()
        {
            var command = new List<byte>();
            command.AddRange(Encoding.ASCII.GetBytes(Header));
            //command.AddRange(new byte[] { 0x01, 0x00, 0x00, 0x04} ); // Big Endian Example for below:
            command.AddRange(BitConverter.GetBytes((short)Type)); // High byte so no need to reverse.
            if (BitConverter.IsLittleEndian)
            {
                command.AddRange(BitConverter.GetBytes((short)Body.Length).Reverse()); // Convert to Big Endian
            }
            else
            {
                command.AddRange(BitConverter.GetBytes((short)Body.Length));
            }
            command.AddRange(Body);
            command.AddRange(Encoding.ASCII.GetBytes(Footer));
            return command.ToArray();
        }

        public void Decode(byte[] input)
        {
            if (Encoding.ASCII.GetString(input, 0, 4) != DEFAULT_PACKET_HEADER)
            {
                throw new Exception("Unexpected packet header");
            }
            //The next four bytes is the length of the body and commandtype as an integer in big endian format.
            byte[] lengthBytes = new byte[4];
            Buffer.BlockCopy(input, 4, lengthBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthBytes);
            }
            int packetInfo = BitConverter.ToInt32(lengthBytes, 0);
            Type = (CommandType)((packetInfo >> 24) & 0xFF); //the high byte
            int packetSize = packetInfo & 0xFFFFFF;


            if (Encoding.ASCII.GetString(input, DEFAULT_PACKET_HEADER.Length + sizeof(int) + packetSize, DEFAULT_PACKET_FOOTER.Length) != DEFAULT_PACKET_FOOTER)
            {
                throw new Exception("Unexpected packet footer");
            }


            Body = new byte[packetSize];
            Buffer.BlockCopy(input, DEFAULT_PACKET_HEADER.Length + sizeof(int), Body, 0, packetSize);
        }
    }
}
