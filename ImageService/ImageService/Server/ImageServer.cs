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
        private List<IDirectoryHandler> handlers;
        private string[] handlersPath;
        #endregion

        #region Properties
        // The event that notifies about a new Command being recieved
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          
        #endregion

        public ImageServer(IImageController controller, ILoggingService logging, string[] handlersPath)
        {
            m_controller = controller;
            m_logging = logging;
            handlers = new List<IDirectoryHandler>();
            this.handlersPath = handlersPath;
            for (int i = 0; i < handlersPath.Length; i++)
            {
                handlers.Add(new DirectoyHandler(this.m_controller, this.m_logging));
                handlers[i].StartHandleDirectory(handlersPath[i]);
                m_logging.Log("Directory-Handler created at path:" + handlersPath[i], Logging.Model.MessageTypeEnum.INFO);
            }
        }

        // should we keep it???
        public void SendCommand()
        {
            
        }

        public void CloseServer()
        {
            CommandRecievedEventArgs commandRecEventArgs = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, null);
            CommandRecieved?.Invoke(this, commandRecEventArgs);
            for (int i = 0; i < handlersPath.Length; i++)
            {
                m_logging.Log("Handler stopped at path:" + handlersPath[i], Logging.Model.MessageTypeEnum.INFO);
            }
        }

    }
}
