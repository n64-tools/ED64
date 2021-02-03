using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;

namespace Unf
{
    public class Debugger
    {
        
        //TODO: On the ED64, we should take into account that we are overriding the end of the ROM space here.
        const int MAX_FILE_SIZE = 0x800000; //8MB is the max file size that is allowed (on the 64Drive)

        const string RECEIVE_PACKET_HEADER = "DMA@";
        const string RECEIVE_PACKET_FOOTER = "CMPH";

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

        enum ReceiveCommandType : int
        {
            TEXT = 0x01,
            BINARY = 0x02,
            SCREENSHOT_HEADER = 0x03,
            SCREENSHOT_BODY = 0x04
        }

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
                    const int HEADER_SIZE = 4 * sizeof(int); //consists of 4 int32
                    if (packetBody.Length == HEADER_SIZE)
                    {
                        var headerdata = new int[HEADER_SIZE / 4]; //TODO: this will be needed for the screenshot body so need to store it globally?
                        for (int i = 0; i < packetSize; i += sizeof(int))
                        {
                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(packetBody, i, sizeof(int));
                            }
                            headerdata[i / 4] = BitConverter.ToInt32(packetBody, i);
                        }
                    }
                    else
                    {
                        throw new Exception("Packet decode failed.");
                    }
                    return "ss"; //TODO: something better needed!
                    //break;
                case ReceiveCommandType.SCREENSHOT_BODY:
                    //TODO: handle.
                    // Ensure we got a data header of type screenshot
                    // if (headerdata[0] == (int)ReceiveCommandType.SCREENSHOT_BODY)
                    //{
                    //  int pngType = headerdata[1] // = 2 in most cases?
                    //  int width = headerdata[2], height = headerdata[3];
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

}
