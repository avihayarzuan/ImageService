using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Model
{
    public interface IImageServiceModel
    {
        /// <summary>
        /// Adding a new file to our output folder
        /// </summary>
        /// <param name="path">
        /// The path image to be added.
        /// </param>
        /// <returns>
        /// The files new path if successful or an error message
        /// </returns>
        string AddFile(string path, out bool result);
    }
}
