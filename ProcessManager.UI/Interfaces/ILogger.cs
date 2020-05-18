namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public interface ILogger
    {

		#region Events

		/// <summary>
		/// An action that will be invoked when a new log is logged 
		/// </summary>
		public event Action<string, LogLevel> NewLog;

		#endregion


		#region Properties

		/// <summary>
		/// The logger's verboseness level 
		/// </summary>
		public LogLevel LogOutputLevel { get; set; }

		#endregion


		#region Methods

		/// <summary>
		/// Logs a message
		/// </summary>
		/// <param name="logMessage"> The message to log </param>
		/// <param name="logLevel"> The log's output severity </param>
		/// <param name="callerOrigin"> Where the logger was called from </param>
		/// <param name="filePath"> The path where the log came from </param>
		/// <param name="lineNumber"> The line number where the log was called </param>
		public void Log(string logMessage, LogLevel logLevel = LogLevel.Normal, [CallerMemberName]string callerOrigin = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0);

		#endregion

	};
};