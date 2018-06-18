using ImageService.Controller;
using ImageService.Logging;
using ImageService.Logging.Model;
using ImageService.Model;
using ImageService.Server;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.ImageService.Server
{
    class TcpServer
    {
        private int port;
        private TcpListener listener;
        private IClientHandler ch;
        private ILoggingService m_logging;

        /// <summary>
        /// Constructor of the Tcp Server
        /// The server starts a thread that constantly listen to clients
        /// </summary>
        /// <param name="m_controller"></param>
        /// <param name="logging"></param>
        public TcpServer(IImageController m_controller, ILoggingService logging)
        {
            this.port = int.Parse(ConfigurationManager.AppSettings["port"]);
            this.m_logging = logging;
            //this.ch = new ClientHandler(m_controller, logging);
            this.ch = new ClientHandlerMobile(m_controller, logging);
        }

        /// <summary>
        /// Start listen to clients in a different thread
        /// </summary>
        public void Start()
        {
            Thread.Sleep(10000);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7999);
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
        /// Stop listening to port
        /// </summary>
        public void Stop()
        {
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
        /// Delegate method to send activate an event in upper class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SendLog(object sender, MessageRecievedEventArgs e)
        {
            ch.SendLog(sender, e);
        }
        /// <summary>
        /// Delegate method to send activate an event in upper class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SendClose(object sender, DirectoryCloseEventArgs e)
        {
            ch.SendCloseHandler(sender, e);
        }
    }
}
