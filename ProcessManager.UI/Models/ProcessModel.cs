namespace ProcessManager.UI
{
    using System;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// A class that contains data and functionality of a process
    /// </summary>
    public class ProcessModel
    {

        #region Private fields

        private string[] _cmdExtensions = new string[]
        {
            ".cmd",
            ".bat",
        };

        #endregion


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

        public bool IsProcessConsole => _cmdExtensions.Contains(Path.GetExtension(ProcessPath));

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



        /// <summary>
        /// Run the current process
        /// </summary>
        /// <returns></returns>
        public bool RunProcess()
        {
            // Check if the process if currently running
            if (IsRunning == true)
                return false;

            if (IsProcessConsole)
            {
                // Call WinApi function to create the process and set the process ID
                ulong processID = CoreDLL.RunConsoleProcess(ProcessPath, ProcessArgs, ProcessClosedEvent, VisibleOnStartup);
                ProcessID = processID;
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