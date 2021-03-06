﻿using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ImageService.Model
{
    public class ImageServiceModel : IImageServiceModel
    {

        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size
        private static Regex r = new Regex(":");
        #endregion
        /// <summary>
        /// Constructor of ImageServiceModel
        /// </summary>
        /// <param name="outputDir">
        /// the output directory
        /// </param>
        /// <param name="thumbnailSize">
        /// the size of the thumbnails to create
        /// </param>
        public ImageServiceModel(string outputDir, int thumbnailSize)
        {
            m_OutputFolder = outputDir;
            m_thumbnailSize = thumbnailSize;
        }
        /// <summary>
        /// Given the path to take the image from, it will copy to the output directory
        /// </summary>
        /// <param name="path">
        /// Path to take from the files
        /// </param>
        /// <param name="result">
        /// true if succeeded, false otherwise
        /// </param>
        /// <returns>
        /// The error message if exists
        /// </returns>
        public string AddFile(string path, out bool result)
        {
            try
            {
                // Creating our directory if not exist and hiding it
                DirectoryInfo di = Directory.CreateDirectory(m_OutputFolder);
                di.Attributes |= FileAttributes.Hidden;
                // Get the pictures date time
                DateTime picTime = GetDateTakenFromImage(path);
                // Creating strings in order to create our directory
                string picFolder = Path.Combine(picTime.Year.ToString(), picTime.Month.ToString());
                string imageName = Path.GetFileName(path);
                string picDestFolder = Path.Combine(m_OutputFolder, picFolder);
                string thumbFolderDest = Path.Combine(m_OutputFolder, "thumbnails", picFolder);
                string destPath = Path.Combine(picDestFolder, imageName);
                // Creating a folder for our picture and thumbnail
                Directory.CreateDirectory(picDestFolder);
                Directory.CreateDirectory(thumbFolderDest);
                // If the file already exists and its not the same file
                if (File.Exists(destPath) && !FileEquals( path,destPath))
                {
                    destPath = DuplicateFile(destPath);
                }
                // Lastly saving our created thumbnail and moving our image
                File.Move(path, destPath);
                SaveThumbnail(destPath, Path.Combine(thumbFolderDest, imageName));

                result = true;
                return destPath;
            }
            catch (Exception msg)
            {
                result = false;
                return msg.Message;
            }

        }
        /// <summary>
        /// Retrieves the datetime WITHOUT loading the whole image
        /// </summary>
        /// <param name="path">
        /// Path of the image
        /// </param>
        /// <returns>
        /// DateTime
        /// </returns>
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                try
                {
                    System.Drawing.Imaging.PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
                catch
                {
                    // Incase the image doesnt have a date we'll return the files date
                    return File.GetCreationTime(path);
                }
            }
        }

        /// <summary>
        /// Save the images thubmnail according to the thumbnailSize
        /// </summary>
        /// <param name="path">
        /// Path of the image
        /// </param>
        /// <param name="destDir">
        /// Path to put in the thumbnail
        /// </param>
        private void SaveThumbnail(string path, string destDir)
        {
            Image myImage = Image.FromFile(path);
            Image thumb = myImage.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);
            thumb.Save(destDir);
            // Lastly releasing the image
            myImage.Dispose();
        }

        /// <summary>
        /// In case there is a file with the same name
        /// The function copy the file to the output path and change the name
        /// of the file adding unique number at the end.
        /// </summary>
        /// <param name="path">
        /// The file path
        /// </param>
        /// <returns>
        /// The new file name path
        /// </returns>
        private string DuplicateFile(string path)
        {
            int i = 1;
            string imageName = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            string pathFolder = Path.GetDirectoryName(path);
            // As long as the file exists we'll rename it 
            while (File.Exists(path))
            {
                imageName = imageName + "(" + i.ToString() + ")";
                path = Path.Combine(pathFolder, imageName + extension);
                i++;
            }
            return path;
        }

        /// <summary>
        /// Simple function for compating two files by bytes
        /// </summary>
        /// <param name="path1">first files path</param>
        /// <param name="path2">second files path</param>
        /// <returns></returns>
        private bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
