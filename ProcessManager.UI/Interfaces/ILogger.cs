namespace ProcessManager.UI
{
    using System;
    using System.Runtime.CompilerServices;

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


		#region Methods

		/// <summary>
		/// Logs a message
		/// </summary>
		/// <param name="logMessage"> The message to log </param>
		/// <param name="logLevel"> The log's output severity </param>
		/// <param name="callerOrigin"> Where the logger was called from </param>
		/// <param name="filePath"> The path where the log came from </param>
		/// <param name="lineNumber"> The line number where the log was called </param>
		public void Log(string logMessage, LogLevel logLevel = LogLevel.Info, [CallerMemberName]string callerOrigin = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0);

		#endregion

	};
};