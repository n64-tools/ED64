
namespace UnitTests
{
    [TestClass]
    public class MockCommandLineArgumentTests
    {

        //The following may require real hardware... Perhaps it should be a different proj???
        // https://stackoverflow.com/questions/36626668/simulate-command-line-argument-in-unit-tests

        [TestMethod]
        public void UploadFpgaRbf()
        {
            //-fpga=<filename>
        }

        [TestMethod]
        public void UsbDiagnostics()
        {
            //-diag
        }

        [TestMethod]
        public void UploadAndStartRom()
        {
            //<filename>
        }

        [TestMethod]
        public void UploadRom()
        {
            //-rom=<filename>
        }

        [TestMethod]
        public void StartRom()
        {
            //-start
        }

        [TestMethod]
        public void UploadForceRom()
        {
            //-forcerom=<filename>
        }

        [TestMethod]
        public void StartRomAdvanced()
        {
            //-start=<filename>
        }

        [TestMethod]
        public void DownloadRom()
        {
            //-drom =< filename >
        }

        [TestMethod]
        public void UploadFile()
        {
            //-cp testfile.txt sd:testfile1.txt
        }

        [TestMethod]
        public void DownloadFile()
        {
            //-cp sd:testfile1.txt testfile2.txt
        }

        [TestMethod]
        public void UploadScreenshot()
        {
            //-screen=<filename>
        }

    }
}