using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDPChat
{
    internal class Program
    {
        static IPAddress ipAddress;
        static string yourName = "you";
        static string friendName = "friend";
        static int cursorPosition = 0;
        static string chatBox = "";
        static int port;
        static int rPort;
        static bool isInit = false;

        static void Main(string[] args)
        {
            if (OperatingSystem.IsWindows())
            {
                Console.SetWindowSize(120, 30);
                Console.SetBufferSize(120, 30);
            }

            Console.Write("Your name: ");
            yourName = Console.ReadLine();

            ipAddress = IPAddress.Parse("127.0.0.1");

            Random random = new Random();
            port = random.Next(49152, 65536);
            Console.WriteLine($"Your port: {port}");

            Console.Write("Friend's port: ");
            rPort = Int32.Parse(Console.ReadLine());

            Console.Clear();
            TextBoxInit();

            CommandSend($"COMMAND_SET_NAME_{yourName}");

            Thread receive = new Thread(Receive);

            receive.Start();

            while (true)
            {
                Send();
            }
        }

        static void TextBoxInit()
        {
            Console.SetCursorPosition(0, 29);
            Console.Write(">");
            for (int i = 0; i < 118; i++)
            {
                Console.Write(' ');
            }
            Console.SetCursorPosition(2, 29);
        }

        static void Receive()
        {
            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
            IPEndPoint rEndPoint = new IPEndPoint(ipAddress, rPort);
            while (true)
            {
                UdpClient client = new UdpClient(endPoint);

                byte[] buffer = client.Receive(ref rEndPoint);
                bool isCommand = false;
                string result = Encoding.UTF8.GetString(buffer);

                if (result.Length >= 7)
                {
                    if (result.Substring(0, 7) == "COMMAND")
                    {
                        ExecuteCommand(result.Trim('\0').Split('_'));

                        if (!isInit)
                        {
                            CommandSend($"COMMAND_SET_NAME_{yourName}");
                            isInit = true;
                            isCommand = true;
                        }
                    }
                }
                if(!isCommand)
                {
                    UpdateChat($"{friendName}: {result}", ConsoleColor.Red);
                }

                client.Close();
            }
        }

        static void ExecuteCommand(string[] args)
        {
            if(args[1] == "SET")
            {
                if(args[2] == "NAME")
                {
                    friendName = args[3];
                }
            }
        }

        static void Send()
        {
            ConsoleKeyInfo result = Console.ReadKey();

            if (result.Key == ConsoleKey.Enter)
            {
                UdpClient client = new UdpClient();
                IPEndPoint rEndPoint = new IPEndPoint(ipAddress, rPort);

                byte[] buffer = Encoding.UTF8.GetBytes(chatBox);

                client.Send(buffer, buffer.Length, rEndPoint);

                UpdateChat($"{yourName}: {chatBox}", ConsoleColor.Green);

                chatBox = "";
                TextBoxInit();
                client.Close();
            }
            else if(result.Key == ConsoleKey.Backspace)
            {
                if(chatBox.Length != 0)
                {
                    chatBox = chatBox.Substring(0, chatBox.Length - 1);
                    Console.Write(" \b");
                }
                else
                {
                    Console.Write(" ");
                }
            }
            else
            {
                chatBox += result.KeyChar;
            }
        }

        static void CommandSend(string command)
        {
            UdpClient client = new UdpClient();
            IPEndPoint rEndPoint = new IPEndPoint(ipAddress, rPort);
            byte[] comBuf = Encoding.UTF8.GetBytes(command);
            client.Send(comBuf, comBuf.Length, rEndPoint);
            client.Close();
        }

        static void UpdateChat(string message, ConsoleColor color)
        {
            if (cursorPosition > 25)
            {
                Console.Clear();
                TextBoxInit();
                cursorPosition = 0;
            }
            int l = Console.GetCursorPosition().Left;
            int t = Console.GetCursorPosition().Top;
            Console.SetCursorPosition(0, cursorPosition);
            cursorPosition++;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
            Console.SetCursorPosition(l, t);
        }
    }
}
