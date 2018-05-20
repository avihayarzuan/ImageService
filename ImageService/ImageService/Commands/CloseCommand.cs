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
using Newtonsoft.Json.Linq;

namespace ImageService.ImageService.Commands
{
    
    class CloseCommand : ICommand
    {
        public event EventHandler<CommandRecievedEventArgs> CloseCommandEvent;
        /// <summary>
        /// Implements the ICommand interface
        /// When given closecommand, the method invokes the closeCommandEvent
        /// </summary>
        /// <param name="args">
        /// The command enum and path
        /// </param>
        /// <param name="result">
        /// </param>
        /// <returns></returns>
        public string Execute(string[] args, out bool result)
        {
            string path = args[1];
            CommandRecievedEventArgs e = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, path);
            CloseCommandEvent?.Invoke(this, e);

            result = true;

            JObject closeObj = new JObject
            {
                ["CommandEnum"] = (int)CommandEnum.CloseCommand,
                ["Directory"] = path
            };
            return closeObj.ToString();
        }
    }
}
