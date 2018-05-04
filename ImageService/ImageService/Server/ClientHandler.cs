using ImageService.Commands;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.ImageService.Commands;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
                        //string commandLine = reader.ReadLine();
                        //int num = reader.ReadInt32();
                        string commandLine = reader.ReadString();
                        m_logging.Log(commandLine, Logging.Model.MessageTypeEnum.INFO);
                        //Console.WriteLine(commandLine);
                        string[] s = commandLine.Split(' ');
                        //Console.WriteLine(first_word);
                        bool ret = int.TryParse(s[0], out int commandID);
                        if (ret)
                        {
                            //s = { commandLine };
                            string answer = m_controller.ExecuteCommand(commandID, s, out bool result);
                            //string answer = commands[commandID].Execute(s, out bool result);
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
                    //string result = commandLine.ex;
                }
                //client.Close();
            }).Start();
        }
    }
}
//try
//{
//}
//catch (Exception e)
//{
//    m_logging.Log(e.Message, Logging.Model.MessageTypeEnum.FAIL);
//client.Close();
//this.activeClients.Remove(client);
//}
//try
//{
//}
//catch (Exception e)
//{
//    m_logging.Log(e.Message, Logging.Model.MessageTypeEnum.FAIL);
//    client.Close();
//    this.activeClients.Remove(client);
//}
