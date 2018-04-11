using ImageService.Infrastructure;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModel m_model;
        /// <summary>
        /// Constructor for our New file command
        /// </summary>
        /// <param name="model">
        /// The service model logic to execute the command
        /// </param>
        public NewFileCommand(IImageServiceModel model)
        {
            m_model = model;            // Storing the Model
        }
        /// <summary>
        /// Executing our New file command
        /// </summary>
        /// <param name="args">
        /// The path in which the image is in
        /// </param>
        /// <param name="result">
        /// Whether or not the fille adding operation was successful or not
        /// </param>
        /// <returns>
        /// The image path if successfull otherwise an error message
        /// </returns>
        public string Execute(string[] args, out bool result)
        {
            // The String Will Return the New Path if result = true, and will return the error message
            return m_model.AddFile(args[0], out result);
        }
    }
}
