using ImageService.Controller;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Model;
using ImageService.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    class ClientHandlerMobile : IClientHandler
    {
        // all clients in the list are in a shape of a tuple as follows:
        private List<Tuple<TcpClient, NetworkStream>> activeClients;
        private ILoggingService m_logging;
        private IImageController m_controller;

        /// <summary>
        /// Constructor of ClientHandler Class.
        /// the Class responsible on the communication with all the clients.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="logging"></param>
        public ClientHandlerMobile(IImageController controller, ILoggingService logging)
        {
            this.activeClients = new List<Tuple<TcpClient, NetworkStream>>();
            m_logging = logging;
            m_controller = controller;
        }

        /// <summary>
        /// Given client, the method get his stream, binaryReader and BinaryWriter
        /// make's a Tuple of them' and start 'Handle'.
        /// </summary>
        /// <param name="client"></param>
        public void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            //BinaryReader reader = new BinaryReader(stream);
            //BinaryWriter writer = new BinaryWriter(stream);
            Tuple<TcpClient, NetworkStream> t = new Tuple<TcpClient, NetworkStream>(client, stream);
            this.activeClients.Add(t);
            new Task(() =>
            {
                {
                    try
                    {
                        while (true)
                        {
                            byte[] bytesArr = new byte[4096];
                            int res = stream.Read(bytesArr, 0, bytesArr.Length);
                            string message = Encoding.ASCII.GetString(bytesArr, 0, res);
                            string[] s = message.Split(' ');
                            int size = int.Parse(s[0]);
                            string name = s[1];
                            bytesArr = new byte[size];

                            int byteCount = stream.Read(bytesArr, 0, bytesArr.Length);
                            byte[] curr;
                            int temp;
                            while(byteCount<bytesArr.Length)
                            {
                                curr = new byte[size];
                                temp = stream.Read(curr, 0, curr.Length);
                                Transfer(bytesArr, curr, byteCount);
                                byteCount += temp;
                            }
                            ArrayToImage(name, bytesArr);
                        }
                    }
                    catch
                    {
                        m_logging.Log("Client has disconnected", Logging.Model.MessageTypeEnum.INFO);
                        client.Close();
                        this.activeClients.Remove(t);
                    }
                }
            }).Start();
        }

        public void ArrayToImage(string name, byte[] bytesArray)
        {
            using (var ms = new MemoryStream(bytesArray))
            {
                string path = ConfigurationManager.AppSettings["OutputDir"];
                File.WriteAllBytes(path + "\\" + name, bytesArray);
            }

        }

        public void Transfer(byte[] src, byte[] dst, int start)
        {
            for (int i = start; i < src.Length; i++)
            {
                src[i] = dst[i - start];
            }
        }

        public void SendCloseHandler(object sender, DirectoryCloseEventArgs e)
        {
            
        }

        public void SendLog(object sender, MessageRecievedEventArgs e)
        {
            
        }
    }
}



