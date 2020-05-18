namespace ProcessManager.UI
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;


    /// <summary>
    /// An implementation of <see cref="ILoggerBase"/> that outputs log to a console
    /// </summary>
    public class ConsoleLogger : ILogger
    {

        private object _synchronizatingObject = new object();


        /// <summary>
        /// The logger's verboseness level 
        /// </summary>
        public LogLevel LogOutputLevel { get; set; }


        /// <summary>
        /// An action that will be invoked when a new log is logged 
        /// </summary>
        public event Action<string, LogLevel> NewLog = (message, outputLever) => { };


        /// <summary>
        /// Logs the message to console
        /// </summary>
        /// <param name="logMessage"> The message to log </param>
        /// <param name="logLevel"> The log's output severity </param>
        /// <param name="callerOrigin"> Where the logger was called from </param>
        /// <param name="filePath"> The path where the log came from </param>
        /// <param name="lineNumber"> The line number where the log was called </param>
        public void Log(string logMessage, LogLevel logLevel = LogLevel.Info, [CallerMemberName] string callerOrigin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            lock (_synchronizatingObject)
            {
                // Writes the LogLevel in a format I like
                var  writeLogLevel = new Action<LogLevel, ConsoleColor>((logLevel, colour) =>
                {
                    // Write first character
                    Console.Write('[');

                    // Set colour and write the log level
                    Console.ForegroundColor = colour;
                    Console.Write($"{logLevel}");

                    // Reset console colours and add the last bracket
                    Console.ResetColor();
                    Console.WriteLine(']');
                });


                // Setup message format
                switch (logLevel)
                {
                    case LogLevel.Info:
                    {
                        writeLogLevel(logLevel, ConsoleColor.Gray);
                        break;
                    };

                    case LogLevel.Warning:
                    {
                        writeLogLevel(logLevel, ConsoleColor.Yellow);
                        break;
                    };

                    case LogLevel.Error:
                    {
                        writeLogLevel(logLevel, ConsoleColor.Red);
                        break;
                    };

                    case LogLevel.Critical:
                    {
                        writeLogLevel(logLevel, ConsoleColor.DarkRed);
                        break;
                    };


                    default:
                    {
                        writeLogLevel(logLevel, ConsoleColor.Gray);
                        break;
                    };
                };


                // Setup log message format
                string message = $"[{Path.GetFileName(filePath)} > {callerOrigin}() > Line: {lineNumber}]" +
                    $"{Environment.NewLine}{logMessage}{Environment.NewLine}";

                // Log to the console
                Console.WriteLine(message);

                // Invoke event
                NewLog?.Invoke(logMessage, logLevel);
            };
        }
    };
};