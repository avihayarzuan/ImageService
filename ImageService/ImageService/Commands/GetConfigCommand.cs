using ImageService.Commands;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;

namespace ImageService.ImageService.Commands
{
    class GetConfigCommand : ICommand
    {
        private List<IDirectoryHandler> list;

        public GetConfigCommand(ref List<IDirectoryHandler> list)
        {
            this.list = list;
        }

        /// <summary>
        /// The method parse configurations of the service and return a 
        /// JSON string with all the values.
        /// The class hold reference list to all the active directories.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public string Execute(string[] args, out bool result)
        {

            int size = this.list.Count;
            string[] handlerPaths = new string[size];
            for (int i = 0; i < size; i++)
            {
                handlerPaths[i] = list[i].GetPath();
            }

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
            result = true;
            return configObj.ToString();

        }
    }
}
