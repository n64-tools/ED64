using Krikzz.EverDrive64.Utilities.XSeries;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Krikzz.EverDrive64.Utilities
{
    internal static class TcpProxy
    {
        private static TcpListener server = null;


        internal static void CreateServer(int port)
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Enter the listening loop.
                while (true)
                {
                    Debug.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    using TcpClient client = server.AcceptTcpClient();

                    Debug.WriteLine("Connected!");

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    // Set a 10 millisecond timeout for reading.
                    stream.ReadTimeout = 10;

                    while (client.Connected)
                    {

                        // Buffer for reading data
                        byte[] bytes = new byte[32768];
                        byte[] receivedBytes = new byte[32768]; //should be spanbyte!

                        int i;

                        // receive all the data sent by the cart.
                        UsbInterface.Read(receivedBytes, 0, receivedBytes.Length);
                        Debug.WriteLine("Received from cart, sent to TCP: {0}", receivedBytes);
                        // Send to tcp client.
                        stream.Write(receivedBytes, 0, receivedBytes.Length);

                        // Loop to receive all the data sent by the tcpclient.
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            UsbInterface.Write(bytes, 0, i);
                            Debug.WriteLine("Received from TCP, Sent to Cart: {0}", bytes);
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

    }
}
