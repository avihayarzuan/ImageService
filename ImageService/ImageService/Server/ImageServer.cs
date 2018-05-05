using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private List<IDirectoryHandler> handlers;
        // part two members:
        private int port;
        private TcpListener listener;
        private IClientHandler ch;
        #endregion

        #region Properties
        // The event that notifies about a new Command being recieved
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        #endregion

        /// <summary>
        /// Constructor of the Server
        /// </summary>
        /// <param name="controller">
        /// ImageController
        /// </param>
        /// <param name="logging">
        /// LoggingService
        /// </param>
        /// <param name="handlersPath">
        /// The path the handler needs to 'handle'
        /// </param>
        public ImageServer(ILoggingService logging)
        {
            string[] handlersPath = ConfigurationManager.AppSettings["Handler"].Split(';');
            this.port = int.Parse(ConfigurationManager.AppSettings["port"]);

            m_controller = new ImageController();
            m_logging = logging;
            handlers = new List<IDirectoryHandler>();
            // Creating our handlers
            for (int i = 0; i < handlersPath.Length; i++)
            {
                handlers.Add(new DirectoyHandler(this.m_controller, this.m_logging));
                handlers[i].StartHandleDirectory(handlersPath[i]);
                CommandRecieved += handlers[i].OnCommandRecieved;
                handlers[i].DirectoryClose += OnHandlerClose;
                // Logging each handler creation into the entry
                m_logging.Log("Directory-Handler created at path:" + handlersPath[i], Logging.Model.MessageTypeEnum.INFO);
            }

            m_controller.CommandUpStream += SendCommand;
            this.ch = new ClientHandler(this.m_controller, logging);
            StartTcp();
            
            /// be added later:
            /// the sendlog function when every log is written
            //m_logging.MessageRecieved += ch.SendLog;
        }

        public void StartTcp()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            listener = new TcpListener(ep);
            listener.Start();
            m_logging.Log("Started TCP Server", Logging.Model.MessageTypeEnum.INFO);

            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        m_logging.Log("Got new connection", Logging.Model.MessageTypeEnum.INFO);

                        ch.HandleClient(client);
                    }
                    catch (SocketException)
                    {
                        m_logging.Log("Socket exception", Logging.Model.MessageTypeEnum.FAIL);
                        break;
                    }
                }
                m_logging.Log("Server stopped", Logging.Model.MessageTypeEnum.INFO);
            });
            task.Start();
        }

        /// <summary>
        /// when command is sent' the command event raise the event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SendCommand(object sender, CommandRecievedEventArgs e)
        {
            CommandRecieved?.Invoke(sender, e);
        }

        /// <summary>
        /// Invoke the CommandRecieved event.
        /// </summary>
        public void CloseServer()
        {
            CommandRecievedEventArgs ev = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, "*");
            CommandRecieved?.Invoke(this, ev);
            try
            {
                listener.Stop();
            }
            catch (Exception e)
            {
                m_logging.Log(e.Message, Logging.Model.MessageTypeEnum.FAIL);
            }
        }

        /// <summary>
        /// Delegate to run when the handler is being closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnHandlerClose(object sender, DirectoryCloseEventArgs e)
        {
            IDirectoryHandler dirHandler = (IDirectoryHandler)sender;
            CommandRecieved -= dirHandler.OnCommandRecieved;
            m_logging.Log("Stop handle directory " + e.Message, Logging.Model.MessageTypeEnum.INFO);
        }
    }
}
