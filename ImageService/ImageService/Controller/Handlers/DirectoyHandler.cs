﻿using ImageService.Model;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Model;
using System.Text.RegularExpressions;
using System.Threading;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        //test
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        //private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private List<FileSystemWatcher> m_listWatchers = new List<FileSystemWatcher>();
        private string m_path;                              // The Path of directory

        #endregion

        // The Event That Notifies that the Directory is being closed
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;

        public DirectoyHandler(IImageController controller, ILoggingService logging)
        {
            m_controller = controller;
            m_logging = logging;
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (e.CommandID == (int)CommandEnum.CloseCommand)
            {
                HandlerClose();
            }
        }

        /// <summary>
        /// Closing our handler and the end of the service
        /// </summary>
        public void HandlerClose()
        {
            foreach (FileSystemWatcher watcher in m_listWatchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(m_path, "dir" + m_path + "directory closed"));
        }

        /// <summary>
        /// Starting our handler, watching the given path for new files added
        /// </summary>
        /// <param name="dirPath">
        /// The path for the FileSystemWatcher to watch
        /// </param>
        public void StartHandleDirectory(string dirPath)
        {
            // Creating our watcher and notifying accordingly
            FileSystemWatcher watcher = new FileSystemWatcher(m_path);
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.EnableRaisingEvents = true;
            this.m_listWatchers.Add(watcher);
            m_logging.Log(" watcher added in " + dirPath, MessageTypeEnum.INFO);

        }

        /// <summary>
        /// Invoking this function every time a new file is added to the watched folder
        /// </summary>
        /// <param name="sender">
        /// The invoker
        /// </param>
        /// <param name="e">
        /// Event arguments
        /// </param>
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string[] path = { e.FullPath };
            string[] filters = { ".jpg", ".png", ".gif", ".bmp" };
            string extension = Path.GetExtension(e.FullPath);
            // Checking if the new file is an image we should handle
            foreach (string f in filters)
            {
                // If so we'll execute the new fille command and logg approptiatly
                if (extension.Equals(f, StringComparison.InvariantCultureIgnoreCase))
                {
                    Thread.Sleep(100);
                    string msg = m_controller.ExecuteCommand((int)CommandEnum.NewFileCommand, path, out bool result);
                    if (result)
                    {
                        m_logging.Log("file in new path:" + msg, MessageTypeEnum.INFO);
                    }
                    else
                    {
                        m_logging.Log("could not move new file. reason: " + msg, MessageTypeEnum.FAIL);
                    }
                }
            }
        }

    }
}
