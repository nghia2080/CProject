using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;
using System.IO;
using Windows.Storage.Streams;

//using System.Runtime.Serialization;

namespace eSECSync.Clients
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

        public async Task<IBuffer> SampleProtectAsync(String strMsg, String strDescriptor, BinaryStringEncoding encoding)
        {
            // Create a DataProtectionProvider object for the specified descriptor.
            DataProtectionProvider Provider = new DataProtectionProvider(strDescriptor);

            // Encode the plaintext input message to a buffer.
            encoding = BinaryStringEncoding.Utf8;
            IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(strMsg, encoding);

            // Encrypt the message.
            IBuffer buffProtected = await Provider.ProtectAsync(buffMsg);

            // Execution of the SampleProtectAsync function resumes here
            // after the awaited task (Provider.ProtectAsync) completes.

            return buffProtected;
        }

        public async Task SaveAsync<T>(T data)
        {
            try
            {
                if (_sessionFile == null)
                {
                    _sessionFile = await EnsureBaseFolder.Instance.PersonalFolder.CreateFileAsync(_fileName,
                                                                                CreationCollisionOption.ReplaceExisting);
                }
                var sessionRandomAccess = await _sessionFile.OpenAsync(FileAccessMode.ReadWrite);
                var sessionOutputStream = sessionRandomAccess.GetOutputStreamAt(0);
                var sessionSerializer = new DataContractSerializer(typeof(List<object>), new[] { typeof(T) });
                sessionSerializer.WriteObject(sessionOutputStream.AsStreamForWrite(), data);
                await sessionOutputStream.FlushAsync();

                sessionOutputStream.Dispose();
                sessionRandomAccess.Dispose();
            }
            catch
            {
                //return null;
            }
        }

        public async Task<T> RestoreAsync<T>()
        {
            try
            {
                if (_sessionFile == null)
                {
                    _sessionFile = await EnsureBaseFolder.Instance.PersonalFolder.GetFileAsync(_fileName);

                    if (_sessionFile == null)
                    {
                        return default(T);
                    }
                }
                var sessionInputStream = await _sessionFile.OpenReadAsync();
                var sessionSerializer = new DataContractSerializer(typeof(List<object>), new[] { typeof(T) });

                var result = (T)sessionSerializer.ReadObject(sessionInputStream.AsStreamForRead());

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
