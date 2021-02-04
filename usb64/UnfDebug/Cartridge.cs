using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unf
{
    public class Cartridge
    {
        private enum Identifier : int
        {
            Unknown = 0x00,
            SixtyFourDriveV1 = 0x01,
            SixtyFourDriveV2 = 0x02,
            Ed64x = 0x03,
            Ed64v3 = 0x05,
            SummerCart64V1 = 0x04
        }

        public enum CicType
        {
            Unknown = 0,
            x101,
            x102,
            x103,
            x105,
            x106
        }

        public enum SaveType
        {
            Unknown = 0,
            EEPROM4K,
            EEPROM16K,
            SRAM,
            FRAM,
            CPAK

        }

        public enum Region
        {
            Unknown = 0,
            NTSC,
            PAL,
            MPAL
        }


        public string Vendor()
        {
            //retroactive
            //krikzz
            //SummerCart
            return "";
        }

        public string Version()
        {
            return "";
        }
    }
}
