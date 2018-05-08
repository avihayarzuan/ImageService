using ImageService.Controller;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Model;
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
        //private Dictionary<int, ICommand> commands;
        private List<TcpClient> activeClients;
        private ILoggingService m_logging;
        private IImageController m_controller;

        public ClientHandler(IImageController controller, ILoggingService logging)
        {
            this.activeClients = new List<TcpClient>();
            m_logging = logging;
            m_controller = controller;
        }

        public void HandleClient(TcpClient client)
        {
            this.activeClients.Add(client);
            new Task(() =>
            {
                using (NetworkStream stream = client.GetStream())
                using (BinaryReader reader = new BinaryReader(stream))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    try
                    {
                        string commandLine = reader.ReadString();
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
                    catch (Exception e)
                    {
                        m_logging.Log(e.Message, Logging.Model.MessageTypeEnum.FAIL);
                        client.Close();
                        this.activeClients.Remove(client);
                    }
                }
            }).Start();
        }


        public void SendLog(object sender, MessageRecievedEventArgs e)
        {
            string[] str = new string[2];
            str[0] = e.Status.ToString();
            str[1] = e.Message.ToString();

            JObject logObj = new JObject
            {
                ["CommandEnum"] = (int)CommandEnum.GetConfigCommand,
                ["logValue"] = JsonConvert.SerializeObject(str)
            };
            string message = logObj.ToString();
            int size = activeClients.Count;
            for (int i = 0; i < size; i++)
            {

                try
                {
                    using (NetworkStream stream = activeClients[i].GetStream())
                    using (BinaryReader reader = new BinaryReader(stream))
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(message);
                    }
                }
                catch
                {
                    activeClients[i].Close();
                    activeClients.Remove(activeClients[i]);
                }
            }
        }
    }

}
