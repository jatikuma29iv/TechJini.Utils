using System;
using System.Collections.Generic;
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
        /// <returns>
        /// Path of the moved file.
        /// </returns>
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
        /// <returns>
        /// Path of the moved file.
        /// </returns>
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
        /// <returns>
        /// Path of the copied file.
        /// </returns>
        public static string CopyFileToPathMakingUniqueName(string fileName, string filePath, string fileCopyPath)
        {
            string dummyPath = Path.Combine(string.Empty, fileName);
            string uniqueFileName = Path.GetFileNameWithoutExtension(dummyPath) + "_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(dummyPath);

            string copiedFilePath = Path.Combine(fileCopyPath, uniqueFileName);
            File.Copy(filePath, copiedFilePath);

            return copiedFilePath;
        }

        /// <summary>
        /// Validates the type of the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileExtensionsLst">The lis of file extensions to validate against.</param>
        /// <returns>
        ///   <c>true</c> if file type exists in the given extensions; otherwise, <c>false</c>.
        /// </returns>
        public static bool ValidateFileType(string fileName, List<string> fileExtensionsLst)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string filePath = Path.Combine(string.Empty, fileName);
                string extension = Path.GetExtension(filePath);
                if (!string.IsNullOrEmpty(extension) && fileExtensionsLst.Contains(extension.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Deletes the specified files.
        /// </summary>
        /// <param name="FileInfos">The file infos of the files to be deleted.</param>
        /// <returns>
        ///   <c>true</c> if all the files are deleted successfully; otherwise, <c>false</c>.
        /// </returns>
        public static bool DeleteFiles(List<FileInfo> FileInfos)
        {
            if (FileInfos != null && FileInfos.Count > 0)
            {
                foreach (FileInfo fileInfo in FileInfos)
                {
                    try
                    {
                        fileInfo.Delete();
                    }
                    catch { }
                }

                foreach (FileInfo fileInfo in FileInfos)
                {
                    if (fileInfo.Exists)
                        return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Renames the files to given names.
        /// </summary>
        /// <param name="SourceFileInfo">The list containing the metadata of the source files.</param>
        /// <param name="fileNames">The list containing the new file names.</param>
        /// <param name="options">The options for handling the name clash while renaming the files.</param>
        /// <returns>
        /// A list containing the metadata for the renamed files
        /// </returns>
        public static List<FileInfo> RenameFiles(List<FileInfo> SourceFileInfo, List<string> fileNames, NameClashOptions options = NameClashOptions.DoNothing)
        {
            List<FileInfo> MovedFileInfos = new List<FileInfo>();
            if (SourceFileInfo != null && fileNames != null && SourceFileInfo.Count == fileNames.Count)
            {
                foreach (FileInfo fileInfo in SourceFileInfo)
                {
                    int index = SourceFileInfo.IndexOf(fileInfo);
                    if (!string.IsNullOrWhiteSpace(fileNames[index]))
                    {
                        string destinationFilePath = fileInfo.FullName.Replace(fileInfo.Name, fileNames[index]);

                        if (options == NameClashOptions.ReplaceExisting)
                        {
                            MovedFileInfos.Add(fileInfo.CopyTo(destinationFilePath, true));
                        }
                        else if (options == NameClashOptions.RenameUniquely)
                        {
                            string extension = Path.GetExtension(destinationFilePath);
                            do
                            {
                                string uniquePath = destinationFilePath.Insert(destinationFilePath.Length - extension.Length, "_" + DateTime.Now.Ticks + extension);
                                if (!File.Exists(uniquePath))
                                {
                                    destinationFilePath = uniquePath;
                                    MovedFileInfos.Add(fileInfo.CopyTo(destinationFilePath));
                                    break;
                                }
                            } while (destinationFilePath.EndsWith(fileNames[index]));
                        }
                        else
                        {
                            try
                            {
                                MovedFileInfos.Add(fileInfo.CopyTo(destinationFilePath, false));
                            }
                            catch
                            {
                                MovedFileInfos.Add(fileInfo);
                            }
                        }

                        // Check if the renamed file is not same as the original file then delete the original file
                        if (!string.Equals(MovedFileInfos[index].FullName, fileInfo.FullName))
                            fileInfo.Delete();
                    }
                }
            }

            return MovedFileInfos;
        }
    }

    public enum NameClashOptions
    {
        DoNothing = 0,
        ReplaceExisting = 1,
        RenameUniquely = 2
    }
}
