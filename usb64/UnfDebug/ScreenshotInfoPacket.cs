using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unf
{
    public class ScreenshotInfoPacket //should be internal?
    {
        public int Width { get; set; } = 0;
        public int ImageType { get; set; } = 0;
        public int Height { get; set; } = 0;
        public int CommandType { get; set; } = 0;

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
            ImageType = imageInfo[1];
            Width = imageInfo[2];
            Height = imageInfo[3];

        }

        public byte[] Encode()
        {
            var body = new List<byte>();
            if (BitConverter.IsLittleEndian)
            {
                body.AddRange(BitConverter.GetBytes(CommandType).Reverse());
                body.AddRange(BitConverter.GetBytes(ImageType).Reverse());
                body.AddRange(BitConverter.GetBytes(Width).Reverse());
                body.AddRange(BitConverter.GetBytes(Height).Reverse());
            }
            else
            {
                body.AddRange(BitConverter.GetBytes(CommandType));
                body.AddRange(BitConverter.GetBytes(ImageType));
                body.AddRange(BitConverter.GetBytes(Width));
                body.AddRange(BitConverter.GetBytes(Height));
            }
            return body.ToArray();
        }

    }
}
