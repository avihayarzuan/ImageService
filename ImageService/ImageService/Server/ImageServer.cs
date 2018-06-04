using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ImageService.ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private List<IDirectoryHandler> handlers;
        private TcpServer server;

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

            handlers = new List<IDirectoryHandler>();
            m_controller = new ImageController(ref handlers);
            m_logging = logging;
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

            this.server = new TcpServer(m_controller, logging);
            this.server.Start();

            /* the following comment cancel the log to update automaticlly all the clients on a new log*/
            //m_logging.MessageRecieved += server.SendLog;
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
            this.handlers.Remove(dirHandler);
            m_logging.Log("Stop handle directory " + e.Message, Logging.Model.MessageTypeEnum.INFO);
            this.server.SendClose(sender, e);
        }
    }
}
