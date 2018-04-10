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

        public NewFileCommand(IImageServiceModel model)
        {
            m_model = model;            // Storing the Model
        }

        public string Execute(string[] args, out bool result)
        {
            // The String Will Return the New Path if result = true, and will return the error message

            result = true;
            string strRes = args[0];
            // Going over each new file, adding it
            foreach(string str in args) {
                this.m_model.AddFile(str, out bool reult);
                if (!result)
                {
                    //result = false;
                    strRes = "semek"; //need to check about resukt and more shit
                }
            }
            return strRes;
        }
    }
}
