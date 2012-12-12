using AntaresShell.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace AntaresShell.IO
{
    public class FileStorageAdapter
    {
        private readonly Dictionary<string, SemaphoreSlim> _semaphores = new Dictionary<string, SemaphoreSlim>();

        /// <summary>
        /// Prevents a default instance of the <see cref="FileStorageAdapter"/> class from being created.
        /// </summary>
        private FileStorageAdapter()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        /// <value>
        /// Identifier to the single instance of the class.
        /// </value>
        public static FileStorageAdapter Instance
        {
            get { return Nested._instance; }
        }

        /// <summary>
        /// Lazy implementation of the singleton pattern.
        /// </summary>
        private class Nested
        {
            /// <summary>
            /// Instance of MessageIO for Singleton pattern.
            /// </summary>
            internal static readonly FileStorageAdapter _instance = new FileStorageAdapter();

            /// <summary>
            /// Initializes static members of the Nested class.
            /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit.
            /// </summary>
            static Nested()
            {
            }
        }

        /// <summary>
        /// Gets File located in Local Storage,
        /// Input file name may include folder paths seperated by "\\".
        /// Example: filename = "Documentation\\Tutorial\\US\\ENG\\version.txt"
        /// </summary>
        /// <param name="filename">Name of file with full path.</param>
        /// <param name="rootFolder">Parental folder.</param>
        /// <returns>Target StorageFile.</returns>
        public async Task<StorageFile> GetFileAsync(string filename, StorageFolder rootFolder = null)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }

            var semaphore = GetSemaphore(filename);
            await semaphore.WaitAsync();

            try
            {
                rootFolder = rootFolder ?? AntaresBaseFolder.Instance.RoamingFolder;
                return await rootFolder.GetFileAsync(NormalizePath(filename));
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Creates File located in Local Storage,
        /// Input file name may include folder paths seperated by "\\".
        /// Example: filename = "Documentation\\Tutorial\\US\\ENG\\version.txt"
        /// </summary>
        /// <param name="filename">File name to create (with full path).</param>
        /// <param name="creationCollisionOption">Option to create.</param>
        /// <param name="rootFolder">Parental folder.</param>
        /// <returns>Created file.</returns>
        public async Task<StorageFile> CreateFileAsync(string filename,
                                                       CreationCollisionOption creationCollisionOption = CreationCollisionOption.FailIfExists,
                                                       StorageFolder rootFolder = null)
        {
            if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(Path.GetFileName(filename)))
            {
                return null;
            }

            var semaphore = GetSemaphore(filename);
            await semaphore.WaitAsync();

            try
            {
                rootFolder = rootFolder ?? AntaresBaseFolder.Instance.RoamingFolder;
                return await rootFolder.CreateFileAsync(NormalizePath(filename), creationCollisionOption);
            }
            catch(Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        }

        private SemaphoreSlim GetSemaphore(string filename)
        {
            if (_semaphores.ContainsKey(filename))
            {
                return _semaphores[filename];
            }

            var semaphore = new SemaphoreSlim(1);
            _semaphores[filename] = semaphore;
            return semaphore;
        }

        /// <summary>
        /// Deletes file.
        /// </summary>
        /// <param name="fileName">File name to be deleted.</param>
        /// <param name="rootFolder">Folder stores file.</param>
        public async void DeleteFileAsync(string fileName, StorageFolder rootFolder = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            rootFolder = rootFolder ?? AntaresBaseFolder.Instance.RoamingFolder;
            try
            {
                var deleteFile = await rootFolder.GetFileAsync(NormalizePath(fileName));
                await deleteFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
            }
        }


        /// <summary>
        /// Deletes folder.
        /// </summary>
        /// <param name="folderPath">Folder to be deleted.</param>
        /// <param name="rootFolder">Parental Folder contains to-be-deleted folder.</param>
        /// <returns>TRUE if successful, else FALSE.</returns>
        public async Task<bool> DeleteFolderAsync(string folderPath, StorageFolder rootFolder = null)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return false;
            }

            rootFolder = rootFolder ?? AntaresBaseFolder.Instance.RoamingFolder;
            StorageFolder deleteFolder;
            try
            {
                deleteFolder = await rootFolder.GetFolderAsync(Path.GetDirectoryName(folderPath));
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return false;
            }

            if (deleteFolder != null)
            {
                await deleteFolder.DeleteAsync();
            }

            return true;
        }

        /// <summary>
        /// Changes folder to new name by adding "_" to its name.
        /// </summary>
        /// <param name="fromName">Path to folder.</param>
        /// <param name="toName"> </param>
        /// <param name="rootFolder"> </param>
        /// <param name="option"> </param>
        /// <returns>Name-changed folder.</returns>
        public async Task<StorageFolder> ChangeFolderName(string fromName,
                                                          string toName,
                                                          StorageFolder rootFolder = null,
                                                          NameCollisionOption option = NameCollisionOption.ReplaceExisting)
        {
            if (string.IsNullOrEmpty(fromName))
            {
                return null;
            }

            fromName = NormalizePath(fromName);

            var changeFolder = rootFolder ?? ApplicationData.Current.LocalFolder;
            try
            {
                changeFolder = await changeFolder.GetFolderAsync(fromName);
            }
            catch (Exception)
            {
                changeFolder = null;
            }

            if (changeFolder != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(toName) || fromName.ToLowerInvariant().EndsWith(toName.ToLowerInvariant()))
                    {
                        return changeFolder;
                    }

                    await changeFolder.RenameAsync(toName, option);
                }
                catch (Exception ex)
                {
                    LogManager.Instance.LogInfo("Cannot change folder name " + fromName + "!" + ex);
                    changeFolder = null;
                }
            }

            return changeFolder;
        }

        /// <summary>
        /// Create a folder in the current rootFolder.
        /// </summary>
        /// <param name="folderName">Name of the folder to be created. This does not have to be an immediate sub-folder of the current folder.</param>
        /// <param name="rootFolder">The current folder.</param>
        /// <returns>None.</returns>
        public async Task<StorageFolder> CreateFolder(string folderName, StorageFolder rootFolder)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                return rootFolder;
            }

            rootFolder = rootFolder ?? AntaresBaseFolder.Instance.RoamingFolder;
            folderName = NormalizePath(folderName);

            var startIndex = folderName.IndexOf('\\');

            if (startIndex == -1)
            {
                return await rootFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            }

            rootFolder = await rootFolder.CreateFolderAsync(folderName.Substring(0, startIndex), CreationCollisionOption.OpenIfExists);
            folderName = folderName.Substring(startIndex + 1);
            return await CreateFolder(folderName, rootFolder);
        }

        /// <summary>
        /// Create a file the current folder provided the file relative path. If file exists, it will be replaced.
        /// </summary>
        /// <param name="fileName">Name of the file in the folder, the file can reside in a sub folder of the current folder.</param>
        /// <param name="rootFolder">The current folder.</param>
        /// <returns>A StorageFile of the newly created file.</returns>
        public async Task<StorageFile> CreateFile(string fileName, StorageFolder rootFolder)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            fileName = NormalizePath(fileName);
            rootFolder = await CreateFolder(Path.GetDirectoryName(fileName), rootFolder);
            return await rootFolder.CreateFileAsync(Path.GetFileName(fileName), CreationCollisionOption.ReplaceExisting);
        }

        /// <summary>
        /// Returns standard string represented a C# path with or without file name.
        /// </summary>
        /// <param name="path">Unnormalized path.</param>
        /// <returns>Normalized path.</returns>
        public string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (string.IsNullOrEmpty(Path.GetDirectoryName(path)))
            {
                return string.IsNullOrEmpty(Path.GetFileName(path)) ? null : Path.GetFileName(path);
            }

            return string.IsNullOrEmpty(Path.GetFileName(path)) ? Path.GetDirectoryName(path + "\\") :
                    (new StringBuilder(Path.GetDirectoryName(path))).Append("\\").Append(Path.GetFileName(path)).ToString();
        }
    }
}
