using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Commands;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Model;
using ImageService.Server;

namespace ImageService.ImageService.Commands
{
    
    class CloseCommand : ICommand
    {
        public event EventHandler<CommandRecievedEventArgs> CloseCommandEvent;
        public string Execute(string[] args, out bool result)
        {
            string path = args[1];
            CommandRecievedEventArgs e = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, path);
            CloseCommandEvent?.Invoke(this, e);

            result = true;
            return path;
        }
    }
}
