using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        #endregion

        #region Properties
        // The event that notifies about a new Command being recieved
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          
        #endregion

        public ImageServer(IImageController controller, ILoggingService logging, string[] handlersPath)
        {
            m_controller = controller;
            m_logging = logging;

            for (int i = 0; i < handlersPath.Length; i++)
            {
                CreateHandler(handlersPath[i]);
                m_logging.Log("Handler created at path:" + handlersPath[i], Logging.Model.MessageTypeEnum.INFO);
            }
        }

        public void CreateHandler(string dirPath)
        {

        }
       
        public void SendCommand()
        {
            
        }

        public void CloseServer()
        {

        }

    }
}
