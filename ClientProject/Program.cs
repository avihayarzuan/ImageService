using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("hello\n");
            //Console.WriteLine("Press any key to exit.");

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            TcpClient client = new TcpClient();
            client.Connect(ep);
            Console.WriteLine("You are connected");
            using (NetworkStream stream = client.GetStream())
            using (BinaryReader reader = new BinaryReader(stream))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Send data to server
                Console.Write("Please enter a number: ");
                string num = Console.ReadLine();
                Console.Write(num);
                writer.Write(num);
                // Get result from server
                string result = reader.ReadString();
                Console.WriteLine(result);
            }
            client.Close();
            Console.ReadKey();
            //IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            //TcpClient client = new TcpClient();
            //client.Connect(ep);
            //Console.WriteLine("You are connected");
            //using (NetworkStream stream = client.GetStream())
            //using (BinaryReader reader = new BinaryReader(stream))
            //using (BinaryWriter writer = new BinaryWriter(stream))
            //{
            //    while (true)
            //    {

            //    // Send data to server
            //    Console.Write("Please enter a number: ");
            //    int num = int.Parse(Console.ReadLine());
            //    writer.Write(num);
            //    // Get result from server
            //    int result = reader.ReadInt32();
            //    Console.WriteLine("Result = {0}", result);
            //    }
            //}
            //client.Close();
        }
    }
}
