using System.Collections.Generic;

namespace AntaresShell.Logger
{
    internal class LogSettings
    {
        private double _maximumFileSize;

        /// <summary>
        /// File name of log file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Yes old log file when launch.
        /// </summary>
        public bool ResetWhenLaunch { get; set; }

        /// <summary>
        /// Maximum file size on bytes.
        /// </summary>
        public double MaximumFileSize 
        {
            get { return _maximumFileSize; }
            set { _maximumFileSize = value * 1024 * 1024; }
        }

        /// <summary>
        /// Level to log.
        /// </summary>
        public List<LogLevel> LogLevels { get; set; }
    }
}
