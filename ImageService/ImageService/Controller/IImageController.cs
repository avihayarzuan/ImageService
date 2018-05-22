using ImageService.Model;
using System;

namespace ImageService.Controller
{
    public interface IImageController
    {
        event EventHandler<CommandRecievedEventArgs> CommandUpStream;

        string ExecuteCommand(int commandID, string[] args, out bool result); // Executing the Command Request
    }
}
