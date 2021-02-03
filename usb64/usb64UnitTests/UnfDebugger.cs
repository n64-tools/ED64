using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.IO;

namespace usb64UnitTests
{
    [TestClass]
    public class UnfDebugger
    {
        [TestMethod]
        public void Send_command_string()
        {
            string command = "commandname arg1 arg2";
            var output = new Unf.Debugger().ProcessSendCommand(command);
            Assert.AreEqual(command, Encoding.ASCII.GetString(output));
        }

        [TestMethod]
        public void Send_command_text_file()
        {
            string filename = @"c:\temp.txt";
            string command = $"@{filename}@";
            string fileText = "abcd";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{filename}", new MockFileData(fileText) },
            });
            string commandResult = $"@{fileText.Length}@{fileText}";
            var output = new Unf.Debugger(fileSystem).ProcessSendCommand(command);
            Assert.AreEqual(commandResult, Encoding.ASCII.GetString(output));
        }

        //[TestMethod]
        //public void Send_command_binary_file()
        //{
        //    string filename = @"c:\temp.txt";
        //    string command = $"@{filename}@";

        //    // Put random bytes into this array.
        //    byte[] fileContent = new byte[0x800000]; //Max is 0x800000 (8MB)
        //    // Use Random class and NextBytes method.
        //    // ... Display the bytes with following method.
        //    Random random = new Random();
        //    random.NextBytes(fileContent);

        //    var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        //    {
        //        { $"{filename}", new MockFileData(fileContent) },
        //    });
        //    string commandResult = $"@{fileContent.Length}@{fileContent}";
        //    var output = new Unf.Debugger(fileSystem).ProcessSendCommand(command);
        //    Assert.IsTrue(commandResult, output);
        //}

        [TestMethod]
        public void Send_command_text_file_with_4_byte_alignment()
        {
            string filename = @"c:\temp.txt";
            string command = $"@{filename}@";
            string fileText = "abcde"; //5 byte file
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{filename}", new MockFileData(fileText) },
            });
            string commandResult = "@8@abcde"; //align to 4 for byte alignment on the 64Drive
            var output = new Unf.Debugger(fileSystem).ProcessSendCommand(command, 4);
            Assert.AreEqual(commandResult, Encoding.ASCII.GetString(output));
        }

        [TestMethod]
        public void Send_command_text_file_with_string_after()
        {
            string filename = @"c:\temp.txt";
            string command = $"commandname arg1 arg2 @{filename}@";
            string fileText = "abcd";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{filename}", new MockFileData(fileText) },
            });
            string commandResult = $"commandname arg1 arg2 @{fileText.Length}@{fileText}";
            var output = new Unf.Debugger(fileSystem).ProcessSendCommand(command);
            Assert.AreEqual(commandResult, Encoding.ASCII.GetString(output));
        }

        [TestMethod]
        public void Send_command_text_file_with_string_before()
        {
            string filename = @"c:\temp.txt";
            string command = $"@{filename}@ arg3";
            string fileText = "abcd";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{filename}", new MockFileData(fileText) },
            });
            string commandResult = "@4@abcd arg3";
            var output = new Unf.Debugger(fileSystem).ProcessSendCommand(command);
            Assert.AreEqual(commandResult, Encoding.ASCII.GetString(output));
        }

        [TestMethod]
        public void Send_command_text_file_with_string_before_and_after()
        {
            string filename = @"c:\temp.txt";
            string command = $"commandname arg1 arg2 @{filename}@ arg3";
            string fileText = "abcd";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{filename}", new MockFileData(fileText) },
            });
            string commandResult = $"commandname arg1 arg2 @{fileText.Length}@{fileText} arg3";
            var output = new Unf.Debugger(fileSystem).ProcessSendCommand(command);
            Assert.AreEqual(commandResult, Encoding.ASCII.GetString(output));
        }

        [TestMethod]
        public void Receive_command_text_packet_body()
        {
            var packetBody = "abcd";
            var command = new Unf.ReceiveCommandPacket();
            command.Type = Unf.ReceiveCommandPacket.CommandType.TEXT;
            command.Body = Encoding.ASCII.GetBytes(packetBody);
            var output = new Unf.Debugger().ProcessReceiveCommand(command.Encode());
            Assert.AreEqual(packetBody, output);
        }

        [TestMethod]
        public void Receive_command_binary_packet_body()
        {
            // Put random bytes into this array.
            byte[] packetBody = new byte[0x20];
            Random random = new Random(); // Use Random class and NextBytes method.
            random.NextBytes(packetBody);
            var command = new Unf.ReceiveCommandPacket
            {
                Type = Unf.ReceiveCommandPacket.CommandType.BINARY,
                Body = packetBody
            };
            var output = new Unf.Debugger().ProcessReceiveCommand(command.Encode());

            //read back the file created and compare it...
            var outputcontent = File.ReadAllBytes(output);

            Assert.IsTrue(packetBody.SequenceEqual(outputcontent));
        }

        [TestMethod]
        public void Receive_command_binary_packet_body_with_padding()
        {
            var packetBody = new byte[] { 0x00, 0x01, 0x02, 0x03 };
            var command = new Unf.ReceiveCommandPacket
            {
                Type = Unf.ReceiveCommandPacket.CommandType.BINARY,
                Body = packetBody
            };
            var commandWithPadding = new List<byte>();
            commandWithPadding.AddRange(command.Encode());
            commandWithPadding.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 }); //added padding.)

            var output = new Unf.Debugger().ProcessReceiveCommand(commandWithPadding.ToArray());

            //read back the file created and compare it...
            var outputcontent = File.ReadAllBytes(output);

            Assert.IsTrue(packetBody.SequenceEqual(outputcontent));
        }

        [TestMethod]
        public void Receive_command_screenshot_info_packet_body()
        {
            var packetBody = new Unf.ScreenshotInfo() 
            { 
                CommandType = (int)Unf.ReceiveCommandPacket.CommandType.SCREENSHOT_BODY,
                ImageType = 2, //TODO: what does this actually mean?
                Width = 320,
                Height = 240
            };


            var command = new Unf.ReceiveCommandPacket
            {
                Type = Unf.ReceiveCommandPacket.CommandType.SCREENSHOT_HEADER,
                Body = packetBody.Encode()
            };
            var output = new Unf.Debugger().ProcessReceiveCommand(command.Encode());
            Assert.IsTrue(output.Length > 4);
        }

        //[TestMethod]
        //public void Receive_command_screenshot_packet_body()
        //{
        //    var packetBody = new byte[] { 0x00, 0x01, 0x02, 0x03 };//need to collect a sample from somewhere!
        //    var command = new List<byte>();
        //    command.AddRange(Encoding.ASCII.GetBytes(Unf.Debugger.RECEIVE_PACKET_HEADER));
        //    //command.AddRange(new byte[] { 0x01, 0x00, 0x00, 0x04} ); //Big Endian Example for below:
        //    command.AddRange(BitConverter.GetBytes((short)Unf.Debugger.ReceiveCommandType.SCREENSHOT_BODY)); //text, high byte so no need to reverse.
        //    command.AddRange(BitConverter.GetBytes((short)packetBody.Length).Reverse()); //Big Endian
        //    command.AddRange(packetBody);
        //    command.AddRange(Encoding.ASCII.GetBytes(Unf.Debugger.RECEIVE_PACKET_FOOTER));
        //    command.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 }); //added padding. Should be a different test!
        //    var output = new Unf.Debugger().ProcessReceiveCommand(command.ToArray());

        //    //read back the file created and compare it...
        //    var outputcontent = File.ReadAllBytes(output);

        //    Assert.IsTrue(packetBody.SequenceEqual(outputcontent));
        //}
    }
}
