namespace ProcessManager.UI
{
    using System;

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
        public bool IsRunning => ProcessID != 0;

        /// <summary>
        /// User defined info about the process
        /// </summary>
        public string ProcessLabel { get; set; }

        #endregion

        public event Action ProcessClosedEvent;

        public event Action ProcessInitializedEvent;

        public ProcessModel()
        {
            
            // Ensure process is closed
            ProcessClosedEvent += () => CloseProcess();
        }


        /// <summary>
        /// Run the current process
        /// </summary>
        /// <returns></returns>
        public bool RunProcess()
        {
            // Check if the process if currently running
            if (IsRunning == true)
                return false;

            // Call WinApi function to create the process and set the process ID
            ulong processID = CoreDLL.RunProcess(ProcessPath, ProcessArgs, ProcessClosedEvent, VisibleOnStartup);
            ProcessID = processID;

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

    };
};