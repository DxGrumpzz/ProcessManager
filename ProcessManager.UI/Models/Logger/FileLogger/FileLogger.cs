﻿namespace ProcessManager.UI
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A logger that is responsible for logging info to a file
    /// </summary>
    public class FileLogger : ILogger
    {
        private object _synchronizingObject = new object();

        private string _logFilePath;

        /// <summary>
        /// The logger's verboseness level 
        /// </summary>
        public LogLevel LogOutputLevel { get; set; }


        /// <summary>
        /// An action that will be invoked when a new log is logged 
        /// </summary>
        public event Action<string, LogLevel> NewLog = (message, outputLever) => { };


        /// <summary>
        /// Default constructor, takes a file path and writes the log there
        /// </summary>
        /// <param name="logFilePath"></param>
        public FileLogger(string logFilePath = "\\Log\\Log.txt")
        {
            if (string.IsNullOrWhiteSpace(logFilePath))
                throw new ArgumentNullException("Log path can't be empty");

            _logFilePath = logFilePath;

            // Create new file for logging
            File.Create(Path.Combine(logFilePath, logFilePath))
                // Dispose file stream after creation
                .Dispose();
        }


        /// <summary>
        /// Logs the message to a file
        /// </summary>
        /// <param name="logMessage"> The message to log </param>
        /// <param name="logLevel"> The log's output severity </param>
        /// <param name="callerOrigin"> Where the logger was called from </param>
        /// <param name="filePath"> The path where the log came from </param>
        /// <param name="lineNumber"> The line number where the log was called </param>
        public void Log(string logMessage, LogLevel logLevel = LogLevel.Info, [CallerMemberName] string callerOrigin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            lock (_synchronizingObject)
            {
                // Setup log message format
                string message =
                    $"[{logLevel}]{Environment.NewLine}" +
                    $"[{Path.GetFileName(filePath)} > {callerOrigin}() > Line: {lineNumber}]" +
                    $"{Environment.NewLine}{logMessage}{Environment.NewLine}";

                // Write log message to log file
                File.AppendAllText(_logFilePath, message);

                // Invoke log event
                NewLog?.Invoke(logMessage, logLevel);
            };
        }
    };
};