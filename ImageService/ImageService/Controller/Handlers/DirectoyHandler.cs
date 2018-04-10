using ImageService.Model;
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

        public void HandlerClose()
        {
            foreach (FileSystemWatcher watcher in m_listWatchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            // some one can explain me this???????
            DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(m_path ,"dir" + m_path + "directory closed"));
        }

        public void StartHandleDirectory(string dirPath)
        {
            m_path = dirPath;
            string[] filters = { "*.jpg", "*.png", "*.gif" ,"*.bmp"};
            //List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
            foreach(string f in filters)
            {
                FileSystemWatcher watcher = new FileSystemWatcher
                {
                    Filter = f,
                    Path = dirPath
                };
                watcher.Created += new FileSystemEventHandler(OnCreated);
                watcher.EnableRaisingEvents = true;
                this.m_listWatchers.Add(watcher);
                //add where was added???
                m_logging.Log(f + " watcher added in " + dirPath, MessageTypeEnum.INFO);
            }

        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string[] path = { e.FullPath };
            string msg = m_controller.ExecuteCommand((int)CommandEnum.NewFileCommand, path , out bool result);
            if (result)
            {
                m_logging.Log("file in new path:" + msg, MessageTypeEnum.INFO);
            }
            else
            {
                m_logging.Log("could not move new file. reason: ", MessageTypeEnum.FAIL);
            }

        }

    }
}
