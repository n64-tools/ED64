using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

namespace usb64UnitTests
{
    [TestClass]
    public class UnfDebugger
    {
        [TestMethod]
        public void Check_Basic_Command_String_Works()
        {
            string command = "commandname arg1 arg2";
            var output = new Unf.Debugger().ProcessCommand(command);
            Assert.AreEqual(output.CommandString, command);
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
            var output = new Unf.Debugger(fileSystem).ProcessCommand(command);
            Assert.AreEqual(commandResult, output.CommandString);
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
            var output = new Unf.Debugger(fileSystem).ProcessCommand(command, 4);
            Assert.AreEqual(commandResult, output.CommandString);
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
            var output = new Unf.Debugger(fileSystem).ProcessCommand(command);
            Assert.AreEqual(commandResult, output.CommandString);
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
            var output = new Unf.Debugger(fileSystem).ProcessCommand(command);
            Assert.AreEqual(commandResult, output.CommandString);
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
            var output = new Unf.Debugger(fileSystem).ProcessCommand(command);
            Assert.AreEqual(commandResult, output.CommandString);
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
