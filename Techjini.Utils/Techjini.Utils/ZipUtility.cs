using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Ionic.Zip;

namespace Techjini.Utils
{
    public static class ZipUtility
    {
        /// <summary>
        /// Creates the zip archive using files infos.
        /// </summary>
        /// <param name="archiveName">Name of the archive.</param>
        /// <param name="filePaths">The file paths.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static string CreateZipArchiveWithPassword(string archiveName, List<string> filePaths, string password)
        {
            try
            {
                string rootPath = HttpContext.Current.Server.MapPath("~");
                string destinationFolderPath = Path.Combine(rootPath, "App_Data", DateTime.Now.Ticks.ToString());
                Directory.CreateDirectory(destinationFolderPath);

                string zipFilePath = Path.Combine(destinationFolderPath, archiveName + ".zip");

                using (var zip = new ZipFile(zipFilePath))
                {
                    foreach (var path in filePaths)
                    {
                        zip.Password = password;
                        zip.Encryption = EncryptionAlgorithm.PkzipWeak;
                        if (zip.ContainsEntry(Path.GetFileName(path)))
                        {
                            FileInfo fileInfo = new FileInfo(path);
                            string newFileName = string.Join("_", Path.GetFileNameWithoutExtension(path), DateTime.Now.Ticks.ToString()) + Path.GetExtension(path);
                            string newFilePath = path.Replace(Path.GetFileName(path), newFileName);
                            fileInfo.MoveTo(newFilePath);

                            zip.AddFile(newFilePath, string.Empty);
                        }
                        else
                        {
                            zip.AddFile(path, string.Empty);
                        }
                    }
                    zip.Save();
                }

                return zipFilePath;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        /// <summary>
        /// Creates the zip archive using files infos.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static string CreateZipArchiveWithPassword(string filePath, string password)
        {
            try
            {
                List<string> filePaths = new List<string> { filePath };
                return CreateZipArchiveWithPassword(Path.GetFileNameWithoutExtension(filePath), filePaths, password);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        /// <summary>
        /// Password protects the zip archive.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static bool PasswordProtectZipArchive(string filePath, string password)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            try
            {
                string extractedFilePath = Path.GetFileNameWithoutExtension(filePath);

                using (var zipFile = ZipFile.Read(filePath))
                {
                    // Extract the files
                    zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                    zipFile.ExtractAll(extractedFilePath);
                }

                // Delete original zip
                File.Delete(filePath);

                // create new zip at same path with same name
                using (var zip = new ZipFile(filePath))
                {
                    zip.Password = password;
                    zip.Encryption = EncryptionAlgorithm.PkzipWeak;
                    zip.AddDirectoryWillTraverseReparsePoints = true;
                    zip.AddDirectory(extractedFilePath);

                    zip.Save();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }

            return true;
        }
    }
}
