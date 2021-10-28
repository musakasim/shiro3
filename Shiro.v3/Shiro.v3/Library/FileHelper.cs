using System;
using System.IO;
using Microsoft.Win32;

namespace Shiro.Library
{
    public class FileHelper
    {
        /// <summary>
        /// Finds a directory if it is under base directory of app's directory
        /// </summary>
        /// <param name="directoryName">Directory name which is being searched</param>
        /// <returns></returns>
        public static string FindInAppDirectory(string directoryName)
        {
            string location = AppDomain.CurrentDomain.BaseDirectory;
            //string location = new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
            return FindInDirectory(location, directoryName);
        }

        public static string FindInDirectory(string baseDirectory, string directoryName)
        {
            var directoryNames = Directory.GetDirectories(baseDirectory);

            foreach (var dirFullName in directoryNames)
            {
                if (new DirectoryInfo(dirFullName).Name == directoryName)
                    return dirFullName;
            }
            //search in subdirectories:
            foreach (var dirFullName in directoryNames)
            {
                var subDirFullName = FindInDirectory(dirFullName, directoryName);
                if (!string.IsNullOrEmpty(subDirFullName) && new DirectoryInfo(subDirFullName).Name == directoryName)
                    return subDirFullName;
            }
            return "";
        }

        public static string ShowFileSelectDialog()
        {
            // Create OpenFileDialog 
            var dlg = new OpenFileDialog();
            dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            //dlg.Filter = "*.csv";
            //dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                return dlg.FileName;
            }
            return null;
        }
    }
}
