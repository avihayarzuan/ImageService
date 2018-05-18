using ImageService.Logging.Model;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public interface IClientHandler
    {
        void HandleClient(TcpClient client);
        void SendLog(object sender, MessageRecievedEventArgs e);
        void SendCloseHandler(object sender, DirectoryCloseEventArgs e);
    }
}
