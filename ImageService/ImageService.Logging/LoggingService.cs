using ImageService.Logging.Model;
using System;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {

        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        /// <summary>
        /// Notify all subscribers of a new message recieved logging it into the entry log
        /// </summary>
        /// <param name="message">
        /// The actuall message
        /// </param>
        /// <param name="type">
        /// The message type for the entry log
        /// </param>
        public void Log(string message, MessageTypeEnum type)
        {
            // Upadting the messsages arguments
            MessageRecievedEventArgs msgRec = new MessageRecievedEventArgs
            {
                Status = type,
                Message = message
            };
            // Notifying all subscribers of the recieved message
            MessageRecieved?.Invoke(this, msgRec);
        }
    }
}
