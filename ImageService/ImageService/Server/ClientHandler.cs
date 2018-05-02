using ImageService.Commands;
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
        private Dictionary<int, ICommand> commands;
        private List<TcpClient> activeClients;
        private ILoggingService m_logging;

        public ClientHandler(ILoggingService logging, ref List<IDirectoryHandler> handlers)
        {
            commands = new Dictionary<int, ICommand>
            {
                { (int)CommandEnum.GetConfigCommand, new ImageService.Commands.GetConfigCommand()},
                { (int)CommandEnum.LogCommand, new ImageService.Commands.LogCommand()},
                { (int)CommandEnum.CloseCommand, new CloseCommand(ref handlers)}
            };

            this.activeClients = new List<TcpClient>();
            m_logging = logging;
        }

        public void HandleClient(TcpClient client)
        {
            this.activeClients.Add(client);
            new Task(() =>
            {
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    string commandLine = reader.ReadLine();
                    string first_word = commandLine.Split(' ').First();
                    if (int.TryParse(first_word, out int commandID))
                    {
                        string[] s = { commandLine };
                        string answer = commands[commandID].Execute(s, out bool result);
                        try
                        {
                            writer.Write(answer);
                        } catch (Exception e)
                        {
                            m_logging.Log(e.Message, Logging.Model.MessageTypeEnum.FAIL);
                            this.activeClients.Remove(client);
                        }
                    }
                    else
                    {
                        try
                        {
                            writer.Write("Error in commandID");
                            m_logging.Log("Error in commandID", Logging.Model.MessageTypeEnum.FAIL);
                        }
                        catch (Exception e)
                        {
                            m_logging.Log(e.Message, Logging.Model.MessageTypeEnum.FAIL);
                            this.activeClients.Remove(client);
                        }
                    }
                    //string result = commandLine.ex;
                }
                client.Close();
            }).Start();
        }
    }
}
