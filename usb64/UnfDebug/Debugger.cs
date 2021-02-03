using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace Unf
{
    public class Debugger
    {
        
        //TODO: On the ED64, we should take into account that we are overriding the end of the ROM space here.
        const int MAX_FILE_SIZE = 0x800000; //8MB is the max file size that is allowed (on the 64Drive)

        public const string RECEIVE_PACKET_HEADER = "DMA@";
        public const string RECEIVE_PACKET_FOOTER = "CMPH";

        readonly IFileSystem fileSystem;

        // <summary>Create MyComponent with the given fileSystem implementation</summary>
        public Debugger(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }
        /// <summary>Create MyComponent</summary>
        public Debugger() : this(
            fileSystem: new FileSystem() //use default implementation which calls System.IO
        )
        {
        }


        public byte[] ProcessSendCommand(string command, int alignment = 1)
        {
            if (command.Contains("@"))
            {
                var filePath = command.Split('@')[1];
                var fileSize = fileSystem.FileInfo.FromFileName(filePath).Length;

                if (fileSize > MAX_FILE_SIZE)
                {
                    throw new Exception("Exceeds maximum file size");
                }

                int start = command.IndexOf("@");
                int end = command.IndexOf("@", start + 1);

                string textBefore = "";
                if (start > 0)
                {
                    textBefore = command.Substring(0, start);
                }

                string textAfter = "";
                if (end < command.Length)
                {
                    textAfter = command.Substring(end + 1);
                }

                // Ensure the file buffer size is aligned if required.
                if (alignment != 0)
                {
                    if (fileSize % alignment != 0)
                    {
                        var size = fileSize;
                        fileSize = size + alignment - size % alignment;
                    }
                }

                var filecontent = new byte[fileSize];
                if (!string.IsNullOrEmpty(filePath))
                {
                    //the file content might be binary, so lets just read it as such!
                    filecontent = fileSystem.File.ReadAllBytes(filePath);
                }

                var ret = new List<byte>();
                ret.AddRange(Encoding.ASCII.GetBytes(textBefore));
                ret.Add((byte)'@');
                ret.AddRange(Encoding.ASCII.GetBytes(fileSize.ToString()));
                ret.Add( (byte)'@' );
                ret.AddRange(filecontent);
                ret.AddRange(Encoding.ASCII.GetBytes(textAfter));
                return ret.ToArray();
            }
            else
            {
                return Encoding.ASCII.GetBytes(command);
            }
        }

        public enum ReceiveCommandType : int
        {
            TEXT = 0x01,
            BINARY = 0x02,
            SCREENSHOT_HEADER = 0x03,
            SCREENSHOT_BODY = 0x04
        }

        private static ScreenshotInfo ImageInfo = new ScreenshotInfo();
        public string ProcessReceiveCommand(byte[] input)
        {
            if (Encoding.ASCII.GetString(input, 0, 4) != RECEIVE_PACKET_HEADER)
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
            ReceiveCommandType packetCommand = (ReceiveCommandType)((packetInfo >> 24) & 0xFF); //the high byte
            int packetSize = packetInfo & 0xFFFFFF;


            if (Encoding.ASCII.GetString(input, RECEIVE_PACKET_HEADER.Length + sizeof(int) + packetSize, RECEIVE_PACKET_FOOTER.Length) != RECEIVE_PACKET_FOOTER)
            {
                throw new Exception("Unexpected packet footer");
            }


            byte[] packetBody = new byte[packetSize];
            Buffer.BlockCopy(input, RECEIVE_PACKET_HEADER.Length + sizeof(int), packetBody, 0, packetSize);

            switch (packetCommand)
            {
                case ReceiveCommandType.TEXT:
                    return Encoding.ASCII.GetString(packetBody);
                case ReceiveCommandType.BINARY:
                    var filename = fileSystem.Path.GetTempFileName();
                    fileSystem.File.WriteAllBytes(filename, packetBody);
                    return filename;
                case ReceiveCommandType.SCREENSHOT_HEADER:
                    ImageInfo = new ScreenshotInfo();
                    ImageInfo.Decode(packetBody);
                    return $"w={ImageInfo.Width} h={ImageInfo.Height}";
                //break;
                case ReceiveCommandType.SCREENSHOT_BODY:
                    //TODO: handle.
                    // Ensure we got a data header of type screenshot
                    // if (ImageInfo.CommandType == (int)ReceiveCommandType.SCREENSHOT_BODY)
                    //{
                    //  int pngType = ImageInfo.Type // = 2 in most cases?
                    //  int width = ImageInfo.Width, height = ImageInfo.Height;
                    //  if (pngType == 2)
                    //  { Convert to PNG. }
                    //  else
                    //  { }
                    //}
                    break;
                default:
                    throw new Exception("Unknown packet type.");
            }
            return "";
        }
    }


    public class ScreenshotInfo
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

    public class CommandPacket
    {
        public string Header { get; set; }
        public int Type { get; set; }
        public byte[] Body { get; set; }

        public string Footer { get; set; }

        public byte[] ToBytes()
        {

            return Body;
        }
    }

}
