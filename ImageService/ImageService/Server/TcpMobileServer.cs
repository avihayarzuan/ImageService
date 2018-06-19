using ImageService.Controller;
using ImageService.Logging;
using ImageService.Logging.Model;
using ImageService.Model;
using ImageService.Server;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ImageService.ImageService.Server
{
    class TcpMobileServer
    {
        private int port;
        private TcpListener listener;
        private IClientHandler ch;
        private ILoggingService m_logging;

        /// <summary>
        /// Constructor of the Tcp Mobile Server
        /// The server starts a thread that constantly listen to clients of mobile port
        /// </summary>
        /// <param name="m_controller"></param>
        /// <param name="logging"></param>
        public TcpMobileServer(IImageController m_controller, ILoggingService logging)
        {
            this.port = int.Parse(ConfigurationManager.AppSettings["MobilePort"]);
            this.m_logging = logging;
            this.ch = new ClientHandlerMobile(m_controller, logging);
        }

        /// <summary>
        /// Start listen to clients in a different thread
        /// </summary>
        public void Start()
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
    }
}
