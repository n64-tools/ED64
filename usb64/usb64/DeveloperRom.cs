using System;
using System.Collections.Generic;
using System.Text;

namespace ed64usb
{
    public class DeveloperRom
    {
        public enum SaveType : byte
        {
            None = 0x00,
            Eeprom4k = 0x01,
            Eeprom16k = 0x02,
            Sram = 0x03,
            Sram768k = 0x04,
            FlashRam = 0x05,
            Sram128k = 0x06
        }

        public enum ExtraInfo : byte
        {
            Off = 0x00,
            Rtc = 0x01,
            NoRegion = 0x02,
            All = 0x03,
        }

        //public enum Cic : byte
        //{
        //    Any = 0x00,
        //    x6101 = 0x01,
        //    x6102 = 0x02,
        //    x6103 = 0x03,
        //    x5104 = 0x04,
        //    x6105 = 0x05,
        //    x6106 = 0x06,
        //    x5167 = 0x07
        //}
    }
}
