using ImageService.Controller;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Model;
using ImageService.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ImageService.Server
{
    class ClientHandler : IClientHandler
    {
        private List<Tuple<TcpClient,NetworkStream,BinaryReader, BinaryWriter>> activeClients;
        private ILoggingService m_logging;
        private IImageController m_controller;

        public ClientHandler(IImageController controller, ILoggingService logging)
        {
            this.activeClients = new List<Tuple<TcpClient,NetworkStream,BinaryReader,BinaryWriter>>();
            m_logging = logging;
            m_controller = controller;

        }

        public void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            BinaryReader reader = new BinaryReader(stream);
            BinaryWriter writer = new BinaryWriter(stream);
            Tuple<TcpClient, NetworkStream, BinaryReader, BinaryWriter> t =
                new Tuple<TcpClient, NetworkStream, BinaryReader, BinaryWriter>
                (client, stream, reader, writer);
            this.activeClients.Add(t);

            new Task(() =>
            {
                {
                    try
                    {
                        while (true)
                        {

                            string commandLine = t.Item3.ReadString();
                            Console.WriteLine(commandLine);
                            if (commandLine == null)
                            {
                                continue;
                            }
                            m_logging.Log(commandLine, Logging.Model.MessageTypeEnum.INFO);
                            string[] s = commandLine.Split(' ');
                            bool ret = int.TryParse(s[0], out int commandID);
                            if (ret)
                            {
                                string answer = m_controller.ExecuteCommand(commandID, s, out bool result);
                                m_logging.Log("activate command" + commandID, Logging.Model.MessageTypeEnum.INFO);
                                writer.Write(answer);
                            }
                            else
                            {
                                writer.Write("Error in commandID");
                                m_logging.Log("Error in commandID", Logging.Model.MessageTypeEnum.FAIL);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        m_logging.Log(e.Message, Logging.Model.MessageTypeEnum.FAIL);
                        client.Close();
                        this.activeClients.Remove(t);
                    }
                }
            }).Start();
        }

        public void SendCloseHandler(object sender, DirectoryCloseEventArgs e)
        {
            JObject closeObj = new JObject
            {
                ["CommandEnum"] = (int)CommandEnum.CloseCommand,
                ["Directory"] = e.DirectoryPath
            };
            string message = closeObj.ToString();
            int size = activeClients.Count;
            for (int i = 0; i < size; i++)
            {

                try
                {
                    {
                        activeClients[i].Item4.Write(message);
                    }
                }
                catch
                {
                    activeClients[i].Item2.Close();
                    activeClients.Remove(activeClients[i]);
                }
            }
        }

        public void SendLog(object sender, MessageRecievedEventArgs e)
        {
            string[] str = new string[2];
            str[0] = e.Status.ToString();
            str[1] = e.Message.ToString();

            JObject logObj = new JObject
            {
                ["CommandEnum"] = (int)CommandEnum.LogCommand,
                ["logValue"] = JsonConvert.SerializeObject(str)
            };
            string message = logObj.ToString();
            int size = activeClients.Count;
            for (int i = 0; i < size; i++)
            {

                try
                {
                    {
                        activeClients[i].Item4.Write(message);
                    }
                }
                catch
                {
                    activeClients[i].Item2.Close();
                    activeClients.Remove(activeClients[i]);
                }
            }
        }
    }

}
