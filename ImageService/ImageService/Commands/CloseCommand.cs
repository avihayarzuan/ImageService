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
        private event EventHandler<CommandRecievedEventArgs> CommandRecieved;

        public CloseCommand(ref EventHandler<CommandRecievedEventArgs> CommandRecieved)
        {
            this.CommandRecieved = CommandRecieved;
        }

        public string Execute(string[] args, out bool result)
        {
            string path = args[1];
            CommandRecievedEventArgs e = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, path);
            CommandRecieved?.Invoke(this, e);
            //foreach(var h in handlersList)
            //{
            //    if (h.GetPath().Equals(path))
            //    {
            //        CommandRecievedEventArgs e = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, null);
            //        h.OnCommandRecieved(h, e);
            //    }
            //}

            result = true;
            return path;
        }
    }
}
