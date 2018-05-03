using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Commands;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Model;

namespace ImageService.ImageService.Commands
{
    
    class CloseCommand : ICommand
    {
        private List<IDirectoryHandler> handlersList;

        public CloseCommand(ref List<IDirectoryHandler> handlersList)
        {
            this.handlersList = handlersList;
        }

        public string Execute(string[] args, out bool result)
        {
            string path = args[1];
            result = true;

            foreach(var h in handlersList)
            {
                if (h.GetPath().Equals(path))
                {
                    CommandRecievedEventArgs e = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, null);
                    h.OnCommandRecieved(h, e);
                }
            }

            return path;
        }
    }
}
