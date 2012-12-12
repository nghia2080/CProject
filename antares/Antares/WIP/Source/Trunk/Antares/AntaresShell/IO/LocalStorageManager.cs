using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;
using System.IO;
using Windows.Storage.Streams;
using System.Threading;

//using System.Runtime.Serialization;

namespace AntaresShell.IO
{
    public class LocalStorageManager
    {
        private readonly string _fileName;

        public string Description { get; set; }

        private StorageFile _sessionFile;

        public LocalStorageManager()
            : this(Guid.NewGuid() + ".xml")
        {
        }

        public LocalStorageManager(string fileName)
            : this(fileName, null)
        {
        }

        public LocalStorageManager(string fileName, string description)
        {
            _fileName = fileName;
            Description = description;
        }

        SemaphoreSlim _sl = new SemaphoreSlim(1);

        public async Task SaveAsync<T>(T data)
        {
            await _sl.WaitAsync();

            try
            {
                if (_sessionFile == null)
                {
                    _sessionFile = await AntaresBaseFolder.Instance.RoamingFolder.CreateFileAsync(_fileName,
                                                                                CreationCollisionOption.ReplaceExisting);
                }

                var sessionRandomAccess = await _sessionFile.OpenAsync(FileAccessMode.ReadWrite);
                var sessionOutputStream = sessionRandomAccess.GetOutputStreamAt(0);
                var sessionSerializer = new DataContractSerializer(typeof(List<object>), new[] { typeof(T) });
                sessionSerializer.WriteObject(sessionOutputStream.AsStreamForWrite(), data);
                await sessionOutputStream.FlushAsync();

                _sessionFile = null;
                sessionOutputStream.Dispose();
                sessionRandomAccess.Dispose();
            }
            catch
            {
                //return null;
            }
            finally
            {
                _sl.Release();
            }
        }

        public async Task<T> RestoreAsync<T>()
        {
            try
            {
                if (_sessionFile == null)
                {
                    _sessionFile = await AntaresBaseFolder.Instance.RoamingFolder.GetFileAsync(_fileName);

                    if (_sessionFile == null)
                    {
                        return default(T);
                    }
                }
                var sessionInputStream = await _sessionFile.OpenReadAsync();
                var sessionSerializer = new DataContractSerializer(typeof(List<object>), new[] { typeof(T) });

                var result = (T)sessionSerializer.ReadObject(sessionInputStream.AsStreamForRead());

                _sessionFile = null;
                sessionInputStream.Dispose();
                return result;
            }
            catch
            {
                return default(T);
            }
        }
    }
}
