using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Text;
using System.Drawing.Imaging;

namespace GraphDLL
{
    public partial class Graph
    {
        internal static Socket mySocket;
        internal static Socket myClient;
        static byte[] myBuffer;
        static int port = 8001;

        static void Accepting()
        {
            try
            {
                myClient = mySocket.Accept();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void TCPinitListener()
        {
            IPEndPoint myIpEndPoint = new IPEndPoint(IPAddress.Any, port);
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mySocket.ReceiveBufferSize = 640 * 480 * 5;
            myBuffer = new byte[mySocket.ReceiveBufferSize];
            mySocket.Bind(myIpEndPoint);
            mySocket.Listen(1000);
            (new Thread(Accepting)).Start();
        }

        public static void TCPReceive(out byte[] data)
        {
            data = null;
            if (myClient != null && myClient.Available > 0)
            {
                myClient.Receive(myBuffer, 4, SocketFlags.None);
                int len = BitConverter.ToInt32(myBuffer, 0);

                try
                {
                    if (len > myBuffer.Length)
                    {
                        myBuffer = new byte[len];
                        myClient.ReceiveBufferSize = len;
                    }

                    while (myClient.Available < len) ;

                    int byteRead = myClient.Receive(myBuffer, len, SocketFlags.None);
                    if (len != byteRead)
                        throw new Exception();
                    byte[] formatted = new byte[byteRead];
                    Array.Copy(myBuffer, 0, formatted, 0, byteRead);
                    data = formatted;
                }
                catch { }
            }
        }
        public static void TCPReceive(out string s)
        {
            byte[] bytes;
            TCPReceive(out bytes);
            if (bytes != null)
                s = Encoding.ASCII.GetString(bytes);
            else
                s = null;
        }
        public static void TCPReceive(out System.Drawing.Image image)
        {
            byte[] data;
            image = null;
            TCPReceive(out data);
            if (data != null)
            {
                MemoryStream ms = new MemoryStream(data);
                try
                {
                    image = System.Drawing.Image.FromStream(ms);
                }
                catch { }
            }
        }
        public static void TCPReceive(out Bitmap bitmap)
        {
            System.Drawing.Image image;
            TCPReceive(out image);
            if (image != null)
                bitmap = new Bitmap((System.Drawing.Bitmap)image);
            else
                bitmap = null;
        }

        public static void TCPReceiveLast(out byte[] data)
        {
            data = null;

            byte[] buffer = null;
            TCPReceive(out buffer);
            if (buffer != null)
            {
                do
                {
                    data = buffer;
                    TCPReceive(out buffer);
                } while (buffer != null);
            }
        }
        public static void TCPReceiveLast(out System.Drawing.Bitmap image)
        {
            byte[] data;
            image = null;
            TCPReceiveLast(out data);
            if (data != null)
            {
                MemoryStream ms = new MemoryStream(data);
                try
                {
                    image = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(ms);                    
                }
                catch { }
            }
        }
        public static void TCPReceiveLast(out Bitmap bitmap)
        {
            System.Drawing.Bitmap image;
            TCPReceiveLast(out image);
            if (image != null)
                bitmap = new Bitmap((System.Drawing.Bitmap)image);
            else
                bitmap = null;
        }


        internal static Socket mySocket2;
        public static void TCPinitSender(string ip)
        {
            Console.Write("Waiting for listener.");
            IPEndPoint myIpEndPoint2 = new IPEndPoint(IPAddress.Parse(ip), port);
            mySocket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mySocket2.NoDelay = true;
            while (true)
                try
                {
                    mySocket2.Connect(myIpEndPoint2);
                    break;
                }
                catch
                {
                    Console.Write(".");
                }
            Console.WriteLine("done");
        }

        public static void TCPSend(byte[] data)
        {
            mySocket2.SendBufferSize = data.Length + 4;
            byte[] len = BitConverter.GetBytes(data.Length);
            mySocket2.Send(len);
            mySocket2.Send(data);
        }
        public static void TCPSend(string s)
        {
            TCPSend(Encoding.ASCII.GetBytes(s));
        }
        public static void TCPSend(System.Drawing.Image image)
        {
            MemoryStream ms = new MemoryStream();            
            image.Save(ms, ImageFormat.Png);
            TCPSend(ms.ToArray());
        }
        public static void TCPSend(Bitmap image)
        {
            TCPSend(image.SysDraw);
        }    
    }
}
