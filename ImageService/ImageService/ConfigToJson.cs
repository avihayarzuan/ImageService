using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;

namespace ImageService.ImageService
{
    class ConfigToJson
    {
        public static string ToJson()
        {

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
