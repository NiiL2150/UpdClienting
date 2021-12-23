using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpClienting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(address, 51234);
            IPEndPoint rEndPoint = new IPEndPoint(address, 61234);

            UdpClient client = new UdpClient(endPoint);

            byte[] buffer = client.Receive(ref rEndPoint);

            string result = Encoding.UTF8.GetString(buffer);

            Console.WriteLine(result);

            Console.ReadLine();
        }
    }
}
