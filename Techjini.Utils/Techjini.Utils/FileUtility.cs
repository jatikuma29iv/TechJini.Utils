using System;
using System.IO;
using System.Web;

namespace Techjini.Utils
{
    public static class FileUtility
    {
        /// <summary>
        /// Moves the file to application data.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static string MoveFileToAppData(string filePath)
        {
            try
            {

                if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    string rootPath = HttpContext.Current.Server.MapPath("~");
                    string destinationPath = Path.Combine(rootPath, "App_Data", DateTime.Now.Ticks.ToString());
                    if (!Directory.Exists(destinationPath))
                        Directory.CreateDirectory(destinationPath);

                    string fileName = Path.GetFileName(filePath);
                    destinationPath = Path.Combine(destinationPath, fileName);

                    //added code to remove additional .txt that we get because of documentum download - Start
                    if (fileName.Split('.').Length > 2 && fileName.ToLower().EndsWith(".txt"))
                    {
                        //destinationPath = destinationPath.TrimEnd(".txt".ToCharArray());
                        destinationPath = destinationPath.Substring(0, destinationPath.Length - 4);
                    }
                    //added code to remove additional .txt that we get because of documentum download - End


                    if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }

                    File.Move(filePath, destinationPath);
                    filePath = destinationPath;

                    // This logic is moved to ConvertToServerPath function
                    //filePath = filePath.Replace('\\', '/');  
                    //filePath = filePath.Replace(rootPath.Replace("\\", "/"), Utils.serverPath).Replace('\\', '/');
                }
                else
                {
                    filePath = null;
                }
            }
            catch (Exception ex)
            {
                filePath = ex.StackTrace;
                Console.WriteLine(ex.ToString());
            }

            return filePath;
        }

        /// <summary>
        /// Moves the file to path making unique name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileMovePath">The file move path.</param>
        /// <returns></returns>
        public static string MoveFileToPathMakingUniqueName(string fileName, string filePath, string fileMovePath)
        {
            string dummyPath = Path.Combine(string.Empty, fileName);
            string uniqueFileName = Path.GetFileNameWithoutExtension(dummyPath) + "_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(dummyPath);

            string movedFilePath = Path.Combine(fileMovePath, uniqueFileName);
            File.Move(filePath, movedFilePath);

            return movedFilePath;
        }

        /// <summary>
        /// Copies the file to path making unique name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileCopyPath">The file copy path.</param>
        /// <returns></returns>
        public static string CopyFileToPathMakingUniqueName(string fileName, string filePath, string fileCopyPath)
        {
            string dummyPath = Path.Combine(string.Empty, fileName);
            string uniqueFileName = Path.GetFileNameWithoutExtension(dummyPath) + "_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(dummyPath);

            string copiedFilePath = Path.Combine(fileCopyPath, uniqueFileName);
            File.Copy(filePath, copiedFilePath);

            return copiedFilePath;
        }
    }
}
