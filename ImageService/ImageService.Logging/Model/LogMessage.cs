using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.ImageService.Logging.Model
{
    /// <summary>
    /// Class that holds a two strings
    /// One of type message, the second is the message itself
    /// </summary>
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
