// Contributed by NetworkFusion / JonesAlmighty

using System.Collections.Generic;
using System.IO;

namespace Unf
{
    public class ImageUtilities
    {

        private static readonly byte[] bmpHeader = new byte[54] {
                0x42, 0x4D, //(BM)
                0x36, 0x10, 0x0E, 0x00, //(File size)
                0x00, 0x00, //(reserved)
                0x00, 0x00, //(reserved)
                0x36, 0x00, 0x00, 0x00, //(offset = header size + info header size 54 bytes)
                //BITMAPINFOHEADER
                0x28, 0x00, 0x00, 0x00, //(Length = 40 bytes)
                0x80, 0x02, 0x00, 0x00, //(Bitmap width signed int = 640)
                0x20, 0xFE, 0xFF, 0xFF, //(Bitmap height signed int = -480 for "topdown")
                0x01, 0x00, //(number of colour planes MUST BE 1)
                0x18, 0x00, //(number of bits per pixel = 24)
                0x00, 0x00, 0x00, 0x00, //(compression)
                0x00, 0x10, 0x0E, 0x00, //(image size)
                0x00, 0x00, 0x00, 0x00, //(horizontal res) 3790
                0x00, 0x00, 0x00, 0x00, //(vertical res) 3800
                0x00, 0x00, 0x00, 0x00, //(number of colours)
                0x00, 0x00, 0x00, 0x00 //(number of important colours)
            };

        /// <summary>
        /// Converts the N64 framebuffer to a bitmap image
        /// </summary>
        /// <param name="horizontal">The screen width</param>
        /// <param name="vertical">The screen height</param>
        /// <param name="frameBuffer">The framebuffer as a byte array</param>
        /// <returns></returns>
        public static byte[] ConvertToBitmap(short horizontal, short vertical, byte[] frameBuffer)
        {
            var imageSize = horizontal * vertical * 3; //Colour Data(3 bytes)

            //filesize = imagesize + 54 //generally it is not used, but we will set it just incase!
            var filesize = imageSize + 54;
            bmpHeader[2] = (byte)(filesize & 0xff);
            bmpHeader[3] = (byte)(filesize >> 8);
            bmpHeader[4] = 0;
            bmpHeader[5] = 0;

            //image width (using short as the max res is 640)
            bmpHeader[18] = (byte)(horizontal & 0xff);
            bmpHeader[19] = (byte)(horizontal >> 8);
            bmpHeader[20] = 0;
            bmpHeader[21] = 0;

            //negitive height for "top-down" bitmap (using short as the max res is 480)
            var topdownHeight = vertical * -1;
            bmpHeader[22] = (byte)(topdownHeight & 0xff);
            bmpHeader[23] = (byte)(topdownHeight >> 8);
            bmpHeader[24] = 0xff;
            bmpHeader[25] = 0xff;

            //imagesize //generally it is not used, but we will set it just incase!
            bmpHeader[34] = (byte)(imageSize & 0xff);
            bmpHeader[35] = (byte)(imageSize >> 8);
            bmpHeader[36] = 0;
            bmpHeader[37] = 0;

            var imageData = new List<byte>();
            imageData.AddRange(bmpHeader);

            using (BinaryReader stream = new BinaryReader(new MemoryStream(frameBuffer)))
            {

                for (int h = 0; h < vertical; h++)
                {
                    for (int w = 0; w < horizontal; w++)
                    {
                        var colour = stream.ReadBytes(2);

                        int red = (colour[0] & 0xf8);
                        int green = ((colour[0] & 0x07) << 5) | ((colour[1] & 0xc0) >> 3);
                        int blue = (colour[1] & 0x3e) << 2;

                        imageData.Add((byte)blue);
                        imageData.Add((byte)green);
                        imageData.Add((byte)red);
                    }
                }
            }

            return imageData.ToArray();
        }

        ///// <summary>
        ///// Converts the N64 framebuffer to a png image
        ///// </summary>
        ///// <param name="horizontal">The screen width</param>
        ///// <param name="vertical">The screen height</param>
        ///// <param name="frameBuffer">The framebuffer as a byte arrat</param>
        ///// <returns></returns>
        //public static byte[] ConvertToPng(short width, short height, byte[] frameBuffer)
        //{
        //    var imageSize = width * height * 4; //Colour Data(3 bytes)

        //    var imageData = new List<byte>();

        //    using (BinaryReader stream = new BinaryReader(new MemoryStream(frameBuffer)))
        //    {

        //        for (int h = 0; h < height; h++)
        //        {
        //            for (int w = 0; w < width; w++)
        //            {
        //                var colour = stream.ReadBytes(2);

        //                int red = (colour[0] & 0xf8);
        //                int green = ((colour[0] & 0x07) << 5) | ((colour[1] & 0xc0) >> 3);
        //                int blue = (colour[1] & 0x3e) << 2;

        //                imageData.Add((byte)blue);
        //                imageData.Add((byte)green);
        //                imageData.Add((byte)red);

        //                //        //        for (int i = 0; i < (int)cart->bytes_read; i += 4)
        //                //        //        {
        //                //        //            int texel = swap_endian((buffer[i + 3] << 24) & 0xFF000000 | (buffer[i + 2] << 16) & 0xFF0000 | (buffer[i + 1] << 8) & 0xFF00 | buffer[i] & 0xFF);
        //                //        //            if (debug_headerdata[1] == 2)
        //                //        //            {
        //                //        //                short pixel1 = texel >> 16;
        //                //        //                short pixel2 = texel;
        //                //        //                image[j++] = 0x08 * ((pixel1 >> 11) & 0x001F); // R1
        //                //        //                image[j++] = 0x08 * ((pixel1 >> 6) & 0x001F);  // G1
        //                //        //                image[j++] = 0x08 * ((pixel1 >> 1) & 0x001F);  // B1
        //                //        //                image[j++] = 0xFF;

        //                //        //                image[j++] = 0x08 * ((pixel2 >> 11) & 0x001F); // R2
        //                //        //                image[j++] = 0x08 * ((pixel2 >> 6) & 0x001F);  // G2
        //                //        //                image[j++] = 0x08 * ((pixel2 >> 1) & 0x001F);  // B2
        //                //        //                image[j++] = 0xFF;
        //                //        //            }
        //                //        //            else
        //                //        //            {
        //                //        //                // TODO: Test this because I sure as hell didn't >:V
        //                //        //                image[j++] = (texel >> 24) & 0xFF; // R
        //                //        //                image[j++] = (texel >> 16) & 0xFF; // G
        //                //        //                image[j++] = (texel >> 8) & 0xFF; // B
        //                //        //                image[j++] = (texel >> 0) & 0xFF; // Alpha
        //                //        //            }
        //                //        //        }
        //            }
        //        }
        //    }

        //    return imageData.ToArray();
        //}

    }
}

