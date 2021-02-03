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
        public void Check_Basic_Command_String_Works()
        {
            string command = "commandname arg1 arg2";
            var output = new Unf.Debugger().ProcessSendCommand(command);
            Assert.AreEqual(command, Encoding.ASCII.GetString(output));
        }

        [TestMethod]
        public void Check_File_Command_String_Works()
        {
            string filename = @"c:\temp.txt";
            string command = $"@{filename}@";
            string fileText = "abcd";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{filename}", new MockFileData(fileText) },
            });
            string commandResult = "@4@abcd";
            var output = new Unf.Debugger(fileSystem).ProcessSendCommand(command);
            Assert.AreEqual(commandResult, Encoding.ASCII.GetString(output));
        }

        //[TestMethod]
        //public void Check_File_Command_String_Fails_when_over_8MB()
        //{
        //    string filename = @"c:\temp.txt";
        //    string command = $"@{filename}@";
        //    string fileText = "abcd";
        //    var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        //    {
        //        { $"{filename}", new MockFileData(fileText) },
        //    });
        //    string commandResult = "@4@abcd";
        //    var output = new Unf.Debugger(fileSystem).ProcessCommand(command);
        //    Assert.AreEqual(commandResult, output.CommandString);
        //}

        [TestMethod]
        public void Check_Append_File_Command_String_Alignment_Works()
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
        public void Check_Append_File_Command_String_After_Works()
        {
            string filename = @"c:\temp.txt";
            string command = $"commandname arg1 arg2 @{filename}@";
            string fileText = "abcd";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{filename}", new MockFileData(fileText) },
            });
            string commandResult = "commandname arg1 arg2 @4@abcd";
            var output = new Unf.Debugger(fileSystem).ProcessSendCommand(command);
            Assert.AreEqual(commandResult, Encoding.ASCII.GetString(output));
        }

        [TestMethod]
        public void Check_Append_File_Command_String_Before_Works()
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
        public void Check_Append_File_Command_String_Before_And_After_Works()
        {
            string filename = @"c:\temp.txt";
            string command = $"commandname arg1 arg2 @{filename}@ arg3";
            string fileText = "abcd";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{filename}", new MockFileData(fileText) },
            });
            string commandResult = "commandname arg1 arg2 @4@abcd arg3";
            var output = new Unf.Debugger(fileSystem).ProcessSendCommand(command);
            Assert.AreEqual(commandResult, Encoding.ASCII.GetString(output));
        }

        [TestMethod]
        public void Check_Text_Receive_Command_Processing_Works()
        {
            var packetBody = "abcd";
            var command = new List<byte>();
            command.AddRange(Encoding.ASCII.GetBytes("DMA@"));
            //command.AddRange(new byte[] { 0x01, 0x00, 0x00, 0x04} ); //Big Endian Example for below:
            command.AddRange(BitConverter.GetBytes((short)1)); //text, high byte so no need to reverse.
            command.AddRange(BitConverter.GetBytes((short)packetBody.Length).Reverse()); //Big Endian
            command.AddRange(Encoding.ASCII.GetBytes(packetBody));
            command.AddRange(Encoding.ASCII.GetBytes("CMPH"));
            command.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 }); //added padding. Should be a different test!
            var output = new Unf.Debugger().ProcessReceiveCommand(command.ToArray());
            Assert.AreEqual(packetBody, output);
        }

        [TestMethod]
        public void Check_Binary_Receive_Command_Processing_Works()
        {
            var packetBody = new byte[] { 0x00, 0x01, 0x02, 0x03};
            var command = new List<byte>();
            command.AddRange(Encoding.ASCII.GetBytes("DMA@"));
            //command.AddRange(new byte[] { 0x01, 0x00, 0x00, 0x04} ); //Big Endian Example for below:
            command.AddRange(BitConverter.GetBytes((short)2)); //text, high byte so no need to reverse.
            command.AddRange(BitConverter.GetBytes((short)packetBody.Length).Reverse()); //Big Endian
            command.AddRange(packetBody);
            command.AddRange(Encoding.ASCII.GetBytes("CMPH"));
            command.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 }); //added padding. Should be a different test!
            var output = new Unf.Debugger().ProcessReceiveCommand(command.ToArray());

            //read back the file created and compare it...
            var outputcontent = File.ReadAllBytes(output);

            Assert.IsTrue(packetBody.SequenceEqual(outputcontent));
        }

        //[TestMethod]
        //public void Check_Screenshot_Decode_Works()
        //{
        //    string command = "commandname arg1 arg2 @filename@ arg3";
        //    string fileText = "abcd";
        //    string commandResult = "commandname arg1 arg2 @4@abcd arg3";
        //    var output = UnfDebug.HandleCommand(command);
        //    Assert.AreEqual(output, command);
        //}
    }
}
