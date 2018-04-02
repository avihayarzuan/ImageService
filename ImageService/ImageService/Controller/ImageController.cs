using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModel m_model;                      // The Model Object
        private Dictionary<int, ICommand> commands;

        public ImageController(IImageServiceModel model)
        {
            m_model = model;                    // Storing the Model Of The System
            commands = new Dictionary<int, ICommand>();
            commands.Add(1, new NewFileCommand(m_model));
        }

        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            // rememeber to return the correct string blablabla
            resultSuccesful = true;
            if (!commands.ContainsKey(commandID)) {
                resultSuccesful = false;
                return "Command not found";
            }
            ICommand command = commands[commandID];
            return command.Execute(args, out bool res);
        }
    }
}
