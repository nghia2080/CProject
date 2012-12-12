using AntaresShell.IO;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;
using Windows.Storage.Streams;

namespace AntaresShell.Logger
{
	/// <summary>
    /// Provide a class which save message to hard driver.
    /// </summary>
    internal class LoggerIO
    {
        /// <summary>
        /// Queue message contains all message which will be written to hard driver.
        /// </summary>
        private readonly Queue<string> _queueMessages;

        /// <summary>
        /// Lock object.
        /// </summary>
        private readonly object _creationLock = new object();

        private readonly string _fileName;
        private readonly double _maxFileSize;

        public LoggerIO(string fileName, double fileSize)
        {
            _fileName = fileName;
            _maxFileSize = fileSize;
            _queueMessages = new Queue<string>();
        }

        /// <summary>
        /// Write message to hard driver.
        /// </summary>
        /// <param name="message">Content to write.</param>
        public void SetMessage(string message)
        {
            if (message == null)
            {
                return;
            }

            lock (_creationLock)
            {
                _queueMessages.Enqueue(message);

                if (_queueMessages.Count == 1)
                {
                    WriteDataHelperAsync();
                }
            }
        }

        /// <summary>
        /// Support class to write content to local,
        /// it will seek to message on peek of queue and recrusive till queue has no element.
        /// </summary>
        private async void WriteDataHelperAsync()
        {
            try
            {
                var message = _queueMessages.Peek();

                var readfile =
                    await
                    FileStorageAdapter.Instance.CreateFileAsync(
                        _fileName, CreationCollisionOption.OpenIfExists);

                var datastream = await readfile.OpenAsync(FileAccessMode.ReadWrite);
                var inputStream = datastream.GetInputStreamAt(0);
                var dataReader = new DataReader(inputStream);
                var numBytesLoaded = await dataReader.LoadAsync((uint)datastream.Size);

                // Yes big file.
                if (_maxFileSize < numBytesLoaded)
                {
                    datastream.Dispose();
                    inputStream.Dispose();

                    readfile = await
                    FileStorageAdapter.Instance.CreateFileAsync(
                        _fileName, CreationCollisionOption.ReplaceExisting);

                    datastream = await readfile.OpenAsync(FileAccessMode.ReadWrite);
                    inputStream = datastream.GetInputStreamAt(0);
                    dataReader = new DataReader(inputStream);
                    numBytesLoaded = await dataReader.LoadAsync((uint)datastream.Size);
                }

                var contentData = dataReader.ReadString(numBytesLoaded);

                var outputStream = datastream.GetOutputStreamAt((uint)datastream.Size);
                var dataWriter = new DataWriter(outputStream);

                dataWriter.WriteString(
                    !string.IsNullOrEmpty(contentData)
                        ? new StringBuilder("\r\n\r\n").Append(DateTime.Now).Append("\r\n").Append(message).ToString()
                        : message);

                datastream.Dispose();
                await dataWriter.StoreAsync();
                inputStream.Dispose();
                await outputStream.FlushAsync();
                outputStream.Dispose();
                lock (_creationLock)
                {
                    _queueMessages.Dequeue();
                    if (_queueMessages.Count != 0)
                    {
                        WriteDataHelperAsync();
                    }
                }
            }
            catch 
            {
                // When user open log file while, Authorized exception will be fire but we skip to handle this case.
                return;
            }
        }
    }
}