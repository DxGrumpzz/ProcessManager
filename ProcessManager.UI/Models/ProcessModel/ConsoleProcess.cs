namespace ProcessManager.UI
{
    using System;


    /// <summary>
    /// 
    /// </summary>
    public class ConsoleProcess : ProcessBase
    {
        public override bool IsRunning => ProcessRunning(_processPointer);


        /// <summary>
        /// If user decides to run as CMD this will store the console script
        /// </summary>
        public string ConsoleScript { get; }

        /// <summary>
        /// A directory from where the console script will execute
        /// </summary>
        public string StartupDirectory { get; }

        public override event Action<IProcessModel> ProcessClosedCallback;


        public ConsoleProcess(string consoleScript, string startupDirectory)
        {
            ConsoleScript = consoleScript;
            StartupDirectory = startupDirectory;

            _processClosedCallback = (IntPtr process) =>
            {
                ProcessClosedCallback?.Invoke(this);
            };

            CreateProcessObject(null, null, consoleScript, startupDirectory, true, _processClosedCallback, ref _processPointer);
        }

        ~ConsoleProcess()
        {
            DestroyProcessObject(ref _processPointer);
        }

    };
};