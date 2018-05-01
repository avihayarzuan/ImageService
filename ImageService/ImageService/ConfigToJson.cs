using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.ImageService
{
    class ConfigToJson
    {
        public static string ToJson()
        {
            JObject configObj = new Jobject();
            return configObj.toString();
        }
        //string[] handlerPaths = ConfigurationManager.AppSettings["Handler"].Split(';');
        //string outputDir = ConfigurationManager.AppSettings["OutputDir"];
        //int thumbnailSize = Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);
        //    // Initializing and creating our members
        //    this.model = new ImageServiceModel(outputDir, thumbnailSize);
        //    this.controller = new ImageController(this.model);
        //    this.logging = new LoggingService();
        //int port = int.Parse(ConfigurationManager.AppSettings["port"]);
    }
}
