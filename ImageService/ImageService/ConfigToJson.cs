﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.ImageService
{
    class ConfigToJson
    {
        public static string ToJson()
        {

            string[] handlerPaths = ConfigurationManager.AppSettings["Handler"].Split(';');
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            string thumbnailSize = ConfigurationManager.AppSettings["ThumbnailSize"];
            string port = ConfigurationManager.AppSettings["port"];

            JObject configObj = new JObject
            {
                ["handlersPaths"] = JsonConvert.SerializeObject(handlerPaths),
                ["outputDir"] = outputDir,
                ["thumbnailSize"] = thumbnailSize,
                ["port"] = port
            };
            return configObj.ToString();
        }
    }
}
