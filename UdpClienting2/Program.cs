using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpClienting2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(address, 61234);
            IPEndPoint rEndPoint = new IPEndPoint(address, 51234);

            Console.ReadLine();

            UdpClient client = new UdpClient(endPoint);

            byte[] buffer = Encoding.UTF8.GetBytes("Hello from Custom2");

            client.Send(buffer, buffer.Length, rEndPoint);

            Console.ReadLine();
        }
    }
}
