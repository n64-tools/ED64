////# include "Include/lodepng.h"
////# include "main.h"
////# include "helper.h"
////# include "device.h"
////# include "debug.h"

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using System.IO;

//namespace ed64usb
//{

//    //Append -d to enable debug mode, which allows you to receive/send input from/to the console (Assuming you're using the included USB+debug libraries). 
//    //If you wrap a part of a command in '@' characters, the data will be treated as a file and will be uploaded to the cart. When uploading files in a command, 
//    //the filepath wrapped between the '@' characters will be replaced with the size of the data inside the file, with the data in the file itself being appended after. 
//    //For example, if there is a file called file.txt with 4 bytes containing abcd, sending the following command: 
//    //commandname arg1 arg2 @file.txt@ arg4 will send commandname arg1 arg2 @4@abcd arg4 to the console. UNFLoader only supports sending 1 file per command.

//    /// <summary>
//    /// UNF Debug Library
//    /// </summary>
//    public class UnfDebugOld
//    {


//        //Consts
//        const bool VERBOSE = false; //shoud be bool
//        const int BUFFER_SIZE = 512;
//        const int HEADER_SIZE = 16;
//        const int BLINKRATE = 40;
//        const int PATH_SIZE = 256; //probably irrelivent!


//        // Global Variables
//        const int DATATYPE_TEXT = 0x01; //probably should be an enum!
//        const int DATATYPE_RAWBINARY = 0x02;
//        const int DATATYPE_HEADER = 0x03;
//        const int DATATYPE_SCREENSHOT = 0x04;
//        //static int debug_headerdata[HEADER_SIZE];

//        System.IO.Ports.SerialPort port;

//        /// <summary>
//        /// The main debug loop for input/output
//        /// </summary>
//        /// <param name="port">The cartridge serial port</param>
//        public void Start(System.IO.Ports.SerialPort cart)
//        //void Debug(ftdi_context_t* cart) //Port?
//        {
//            port = cart;
//            byte[] outbuff = new byte[BUFFER_SIZE], inbuff = new byte[BUFFER_SIZE];
//            //ushort cursorpos = 0;
//            //DWORD pending = 0;
//            //WINDOW* inputwin = newwin(1, getmaxx(stdscr), getmaxy(stdscr) - 1, 0);
//            //time_t debugtimeout = clock() + global_timeout * CLOCKS_PER_SEC;
//            int alignment;



//            //if (global_timeout != 0)
//            //    Console.WriteLine(" Timeout after %d seconds.", CRDEF_INPUT, global_timeout);
//            //Console.WriteLine("\n\n", CRDEF_INPUT);
//            //timeout(0);
//            //curs_set(0);

//            // Initialize our buffers
//            //outbuff = (char*)malloc(BUFFER_SIZE);
//            //inbuff = (char*)malloc(BUFFER_SIZE);
//            //memset(inbuff, 0, BUFFER_SIZE);

//            // Open file for debug output
//            //if (global_debugout != null)
//            //{
//            //    global_debugoutptr = fopen(global_debugout, "w+");
//            //    if (global_debugoutptr == null)
//            //    {
//            //        Console.WriteLine("\n", CRDEF_ERROR);
//            //        throw new Exception("Unable to open %s for writing debug output.", global_debugout);
//            //    }
//            //}

//            // Decide the alignment based off the cart that's connected
//            alignment = 16;
//            //switch (cart->carttype)
//            //{
//            //    case CART_EVERDRIVE: alignment = 16; break;
//            //    //case CART_SC64: alignment = 4; break;
//            //    default: alignment = 0;
//            //}

//            // Start the debug server loop
//            Console.WriteLine("Debug mode started. Press ESC to stop.");
//            while (Console.ReadKey().Key != ConsoleKey.Escape)
//            {
//                //// If ESC is pressed, stop the loop
//                //if (ch == ConsoleKey.Escape || (global_timeout != 0 && debugtimeout < clock()))
//                //    break;

//                TextInput(cart, inbuff, Console.ReadKey());

//                // Check if we have pending data
//                //FT_GetQueueStatus(cart->handle, &pending);
//                int pending = cart.BytesToRead;

//                if (pending > 0)
//                {
//                    int info = 0;
//                    int read = 0;
//                    if (VERBOSE)
//                    {
//                        Console.ForegroundColor = ConsoleColor.Yellow;
//                        Console.WriteLine($"Receiving {pending} bytes");
//                        Console.ResetColor();
//                    }

//                    // Ensure we have valid data by reading the header
//                    //FT_Read(cart->handle, outbuff, 4, &cart->bytes_read);
//                    read += cart.Read(outbuff, 0, cart.BytesToRead);
//                    //read += cart->bytes_read;
//                    if (outbuff[0] != 'D' || outbuff[1] != 'M' || outbuff[2] != 'A' || outbuff[3] != '@')
//                    {
//                        //terminate("Unexpected DMA header: %c %c %c %c.", outbuff[0], outbuff[1], outbuff[2], outbuff[3]);
//                        throw new Exception($"Unexpected DMA header: {outbuff[0]} {outbuff[1]} {outbuff[2]} {outbuff[3]}.");
//                    }

//                    // Get information about the incoming data
//                    //FT_Read(cart->handle, outbuff, 4, &cart->bytes_read);
//                    read += port.Read(outbuff, 0, 4);
//                    //read += cart->bytes_read;
//                    info = (int)swap_endian((uint)(outbuff[3] << 24 | outbuff[2] << 16 | outbuff[1] << 8 | outbuff[0]));

//                    // Decide what to do with the received data
//                    DecideData(cart, info, outbuff, read);

//                    // Read the completion signal
//                    //FT_Read(cart->handle, outbuff, 4, &cart->bytes_read);
//                    read += port.Read(outbuff, 0, 4);
//                    //read += cart->bytes_read;
//                    if (outbuff[0] != 'C' || outbuff[1] != 'M' || outbuff[2] != 'P' || outbuff[3] != 'H')
//                    {
//                        //terminate("Did not receive completion signal: %c %c %c %c.", outbuff[0], outbuff[1], outbuff[2], outbuff[3]);
//                        throw new Exception($"Did not receive completion signal:{ outbuff[0] } { outbuff[1]} { outbuff[2]} { outbuff[3]}.");
//                    }

//                    // Ensure byte alignment by reading X amount of bytes needed
//                    if (alignment != 0 && (read % alignment) != 0)
//                    {
//                        int left = alignment - (read % alignment);
//                        //FT_Read(cart->handle, outbuff, left, &cart->bytes_read);
//                        int bytes_read = port.Read(outbuff, 0, left);
//                    }

//                    // Reset the timeout clock
//                    //debugtimeout = clock() + global_timeout * CLOCKS_PER_SEC;
//                }

//                // If we have no more data, sleep a bit to be kind to the CPU
//                //FT_GetQueueStatus(cart->handle, &pending);
//                //if (pending == 0)
//                //{
//                //    Thread.Sleep(10);

//                //}


//                // Close the debug output file if it exists
//                //if (global_debugoutptr != null)
//                //{
//                //    fclose(global_debugoutptr);
//                //    global_debugoutptr = null;
//                //}

//                // Clean up everything
//                //free(outbuff);
//                //free(inbuff);
//                //wclear(inputwin);
//                //wrefresh(inputwin);
//                //delwin(inputwin);
//                //curs_set(0);
//            }
//        }


//        /*==============================
//            debug_textinput
//            Handles text input in the console
//            @param A pointer to the cart context
//            @param A pointer to the input window
//            @param A pointer to the input buffer
//            @param A pointer to the cursor position value
//            @param The inputted character
//        ==============================*/

//        //void TextInput(System.IO.Ports.SerialPort cart, char inputwin, byte[] buffer, ushort cursorpos, ConsoleKeyInfo ch)
//        void TextInput(System.IO.Ports.SerialPort cart, ref byte[] buffer, ConsoleKeyInfo ch)
//        {
//            //int blinker = 0;
//            int size = 0;

//            // Decide what to do on key presses
//            if (ch.Key == ConsoleKey.Enter && size != 0)
//            {
//                // Check if we're only sending a file or text (and potentially a file appended)
//                if (buffer[0] == '@' && buffer[size - 1] == '@')
//                    FileSend(buffer.ToString().Replace("@","")); //we probably need to strip the @'s off!
//                //else
//                    //AppendFileSend(buffer, size + 1);
//                //memset(buffer, 0, BUFFER_SIZE);
//                //(*cursorpos) = 0;
//                size = 0;
//            }
//            //else if (ch.Key == ConsoleKey.Backspace)
//            //{
//            //    //if ((*cursorpos) > 0)
//            //    // {
//            //    //     buffer[--(*cursorpos)] = 0;
//            //    //     size--;
//            //    // }
//            //}
//            //else if (ch.KeyChar < 128 && ch.KeyChar > 15)
//            //{
//            //    buffer[(*cursorpos)++] = ch;
//            //    size++;
//            //}

//            // Display what we've written
//            //pdprintw_nolog(inputwin, buffer, CRDEF_INPUT);
//            Console.ForegroundColor = ConsoleColor.Yellow;
//            Console.Write(buffer.ToString());
//            Console.ResetColor();

//            //// Draw the blinker
//            //blinker = (blinker++) % (1 + BLINKRATE * 2);
//            //if (blinker >= BLINKRATE)
//            //{
//            //    int x, y;
//            //    getyx(inputwin, y, x);
//            //    mvwaddch(inputwin, y, (*cursorpos), 219);
//            //    wmove(inputwin, y, x);
//            //}
//            //werase(inputwin);
//        }


//        /*==============================
//            debug_appendfilesend
//            Sends the data to the flashcart with a file appended if needed
//            @param The data to send
//            @param The size of the data
//        ==============================*/

//        //void AppendFileSend(byte[] data, int size)
//        //{
//        //    string finaldata;
//        //    string filestart = "";//strchr(data, '@');
//        //    if (filestart != null)
//        //    {
//        //        //FILE* fp;
//        //        int charcount = 0;
//        //        int filesize = 0;
//        //        char sizestring[8];
//        //        string filepath = "";//(char*)malloc(512);
//        //        string lastat;
//        //        string fileend = "";//strchr(++filestart, '@');

//        //        // Ensure we managed to malloc for the filename
//        //        //if (filepath == null)
//        //        //{
//        //        //    Console.WriteLine("Unable to allocate memory for filepath\n", CRDEF_ERROR);
//        //        //    return;
//        //        //}

//        //        //// Check if the filepath is valid
//        //        //if (fileend == null || (*filestart) == '\0')
//        //        //{
//        //        //    Console.WriteLine("Missing terminating '@'\n", CRDEF_ERROR);
//        //        //    //free(filepath);
//        //        //    return;
//        //        //}

//        //        //// Store the filepath and its character count
//        //        //charcount = fileend - filestart;
//        //        //strncpy(filepath, filestart, charcount);
//        //        //filepath[charcount] = '\0';
//        //        //fileend++;

//        //        //// Attempt to open the file
//        //        //fp = fopen(filepath, "rb+");
//        //        //if (fp == NULL)
//        //        //{
//        //        //    Console.WriteLine("Unable to open file '%s'\n", CRDEF_ERROR, filepath);
//        //        //    //free(filepath);
//        //        //    return;
//        //        //}

//        //        //// Get the filesize
//        //        //fseek(fp, 0L, SEEK_END);
//        //        //filesize = ftell(fp);
//        //        //rewind(fp);
//        //        //sprintf(sizestring, "%d", filesize);

//        //        //// Allocate memory for the new data buffer
//        //        //size = (size - charcount) + filesize + strlen(sizestring);
//        //        //finaldata = (char*)malloc(size + 4); // Plus 4 for byte alignment on the 64Drive (so we don't write past the buffer)
//        //        //if (finaldata == null)
//        //        //{
//        //        //    //pdprint("Unable to allocate memory for USB data\n", CRDEF_ERROR);
//        //        //    free(filepath);
//        //        //    fclose(fp);
//        //        //    return;
//        //        //}
//        //        //memset(finaldata, 0, size + 4);

//        //        //// Rewrite the data with the new format
//        //        //memcpy(finaldata, data, filestart - data);
//        //        //strcat(finaldata, sizestring);
//        //        //strcat(finaldata, "@");
//        //        //lastat = strchr(finaldata, '@') + strlen(sizestring) + 2;
//        //        //fread(lastat, 1, filesize, fp);
//        //        //strcat(lastat + filesize, fileend);

//        //        //// Clean up all the memory we borrowed
//        //        //fclose(fp);
//        //        //free(filepath);

//        //        try
//        //        {
//        //            using (FileStream fs = File.OpenRead(filepath))
//        //            {
//        //            }

//        //        }
//        //        catch (Exception ex)
//        //        {
//        //            Console.WriteLine(ex);
//        //        }
//        //    }
//        //    else
//        //        finaldata = data;

//        //    // Ensure the data isn't too large
//        //    if (size > 0x800000)
//        //    {
//        //        Console.ForegroundColor = ConsoleColor.Red;
//        //        Console.WriteLine("Cannot upload data larger than 8MB"); //\n", CRDEF_ERROR);
//        //        Console.ResetColor();
//        //        //if (filestart != NULL)
//        //          //  free(finaldata);
//        //        return;
//        //    }

//        //    // Send the data to the connected flashcart
//        //    //device_senddata(DATATYPE_TEXT, finaldata, size);
//        //    port.Write((new BinaryReader(fs)).ReadBytes((int)fs.Length), 0, (int)fs.Length);

//        //    // Free the final data buffer and print success
//        //    //if (filestart != NULL)
//        //    //    free(finaldata);
//        //    pdprint_replace("Sent command '%s'\n", CRDEF_INFO, data);
//        //}


//        /// <summary>
//        /// Sends the file to the flashcart
//        /// </summary>
//        /// <param name="filename">The filename of the file to send wrapped around @ symbols</param>
//        void FileSend(string filename)
//        {
//            try
//            {
//                using (FileStream fs = File.OpenRead(filename))
//                {
//                    // Ensure the filesize isn't too large
//                    if (fs.Length > 0x800000)
//                    {
//                        throw new Exception("Cannot upload data larger than 8M");
//                    }

//                    // Send the data to the connected flashcart
//                    port.Write($"@{fs.Length}@");

//                    port.Write((new BinaryReader(fs)).ReadBytes((int)fs.Length), 0, (int)fs.Length);

//                    Console.WriteLine($"Sent file: {fs.Name}, with length: {fs.Length}");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine(ex);
//                Console.ResetColor();
//                //throw ex;
//            }
//        }


//        /*==============================
//            debug_decidedata
//            Decides what function to call based on the command type stored in the info
//            @param A pointer to the cart context
//            @param 4 bytes with the info and size (from the cartridge)
//            @param The buffer to use
//            @param A pointer to a variable that stores the number of bytes read
//        ==============================*/

//        //void decidedata(ftdi_context_t* cart, uint info, byte[] buffer, uint read)
//        void DecideData(System.IO.Ports.SerialPort cart, int info, byte[] buffer, int read)
//        {
//            int command = (info >> 24) & 0xFF;
//            int size = info & 0xFFFFFF;

//            // Decide what to do with the data based off the command type
//            switch (command)
//            {
//                case DATATYPE_TEXT: HandleText(cart, size, buffer, read); break;
//                //case DATATYPE_RAWBINARY: HandleRawbinary(cart, size, buffer, read); break;
//                case DATATYPE_HEADER: HandleHeader(cart, size, buffer, read); break;
//                //case DATATYPE_SCREENSHOT: HandleScreenshot(cart, size, buffer, read); break;
//                default: throw new Exception($"Did not receive completion signal: \"Unknown data type.\"");
//            }
//        }


//        /*==============================
//            debug_handle_text
//            Handles DATATYPE_TEXT
//            @param A pointer to the cart context
//            @param The size of the incoming data
//            @param The buffer to use
//            @param A pointer to a variable that stores the number of bytes read
//        ==============================*/

//        void HandleText(System.IO.Ports.SerialPort cart, int size, byte[] buffer, int read)
//        {
//            int total = 0;
//            int left = size;

//            // Ensure the data fits within our buffer
//            if (left > BUFFER_SIZE)
//                left = BUFFER_SIZE;

//            // Read bytes until we finished
//            while (left != 0)
//            {
//                // Read from the USB and print it
//                //FT_Read(cart->handle, buffer, left, &cart->bytes_read);
//                int bytes_read = port.Read(buffer, 0, left);
//                Console.ForegroundColor = ConsoleColor.Blue;
//                Console.WriteLine("%s", buffer); //CRDEF_PRINT, 
//                Console.ResetColor();

//                // Store the amount of bytes read
//                //(*read) += cart->bytes_read;
//                //total += cart->bytes_read;

//                // Ensure the data fits within our buffer
//                left = size - total;
//                if (left > BUFFER_SIZE)
//                    left = BUFFER_SIZE;
//            }
//        }


//        /*==============================
//            debug_handle_rawbinary
//            Handles DATATYPE_RAWBINARY
//            @param A pointer to the cart context
//            @param The size of the incoming data
//            @param The buffer to use
//            @param A pointer to a variable that stores the number of bytes read
//        ==============================*/

//        //void HandleRawbinary(System.IO.Ports.SerialPort cart, int size, byte[] buffer, int read)
//        //{
//        //    int total = 0;
//        //    int left = size;
//        //    string filename = (char*)malloc(PATH_SIZE);
//        //    string extraname = gen_filename();
//        //    FILE* fp;

//        //    // Ensure we malloced successfully
//        //    if (filename == null || extraname == null)
//        //        throw new Exception("Unable to allocate memory for binary file.");

//        //    // Create the binary file to save data to
//        //    memset(filename, 0, PATH_SIZE);
//        //    if (global_exportpath != null)
//        //        strcat_s(filename, PATH_SIZE, global_exportpath);
//        //    strcat_s(filename, PATH_SIZE, "binaryout-");
//        //    strcat_s(filename, PATH_SIZE, extraname);
//        //    strcat_s(filename, PATH_SIZE, ".bin");
//        //    fopen_s(&fp, filename, "wb+");

//        //    // Ensure the file was created
//        //    if (fp == NULL)
//        //        terminate("Unable to create binary file.");

//        //    // Ensure the data fits within our buffer
//        //    if (left > BUFFER_SIZE)
//        //        left = BUFFER_SIZE;

//        //    // Read bytes until we finished
//        //    while (left != 0)
//        //    {
//        //        // Read from the USB and save it to our binary file
//        //        FT_Read(cart->handle, buffer, left, &cart->bytes_read);
//        //        fwrite(buffer, 1, left, fp);

//        //        // Store the amount of bytes read
//        //        (*read) += cart->bytes_read;
//        //        total += cart->bytes_read;

//        //        // Ensure the data fits within our buffer
//        //        left = size - total;
//        //        if (left > BUFFER_SIZE)
//        //            left = BUFFER_SIZE;
//        //    }

//        //    // Close the file and free the memory used for the filename
//        //    Console.WriteLine("Wrote %d bytes to %s.\n", CRDEF_INFO, size, filename);
//        //    //fclose(fp);
//        //    //free(filename);
//        //    //free(extraname);
//        //}


//        /*==============================
//            debug_handle_header
//            Handles DATATYPE_HEADER
//            @param A pointer to the cart context
//            @param The size of the incoming data
//            @param The buffer to use
//            @param A pointer to a variable that stores the number of bytes read
//        ==============================*/

//        int HandleHeader(System.IO.Ports.SerialPort cart, int size, byte[] buffer, int read)
//        {
//            int total = 0;
//            int left = size;

//            // Ensure the data fits within our buffer
//            if (left > HEADER_SIZE)
//                left = HEADER_SIZE;

//            // Read bytes until we finished
//            while (left != 0)
//            {
//                // Read from the USB and save it to the global headerdata
//                int readBytes = cart.Read(buffer, 0, left);
//                //FT_Read(cart->handle, buffer, left, &cart->bytes_read);
//                for (int i = 0; i < (int)cart->bytes_read; i += 4)
//                {
//                    headerdata[i / 4] = swap_endian((uint)(buffer[i + 3] << 24 | buffer[i + 2] << 16 | buffer[i + 1] << 8 | buffer[i]));
//                }

//                // Store the amount of bytes read
//                //(*read) += cart->bytes_read;
//                //total += cart->bytes_read;

//                // Ensure the data fits within our buffer
//                left = size - total;
//                if (left > HEADER_SIZE)
//                    left = HEADER_SIZE;
//                return readBytes;
//            }
//        }


//        /*==============================
//            debug_handle_screenshot
//            Handles DATATYPE_SCREENSHOT
//            @param A pointer to the cart context
//            @param The size of the incoming data
//            @param The buffer to use
//            @param A pointer to a variable that stores the number of bytes read
//        ==============================*/

//        //void handle_screenshot(System.IO.Ports.SerialPort cart, int size, byte[] buffer, uint read)
//        //{
//        //    int total = 0;
//        //    int left = size;
//        //    int j = 0;
//        //    u8* image;
//        //    int w = debug_headerdata[2], h = debug_headerdata[3];
//        //    string filename = (char*)malloc(PATH_SIZE);
//        //    string extraname = gen_filename();

//        //    // Ensure we got a data header of type screenshot
//        //    if (debug_headerdata[0] != DATATYPE_SCREENSHOT)
//        //        terminate("Unexpected data header for screenshot.");

//        //    // Allocate space for the image
//        //    image = (u8*)malloc(4 * w * h);

//        //    // Ensure we malloced successfully
//        //    if (filename == NULL || extraname == NULL || image == NULL)
//        //        terminate("Unable to allocate memory for binary file.");

//        //    // Create the binary file to save data to
//        //    memset(filename, 0, PATH_SIZE);
//        //    if (global_exportpath != NULL)
//        //        strcat_s(filename, PATH_SIZE, global_exportpath);
//        //    strcat_s(filename, PATH_SIZE, "screenshot-");
//        //    strcat_s(filename, PATH_SIZE, extraname);
//        //    strcat_s(filename, PATH_SIZE, ".png");

//        //    // Ensure the data fits within our buffer
//        //    if (left > BUFFER_SIZE)
//        //        left = BUFFER_SIZE;

//        //    // Read bytes until we finished
//        //    while (left != 0)
//        //    {
//        //        // Read from the USB and save it to our binary file
//        //        //FT_Read(cart->handle, buffer, left, &cart->bytes_read);
//        //        bytes_read = cart.Read(buffer, 0, left);
//        //        for (int i = 0; i < (int)cart->bytes_read; i += 4)
//        //        {
//        //            int texel = swap_endian((buffer[i + 3] << 24) & 0xFF000000 | (buffer[i + 2] << 16) & 0xFF0000 | (buffer[i + 1] << 8) & 0xFF00 | buffer[i] & 0xFF);
//        //            if (debug_headerdata[1] == 2)
//        //            {
//        //                short pixel1 = texel >> 16;
//        //                short pixel2 = texel;
//        //                image[j++] = 0x08 * ((pixel1 >> 11) & 0x001F); // R1
//        //                image[j++] = 0x08 * ((pixel1 >> 6) & 0x001F);  // G1
//        //                image[j++] = 0x08 * ((pixel1 >> 1) & 0x001F);  // B1
//        //                image[j++] = 0xFF;

//        //                image[j++] = 0x08 * ((pixel2 >> 11) & 0x001F); // R2
//        //                image[j++] = 0x08 * ((pixel2 >> 6) & 0x001F);  // G2
//        //                image[j++] = 0x08 * ((pixel2 >> 1) & 0x001F);  // B2
//        //                image[j++] = 0xFF;
//        //            }
//        //            else
//        //            {
//        //                // TODO: Test this because I sure as hell didn't >:V
//        //                image[j++] = (texel >> 24) & 0xFF; // R
//        //                image[j++] = (texel >> 16) & 0xFF; // G
//        //                image[j++] = (texel >> 8) & 0xFF; // B
//        //                image[j++] = (texel >> 0) & 0xFF; // Alpha
//        //            }
//        //        }

//        //        // Store the amount of bytes read
//        //        (*read) += cart->bytes_read;
//        //        total += cart->bytes_read;

//        //        // Ensure the data fits within our buffer
//        //        left = size - total;
//        //        if (left > BUFFER_SIZE)
//        //            left = BUFFER_SIZE;
//        //    }

//        //    // Close the file and free the dynamic memory used
//        //    lodepng_encode32_file(filename, image, w, h);
//        //    pdprint("Wrote %dx%d pixels to %s.\n", CRDEF_INFO, w, h, filename);
//        //    //free(image);
//        //    //free(filename);
//        //    //free(extraname);
//        //}

//        //FROM HELPER.CPP:

//        uint swap_endian(uint val)
//        {
//            return ((val << 24)) |
//                   ((val << 8) & 0x00ff0000) |
//                   ((val >> 8) & 0x0000ff00) |
//                   ((val >> 24));
//        }
//    }
//}
