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
            try
            {
                // Get the pictures date time
                DateTime picTime = GetDateTakenFromImage(path);
                string imageName = Path.GetFileName(path);
                string picFolder = Path.Combine(picTime.Year.ToString(), picTime.Month.ToString());
                string picDestFolder = Path.Combine(m_OutputFolder, picFolder);
                string thumbFolderDest = Path.Combine(m_OutputFolder, "thumbnails", picFolder);
                // Creating a folder for our picture and thumbnail
                Directory.CreateDirectory(picDestFolder);
                Directory.CreateDirectory(thumbFolderDest);
                // If the file already exists we'll give it a new name
                if (File.Exists(Path.Combine(picDestFolder, imageName)))
                {
                    imageName = DuplicateFile(picDestFolder, imageName);
                }
                // Lastly saving our created thumbnail and moving our image
                SaveThumbnail(path, Path.Combine(thumbFolderDest, imageName));
                File.Move(path, Path.Combine(picDestFolder, imageName));

                result = true;
                return Path.Combine(picDestFolder, imageName);
            }
            catch (Exception msg)
            {
                result = false;
                return msg.ToString();
            }
            
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
            myImage.Dispose();
        }

        private string DuplicateFile(string folder, string imageName)
        {
            int i = 1;
            // As long as the file exists we'll rename it 
            while (File.Exists(Path.Combine(folder, imageName)))
            {
                imageName = imageName + "(" + i.ToString() + ")";
                i++;
            }
            return imageName;
        } 

    }
}
