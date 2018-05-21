using ImageService.Server;
using ImageService.Commands;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Controller.Handlers;

namespace ImageService.ImageService.Commands
{
    class GetConfigCommand : ICommand
    {
        private List<IDirectoryHandler> list;
        public GetConfigCommand(ref List<IDirectoryHandler> list)
        {
            this.list = list;
        }

        public string Execute(string[] args, out bool result)
        {
            result = true;

            int size = this.list.Count;
            string[] handlerPaths = new string[size];
            for (int i = 0; i < size; i++)
            {
                handlerPaths[i] = list[i].GetPath();
            }
            


            //string[] handlerPaths = ConfigurationManager.AppSettings["Handler"].Split(';');
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            string src = ConfigurationManager.AppSettings["SourceName"];
            string log = ConfigurationManager.AppSettings["LogName"];
            string thumbnailSize = ConfigurationManager.AppSettings["ThumbnailSize"];
            string port = ConfigurationManager.AppSettings["port"];

            JObject configObj = new JObject
            {
                ["CommandEnum"] = (int)CommandEnum.GetConfigCommand,
                ["handlersPaths"] = JsonConvert.SerializeObject(handlerPaths),
                ["Output"] = outputDir,
                ["SourceName"] = src,
                ["LogName"] = log,
                ["thumbnailSize"] = thumbnailSize,
                ["port"] = port
            };
            return configObj.ToString();

        }
    }
}
