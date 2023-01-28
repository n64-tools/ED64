using CommandLine;
//using System.Collections.Generic;


// TODO: this file is not yet used in anger!!!
namespace Krikzz.EverDrive64.Utilities.XSeries
{

    interface IStorageCopyOptions
    {
        // Original was "cp", but perhaps change to "filecopy"
        //Console.WriteLine("      i.e. SD:\\<filepath> to C:\\<filepath> OR C:\\<filepath> to SD:\\<filepath>.");
        [Option("src", SetName = "cp", Required = true, HelpText = "Copies a file between devices <src>.")]
        [Value(0, MetaName = "InputPath", HelpText = "Input (source) file-name including path")]
        string CopySourcePath { get; set; }
        [Option("dest", SetName = "cp", Required = true, HelpText = "Copies a file between devices <dest>.")]
        [Value(1, MetaName = "OutputPath", HelpText = "Output (destination) file-name including path")]
        string CopyDestinationPath { get; set; }

        // This is an alternative option for the above!
        //[Option(SetName = "cp", Required = true, Max = 2, HelpText = "Copies a file between devices.\r\n      i.e. SD:\\<filepath> to C:\\<filepath> OR C:\\<filepath> to SD:\\<filepath>.")]
        //IEnumerable<string> StorageCopyFiles { get; set; }
    }

    interface ISendRomFileOptions
    {
        [Option("path", SetName = "rom", Required = true, HelpText = "Loads specified ROM.")]
        [Value(0, MetaName = "InputPath", HelpText = "Input (source) file-name including path")]
        string RomFilePath { get; set; }

        [Option("force", SetName = "rom", Required = false, Default = false, HelpText = "Loads specified ROM, even though it is not of a known type (e.g. 64dd).")]
        bool ForceRomLoad { get; set; } // TODO: perhaps this should be ForceType with enum for: OFF, ROM, EMU, FPGA, OTHER

        [Option("savetype", SetName = "rom", Required = false, Default = "Off", HelpText = "Runs the ROM with a save type when not matched in the internal database.")]
        
        string RomSaveFlag { get; set; }
        //Console.WriteLine("-save=<savetype> (Runs the ROM with a save type when not matched in the internal database)");
        //Console.WriteLine("      Options: [None,Eeprom4k,Eeprom16k,Sram,Sram768k,FlashRam,Sram1024k].");

        [Option("extra", SetName = "rom", Required = false, Default = "None", HelpText = "Runs the ROM with RTC or forced region.")]
        string RomOptionFlags { get; set; }
        //Console.WriteLine("-extra=<RTC-RegionType> (Runs the ROM with RTC or forced region)");
        //Console.WriteLine("      Options: [Off,Rtc,NoRegion,All].");

        [Option("fpga", SetName = "rom", Required = false, Default = false, HelpText = "Loads specified file as FPGA RBF file.")]
        bool IsFpgaFile { get; set; }

        //[Option("unfdebug", SetName = "rom", Required = false, Default = false, HelpText = "Start UNF Debugger after file loads.")]
        //bool StartUfnDebugger { get; set; }

        [Option("tcpdebug", SetName = "rom", Required = false, Default = false, HelpText = "Start TCP Debugger after file loads.")]
        bool StartTcpDebugServer { get; set; }
        [Option("tcpdebugport", SetName = "rom", Required = false, Default = 3333, HelpText = "Set the TCP debugger port.")]
        uint TcpDebuggerPort { get; set; }

        [Option("autostart", SetName = "rom", Required = false, Default = true, HelpText = "Start the ROM after loaded.")]
        bool AutoStartRom { get; set; }
    }

    interface IRunCommsDiagnosticsOptions
    {
        [Option("diag", SetName = "diagnostics", Default = true, HelpText = "Runs communications diagnostics.")]
        bool RunCommDiagnostics { get; set; }
    }

    interface IRetrieveRomFileOptions
    {
        [Option("dest", SetName = "drom", HelpText = "Retrives loaded ROM to PC.")]
        [Value(0, MetaName = "OutputPath", HelpText = "Output (source) file-name including path")]
        string DumpRomOutputFilePath { get; set; }
    }

    interface IRetrieveScreenRamOptions
    {
        [Option("dest", SetName = "screen", HelpText = "Dumps framebuffer as BMP to PC.")]
        [Value(0, MetaName = "OutputPath", HelpText = "Output (source) file-name including path")]
        string DumpScreenOutputFilePath { get; set; }
    }

    public class CmdLineOptions : IStorageCopyOptions, ISendRomFileOptions, IRunCommsDiagnosticsOptions, IRetrieveRomFileOptions,  IRetrieveScreenRamOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        // These are IRunDiagnosticsOptions
        public bool RunCommDiagnostics { get; set; }

        // These are IRetrieveRomFileOptions
        public string DumpRomOutputFilePath { get; set; }

        // These are IRetrieveScreenRamOptions
        public string DumpScreenOutputFilePath { get; set; }

        // These are the IStorageCopyOptions options!
        public string CopySourcePath { get; set; }
        public string CopyDestinationPath { get; set; }
        //public IEnumerable<string> StorageCopyFiles { get; set; }

        // These are the ISendFileOptions options
        public string RomFilePath { get; set; }
        public bool ForceRomLoad { get; set; }
        public string RomSaveFlag { get; set; }
        public string RomOptionFlags { get; set; }
        public bool IsFpgaFile { get; set; }
        //public bool StartUfnDebugger { get; set; }
        public bool StartTcpDebugServer { get; set; }
        public uint TcpDebuggerPort { get; set; }
        public bool AutoStartRom { get; set; }


        [Option("start", Required = false, Default = "", HelpText = "Used for ROM save file. Only required when different from '-rom=<filename>'")]
        public string StartSpecifiedRomName { get; set; }

    }

}
