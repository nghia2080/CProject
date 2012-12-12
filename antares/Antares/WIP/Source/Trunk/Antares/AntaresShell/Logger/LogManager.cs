using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AntaresShell.IO;
using AntaresShell.Utilities;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace AntaresShell.Logger
{
    public class LogManager
    {
        private const double EPSILON = 1e-9;

        /// <summary>
        /// Represents FileName node.
        /// </summary>
        private const string FILENAME_NODE = "//VAIOCareLogger//Settings//FileName";

        /// <summary>
        /// Represents FileName node.
        /// </summary>
        private const string RESET_NODE = "//VAIOCareLogger//Settings//ResetWhenLaunch";

        /// <summary>
        /// Represents file size node.
        /// </summary>
        private const string FILESIZE_NODE = "//VAIOCareLogger//Settings//MaximumFileSize";

        /// <summary>
        /// Represents log level node.
        /// </summary>
        private const string LOGLEVEL_NODE = "//VAIOCareLogger//Settings//LogLevel";

        private LogSettings _logSettings;
        private LoggerIO _loggerIO;

        public async Task ReadSettingsAsync(string filePath, StorageFolder rootFolder)
        {
            try
            {
                var content = await IOStream.Instance.ReadFromFileAsync(filePath, rootFolder);
                if (content == null)
                {
                    return;
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadFromString(content);

                _logSettings = new LogSettings();

                var fileNameNode = xmlDoc.SelectSingleNode(FILENAME_NODE);
                if (fileNameNode != null)
                {
                    var namedItem = fileNameNode.Attributes.GetNamedItem("value");
                    if (namedItem != null)
                        _logSettings.FileName = namedItem.NodeValue.ToString();
                }

                if (string.IsNullOrEmpty(_logSettings.FileName))
                {
                    return;
                }

                var resetNode = xmlDoc.SelectSingleNode(RESET_NODE);
                var reset = false;
                if (resetNode != null)
                {
                    var xmlNode = resetNode.Attributes.GetNamedItem("value");
                    if (xmlNode != null)
                        bool.TryParse(xmlNode.NodeValue.ToString(), out reset);
                }
                _logSettings.ResetWhenLaunch = reset;

                if (_logSettings.ResetWhenLaunch)
                {
                    FileStorageAdapter.Instance.DeleteFileAsync(_logSettings.FileName, AntaresBaseFolder.Instance.RoamingFolder);
                }

                var maxFileSizeNode = xmlDoc.SelectSingleNode(FILESIZE_NODE);
                double maxSize = 0.0;
                if (maxFileSizeNode != null)
                {
                    var item = maxFileSizeNode.Attributes.GetNamedItem("value");
                    if (item != null)
                        double.TryParse(item.NodeValue.ToString(), out maxSize);
                }
                _logSettings.MaximumFileSize = maxSize;

                if (Math.Abs(_logSettings.MaximumFileSize - 0) < EPSILON)
                {
                    return;
                }

                var logLevel = xmlDoc.SelectSingleNode(LOGLEVEL_NODE);
                if (logLevel != null)
                {
                    var namedItem = logLevel.Attributes.GetNamedItem("value");
                    if (namedItem != null)
                    {
                        var log = namedItem.NodeValue.ToString();

                        if (string.IsNullOrEmpty(log))
                        {
                            return;
                        }

                        var logLevels = log.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                        var tempList = new List<LogLevel>();
                        foreach (var level in logLevels)
                        {
                            LogLevel lvl;

                            Enum.TryParse(level, out lvl);

                            tempList.Add(lvl);
                        }

                        _logSettings.LogLevels = tempList;
                    }
                }

                if (_logSettings.LogLevels.Count == 0)
                {
                    return;
                }

                _loggerIO = new LoggerIO(_logSettings.FileName, _logSettings.MaximumFileSize);
            }
            catch
            {
                // Any exception when load setting, return null and dont log any to file.
                _logSettings = null;
                _loggerIO = null;
            }
        }

        public void LogInfo(string content)
        {
            if (_logSettings == null || _loggerIO == null || !_logSettings.LogLevels.Contains(LogLevel.INFO))
            {
                return;
            }

            _loggerIO.SetMessage(content);
        }

        public void LogException(string content)
        {
            if (_logSettings == null || _loggerIO == null || !_logSettings.LogLevels.Contains(LogLevel.ERROR))
            {
                return;
            }

            _loggerIO.SetMessage(content);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="LogManager"/> class from being created.
        /// </summary>
        private LogManager() { }

        /// <summary>
        /// Lazy implementation of the singleton pattern.
        /// </summary>
        private class Nested
        {
            /// <summary>
            /// Initializes static members of the Nested class. 
            /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit.
            /// </summary>
            static Nested()
            {
            }

            /// <summary>
            /// Instance of VAIOCareLogger for Singleton pattern.
            /// </summary>
            internal static readonly LogManager _instance = new LogManager();
        }

        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        /// <value>
        /// Identifier to the single instance of the class.
        /// </value>
        public static LogManager Instance
        {
            get { return Nested._instance; }
        }
    }
}
