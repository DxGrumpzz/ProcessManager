namespace ProcessManager.UI
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A class that contains data and functionality of a process
    /// </summary>
    public class ProcessModel
    {


        #region Public properties

        /// <summary>
        /// The path/name of the process 
        /// </summary>
        public string ProcessPath { get; set; }

        /// <summary>
        /// The process' arguments
        /// </summary>
        public string ProcessArgs { get; set; }

        /// <summary>
        /// The PID of the process 
        /// </summary>
        public ulong ProcessID { get; set; }

        /// <summary>
        /// A boolean flag that indicates if this process will start invisible
        /// </summary>
        public bool VisibleOnStartup { get; set; }

        /// <summary>
        /// A boolean flag that determines if a process is running
        /// </summary>
        public bool IsRunning => CoreDLL.IsProcessRunning(ProcessID);

        /// <summary>
        /// User defined info about the process
        /// </summary>
        public string ProcessLabel { get; set; }

        /// <summary>
        /// The visibility state of this process
        /// </summary>
        public ProcessVisibilityState ProcessVisibilityState { get; set; }


        /// <summary>
        /// A boolean flag indicating that this process is to be ran from the console
        /// </summary>
        public bool RunAsConsole { get; set; }

        /// <summary>
        /// Directory path to run a console script from. Used only in console processes
        /// </summary>
        public string StartInDirectory { get; set; }

        /// <summary>
        /// A script to be ran in the console
        /// </summary>
        public string ConsoleScript { get; set; }


        #endregion


        /// <summary>
        /// An event that will be fired when this process is closed
        /// </summary>
        public event Action ProcessClosedEvent;

        /// <summary>
        /// An event that will be fired when the process is started successfully
        /// </summary>
        public event Action ProcessInitializedEvent;

        /// <summary>
        /// An event that will be fired when the process visibility has changed
        /// </summary>
        public event Action<ProcessVisibilityState> ProcessVisibilityStateChanged;


        [DllImport("ProcessManager.Core.dll", CharSet = CharSet.Unicode)]
        private extern static bool Test(string runFromDirectory, string script, Action processClosedEvent, bool visibleOnStartup, out ulong pid);


        /// <summary>
        /// Run the current process
        /// </summary>
        /// <returns></returns>
        public bool RunProcess()
        {
            // Check if the process if currently running
            if (IsRunning == true)
                return false;

            if (RunAsConsole)
            {
                // Call WinApi function to create the process and set the process ID
                bool result = Test(StartInDirectory, ConsoleScript, ProcessClosedEvent, VisibleOnStartup, out ulong pid);

                if (result == false)
                {
                    ProcessID = pid;
                }
                else
                    ProcessID = pid;

                ProcessInitializedEvent?.Invoke();

                return result;
            }
            else
            {
                // Call WinApi function to create the process and set the process ID
                ulong processID = CoreDLL.RunProcess(ProcessPath, ProcessArgs, ProcessClosedEvent, VisibleOnStartup);
                ProcessID = processID;
            };

            // If process ID returned as 0 it means process creation failed
            if (ProcessID != 0 ? true : false)
            {
                ProcessInitializedEvent?.Invoke();
                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// Close this process
        /// </summary>
        /// <returns></returns>
        public bool CloseProcess()
        {
            // Check to see if the process is already closed
            if (IsRunning == false)
                return false;

            // Call WinApi function that closes this process
            CoreDLL.CloseProcess(ProcessID);

            // Set process PID back to 0
            ProcessID = 0;

            return true;
        }

        /// <summary>
        /// Closes a process tree
        /// </summary>
        /// <returns></returns>
        public bool CloseProcessTree()
        {
            // Check to see if the process is already closed
            if (IsRunning == false)
                return false;

            CoreDLL.CloseProcessTree(ProcessID);

            // Set process PID back to 0
            ProcessID = 0;

            return true;
        }



        /// <summary>
        /// Shows a process to the user
        /// </summary>
        /// <returns></returns>
        public bool ShowProcess()
        {
            bool result = CoreDLL.ShowProcess(ProcessID);

            // If ShowWindow function was called succesfuly
            if (result == true)
            {
                ProcessVisibilityState = ProcessVisibilityState.Visible;

                // Invoke event
                ProcessVisibilityStateChanged?.Invoke(ProcessVisibilityState);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Hides a process from the user
        /// </summary>
        /// <returns></returns>
        public bool HideProcess()
        {
            bool result = CoreDLL.HideProcess(ProcessID);

            // If ShowWindow function was called succesfuly
            if (result == true)
            {
                ProcessVisibilityState = ProcessVisibilityState.Hidden;

                // Invoke event
                ProcessVisibilityStateChanged?.Invoke(ProcessVisibilityState);
                return true;
            }
            else
                return false;
        }

    };
};