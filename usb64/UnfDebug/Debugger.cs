using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;

namespace Unf
{
    public class Debugger
    {
        //8MB is the max file size that is allowed (on the 64Drive)
        //TODO: On the ED64, we should take into account that we are overriding the end of the ROM space here.
        const int MAX_FILE_SIZE = 0x800000; 

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


        public DebugCommand ProcessCommand(string command, int alignment = 1)
        {
            if (command.Contains("@"))
            {
                var filePath = command.Split('@')[1];
                var fileSize = fileSystem.FileInfo.FromFileName(filePath).Length;

                // should do this in the file send?!
                if (fileSize > MAX_FILE_SIZE)
                {
                    throw new Exception("Exceeds maximum file size");
                }

                var filecontent = "";

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
                if (!string.IsNullOrEmpty(filePath))
                {
                    filecontent = fileSystem.File.ReadAllText(filePath);
                }

                // should do this straight after the file send!
                if (alignment != 0)
                {
                    if (fileSize % alignment != 0)
                    {
                        var size = fileSize;
                        fileSize = size + alignment - size % alignment;
                    }
                }

                //the file content might be binary, if so, handle!
                string adjustedCommand = $"{textBefore}@{fileSize}@{filecontent}{textAfter}";
                //send text before (if not blank
                //send @{fileSize}@
                //send file
                //send text after (if not empty)

                return new DebugCommand() {CommandString = adjustedCommand, FilePath = filePath };
            }
            else
            {
                return new DebugCommand() { CommandString = command };
            }
        }

        ///// <summary>
        ///// Sends the file to the flashcart
        ///// </summary>
        ///// <param name="filename">The filename of the file to send wrapped around @ symbols</param>
        //    void FileSend(string filename, int alignment = 0)
        //    {
        //        try
        //        {
        //            using (FileStream fs = File.OpenRead(filename))
        //            {
        //                // Ensure the filesize isn't too large
        //                if (fs.Length > MAX_FILE_SIZE)
        //                {
        //                    throw new Exception($"Cannot upload data larger than {MAX_FILE_SIZE}");
        //                }

        //                // Send the data to the connected flashcart
        //                port.Write($"@{fs.Length}@");

        //                port.Write((new BinaryReader(fs)).ReadBytes((int)fs.Length), 0, (int)fs.Length);

        //                // Console.WriteLine($"Sent file: {fs.Name}, with length: {fs.Length}");

        //          if (fs.Size % alignment != 0) //ignore alignment when zero (since div by 0)
        //          {
        //              var alignmentBytes = (fs.Size + alignment - fs.Size % alignment) - bytesWritten;
        //              port.Write(0, alignmentBytes);
        //          }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        // TODO: return file sent length?
        //    }

    }

    public class DebugCommand
    {
        public string CommandString { get; set; } = "";
        public string FilePath { get; set; } = "";
    }
}
