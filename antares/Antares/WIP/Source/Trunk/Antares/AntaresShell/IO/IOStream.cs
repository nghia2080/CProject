//-----------------------------------------------------------------------
// <copyright file="IOStream.cs" company="Sony Corporation">
//     Copyright by Sony Corporation.
// </copyright>
// <summary>Provides the api to read/write from file.</summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AntaresShell.Logger;
using Windows.Storage;
using Windows.Storage.Streams;

namespace AntaresShell.IO
{
    /// <summary>
    /// Provides the api to read/write from file.
    /// </summary>
    public class IOStream
    {
        private readonly Dictionary<string, SemaphoreSlim> _semaphores = new Dictionary<string, SemaphoreSlim>();

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

        private string NormalizePath(string oldPath)
        {
            oldPath = oldPath.Replace("/", @"\");
            while (oldPath.Contains("\\\\"))
            {
                oldPath = oldPath.Replace("\\\\", "\\");
            }

            while (oldPath.StartsWith(@"\"))
            {
                oldPath = oldPath.Substring(1, oldPath.Length - 1);
            }

            return oldPath;
        }

        #region Read file

        /// <summary>
        /// Read string content from file.
        /// </summary>
        /// <param name="path">Location of file, separate by //.</param>
        /// <param name="rootFolder"> </param>
        /// <returns>Content of file.</returns>
        public async Task<string> ReadFromFileAsync(string path, StorageFolder rootFolder = null)
        {
            if (path == null)
            {
                return null;
            }

            try
            {
                var file = await GetFileToReadAsync(path, rootFolder);

                if (file == null)
                {
                    return null;
                }

                var readStream = await file.OpenAsync(FileAccessMode.Read);

                var inputStream = readStream.GetInputStreamAt(0);
                var dataReader = new DataReader(inputStream);

                var numBytesLoaded = await dataReader.LoadAsync((uint)readStream.Size);
                var content = dataReader.ReadString(numBytesLoaded);
                dataReader.DetachStream();
                dataReader.Dispose();
                inputStream.Dispose();
                readStream.Dispose();

                return content;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get storage file.
        /// </summary>
        /// <param name="path">Location of file, separate by //.</param>
        /// <param name="rootFolder"> </param>
        /// <returns>A file that get from path.</returns>
        public async Task<StorageFile> GetFileToReadAsync(string path, StorageFolder rootFolder)
        {
            if (path == null)
            {
                return null;
            }

            if (rootFolder == null)
            {
                return await GetFileToReadAsync(path);
            }

            var semaphore = GetSemaphore(path);
            
            await semaphore.WaitAsync();

            try
            {
                return await rootFolder.GetFileAsync(NormalizePath(path));
            }
            catch
            {
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Get file base on defined logic as below:
        /// Here are the steps:
        /// 1. Check the exist of parent folder ? 
        /// 2. If parent folder doesnt not exit: Check in installed folder.
        /// 3. Otherwise, check in roaming folder.
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>Expected file.</returns>
        private async Task<StorageFile> GetFileToReadAsync(string path)
        {
            // If the parent folder of file doesnt not exist.
            if (!await CheckFolderExistenceAsync(path))
            {
                // Get file in installed folder.
                return await GetFileToReadAsync(path, AntaresBaseFolder.Instance.InstalledFolder);
            }
            
            // Get file in roaming folder.
            return await GetFileToReadAsync(path, AntaresBaseFolder.Instance.RoamingFolder);
        }

        /// <summary>
        /// Check exitance of parent folder of this file.
        /// </summary>
        /// <param name="path">Path of file.</param>
        /// <returns>True if exist.</returns>
        public async Task<bool> CheckFolderExistenceAsync(string path)
        {
            if (path == null)
            {
                return false;
            }

            var leafFolder = AntaresBaseFolder.Instance.RoamingFolder;

            var lashSplash = path.LastIndexOf("/", StringComparison.Ordinal);

            if (lashSplash <= 0)
            {
                lashSplash = path.LastIndexOf(@"\", StringComparison.Ordinal);
            }

            path = path.Substring(0, lashSplash);

            try
            {
                await leafFolder.GetFolderAsync(path);
                return true;
            }
            catch (Exception exception)
            {
                LogManager.Instance.LogException(exception.ToString());
                return false;
            }
        }
        #endregion

        #region Write file

        /// <summary>
        /// Write a string content to file.
        /// </summary>
        /// <param name="content">Content want to write.</param>
        /// <param name="path">Location of file, separate by //.</param>
        /// <param name="rootFolder"> </param>
        /// <returns>True if success and otherwise.</returns>
        public async Task<bool> WriteFileContentAsync(string content, string path, StorageFolder rootFolder = null)
        {
            if (path == null || content == null)
            {
                return false;
            }

            if (rootFolder == null)
            {
                rootFolder = AntaresBaseFolder.Instance.RoamingFolder;
            }

            try
            {
                var cacheFile = await GetFileToWriteAsync(path, rootFolder);

                var writeStream = await cacheFile.OpenAsync(FileAccessMode.ReadWrite);
                var outputStream = writeStream.GetOutputStreamAt(0);
                var dataWriter = new DataWriter(outputStream);

                dataWriter.WriteString(content);

                await dataWriter.StoreAsync();
                await outputStream.FlushAsync();
                writeStream.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get storage file.
        /// </summary>
        /// <param name="path">Location of file, separate by //.</param>
        /// <param name="rootFolder"> </param>
        /// <returns>A file that get from path.</returns>
        public async Task<StorageFile> GetFileToWriteAsync(string path, StorageFolder rootFolder)
        {
            if (path == null || rootFolder == null)
            {
                return null;
            }

            var semaphore = GetSemaphore(path);
            await semaphore.WaitAsync();

            try
            {
                return await rootFolder.CreateFileAsync(NormalizePath(path), CreationCollisionOption.ReplaceExisting);
            }
            catch
            {
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        }
        #endregion

        #region Singleton pattern
        /// <summary>
        /// Prevents a default instance of the <see cref="IOStream"/> class from being created.
        /// </summary>
        private IOStream()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        /// <value>
        /// Identifier to the single instance of the class.
        /// </value>
        public static IOStream Instance
        {
            get { return Nested._instance; }
        }

        /// <summary>
        /// Lazy implementation of the singleton pattern.
        /// </summary>
        private class Nested
        {
            /// <summary>
            /// Instance of IOStream for Singleton pattern.
            /// </summary>
            internal static readonly IOStream _instance = new IOStream();

            /// <summary>
            /// Initializes static members of the Nested class.
            /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit.
            /// </summary>
            static Nested()
            {
            }
        }
        #endregion
    }
}