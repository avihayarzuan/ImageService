using ImageService.Logging.Model;
using ImageService.Model;
using System.Net.Sockets;

namespace ImageService.Server
{
    public interface IClientHandler
    {
        void HandleClient(TcpClient client);
        void SendLog(object sender, MessageRecievedEventArgs e);
        void SendCloseHandler(object sender, DirectoryCloseEventArgs e);
    }
}
