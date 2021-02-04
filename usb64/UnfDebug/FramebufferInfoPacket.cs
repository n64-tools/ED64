using System;
using System.Collections.Generic;
using System.Linq;

namespace Unf
{
    public class FramebufferInfoPacket //should be internal?
    {
        public int CommandType { get; set; } = 0;
        public int Horizontal { get; set; } = 0;
        public int Vertical { get; set; } = 0;

        public const int IMAGE_INFO_SIZE = 4 * sizeof(int);
        public void Decode(byte[] packetBody)
        {

            var imageInfo = new int[IMAGE_INFO_SIZE / 4];
            if (packetBody.Length == IMAGE_INFO_SIZE)
            {
                for (int i = 0; i < packetBody.Length; i += sizeof(int))
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(packetBody, i, sizeof(int));
                    }
                    imageInfo[i / 4] = BitConverter.ToInt32(packetBody, i);
                }
            }
            else
            {
                throw new Exception("Packet decode failed.");
            }
            CommandType = imageInfo[0];
            if (imageInfo[1] != 2) // This describes the "body" parameters (in int32's) of the packet...
            {
                throw new Exception("Unexpected packet length");
            }
            Horizontal = imageInfo[2];
            Vertical = imageInfo[3];

        }

        public byte[] Encode()
        {
            var body = new List<byte>();
            if (BitConverter.IsLittleEndian)
            {
                body.AddRange(BitConverter.GetBytes(CommandType).Reverse());
                body.AddRange(BitConverter.GetBytes(2).Reverse()); //Size of packet (below parameters)
                body.AddRange(BitConverter.GetBytes(Horizontal).Reverse());
                body.AddRange(BitConverter.GetBytes(Vertical).Reverse());
            }
            else
            {
                body.AddRange(BitConverter.GetBytes(CommandType));
                body.AddRange(BitConverter.GetBytes(2)); //Size of packet (below parameters)
                body.AddRange(BitConverter.GetBytes(Horizontal));
                body.AddRange(BitConverter.GetBytes(Vertical));
            }
            return body.ToArray();
        }

    }
}
