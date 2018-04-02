﻿using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Model
{
    public class ImageServiceModel : IImageServiceModel
    {

        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size
        //we init this once so that if the function is repeatedly called
        //it isn't stressing the garbage man
        private static Regex r = new Regex(":");
        #endregion

        public ImageServiceModel(string outputDir, int thumbnailSize)
        {
            m_OutputFolder = outputDir;
            m_thumbnailSize = thumbnailSize;
        }

        public string AddFile(string path, out bool result)
        {
            // Get the pictures date time
            DateTime picTime = GetDateTakenFromImage(path);
            // Creating a path for it assuming it exists and for the thumbnail
            string picFolder = picTime.Year.ToString() + "/" + picTime.Month.ToString();
            Directory.CreateDirectory(m_OutputFolder + "/" + picFolder);
            Directory.CreateDirectory(m_OutputFolder + "/Thumbnails" + picFolder);
            Directory.Move(path, m_OutputFolder + "/" + picFolder);
            // lastly saving the thumbnail
            SaveThumbnail(path, m_OutputFolder + "/Thumbnails" + picFolder);
            //when and why to set result values????
            result = true;
            return "";
        }

        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                System.Drawing.Imaging.PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }

        // Save the images thubmnail according to the thumbnailSize
        private void SaveThumbnail(string path, string destDir)
        {
            Image myImage = Image.FromFile(path);
            Image thumb = myImage.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);
            thumb.Save(destDir);
        }

    }
}
