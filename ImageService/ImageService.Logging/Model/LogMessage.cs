using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.ImageService.Logging.Model
{
    public class LogMessage
    {
        public string Type { get; private set; }
        public string Message { get; private set; }

        public LogMessage(string type, string msg)
        {
            this.Type = type;
            this.Message = msg;
        }
    }
}
