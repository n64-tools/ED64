using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;
//using System.IO.Ports;


namespace Unf
{
    public class Debugger
    {
        
        //TODO: On the ED64, we should take into account that we are overriding the end of the ROM space here.
        const int MAX_FILE_SIZE = 0x800000; //8MB is the max file size that is allowed (on the 64Drive)

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
                    throw new Exception($"File size exceeds maximum allowed: {MAX_FILE_SIZE}");
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

        public string ProcessReceiveCommand(byte[] input)
        {
            var packet = new ReceiveCommandPacket();
            packet.Decode(input);
            return HandleReceivedCommand(packet.Type, packet.Body);

        }


        private static FramebufferInfoPacket ImageInfo = new FramebufferInfoPacket();
        private string HandleReceivedCommand(ReceiveCommandPacket.CommandType packetCommand, byte[] packetBody)
        {
            switch (packetCommand)
            {
                case ReceiveCommandPacket.CommandType.Text:
                    return Encoding.ASCII.GetString(packetBody);
                case ReceiveCommandPacket.CommandType.Binary:
                    var filename = fileSystem.Path.GetTempFileName();
                    fileSystem.File.WriteAllBytes(filename, packetBody);
                    return filename;
                case ReceiveCommandPacket.CommandType.FramebufferInfo:
                    ImageInfo = new FramebufferInfoPacket();
                    ImageInfo.Decode(packetBody);
                    return $"w={ImageInfo.Horizontal} h={ImageInfo.Vertical}";
                case ReceiveCommandPacket.CommandType.FramebufferBytes:
                    if (ImageInfo.CommandType == (int)ReceiveCommandPacket.CommandType.FramebufferBytes)
                    { // We have already received the required info packet, so we are good to go!
                        ImageUtilities.ConvertToBitmap((short)ImageInfo.Horizontal, (short)ImageInfo.Vertical, packetBody);
                    }
                    break;
                default:
                    throw new Exception("Unknown packet type.");
            }
            return "";
        }
    }
}
