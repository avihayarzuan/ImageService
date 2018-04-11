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
        /// the method create the new file at required path
        /// </summary>
        /// <param name="model">
        /// the image service model
        /// </param>
        public NewFileCommand(IImageServiceModel model)
        {
            m_model = model;            // Storing the Model
        }
        /// <summary>
        /// execute the add file command
        /// </summary>
        /// <param name="args">
        /// the pathed to listen to
        /// </param>
        /// <param name="result">
        /// out bool to indicate if was successful
        /// </param>
        /// <returns>
        /// string which contains the error message
        /// </returns>
        public string Execute(string[] args, out bool result)
        {
            // The String Will Return the New Path if result = true, and will return the error message
            return m_model.AddFile(args[0], out result);
        }
    }
}

            //result = true;
            //string strRes = args[0];
            //// Going over each new file, adding it
            //foreach(string str in args) {
            //    this.m_model.AddFile(str, out result);
            //    if (!result)
            //    {
            //        //result = false;
            //        strRes = "semek"; //need to check about resukt and more shit
            //    }
            //}
            //return strRes;
