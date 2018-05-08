
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

namespace ImageService.ImageService.Commands
{
    class GetConfigCommand : ICommand
    {

        public string Execute(string[] args, out bool result)
        {
            result = true;
            //return ConfigToJson.ToJson();
            string[] handlerPaths = ConfigurationManager.AppSettings["Handler"].Split(';');
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            string src = ConfigurationManager.AppSettings["SourceName"];
            string log = ConfigurationManager.AppSettings["LogName"];
            string thumbnailSize = ConfigurationManager.AppSettings["ThumbnailSize"];
            string port = ConfigurationManager.AppSettings["port"];

            JObject configObj = new JObject
            {
                ["CommandEnum"] = (int)CommandEnum.GetConfigCommand,
                ["handlersPaths"] = JsonConvert.SerializeObject(handlerPaths),
                ["outputDir"] = outputDir,
                ["SourceName"] = src,
                ["LogName"] = log,
                ["thumbnailSize"] = thumbnailSize,
                ["port"] = port
            };
            return configObj.ToString();

        }
    }
}
