namespace ProcessManager.UI
{
    using System;


    /// <summary>
    /// A process that will be ran directly from the console
    /// </summary>
    public class ConsoleProcess : ProcessBase
    {

        /// <summary>
        /// If user decides to run as CMD this will store the console script
        /// </summary>
        public string ConsoleScript { get; }

        /// <summary>
        /// A directory from where the console script will execute
        /// </summary>
        public string StartupDirectory { get; }

        public override event Action<IProcessModel> ProcessClosedCallback;

        public override event Action<IProcessModel> ProcessInitializedCallback;

        public ConsoleProcess(string consoleScript, string startupDirectory, bool visibleOnStartup = true)
        {
            ConsoleScript = consoleScript;
            StartupDirectory = startupDirectory;
            VisibleOnStartup = visibleOnStartup;

            // Set process type
            ProcessType = ProcessType.Console;

            _processClosedCallback = (IntPtr process) =>
            {
                ProcessClosedCallback?.Invoke(this);
            };

            _processInitialziedCallback = (IntPtr process) =>
            {
                ProcessInitializedCallback?.Invoke(this);
            };

            CreateProcessObject(null, null, consoleScript, startupDirectory, ProcessType, visibleOnStartup, _processClosedCallback, _processInitialziedCallback, ref _processPointer);
        }

        ~ConsoleProcess()
        {
            DestroyProcessObject(ref _processPointer);
        }

    };
};