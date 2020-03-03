﻿namespace ProcessManager.UI
{
    using System;

    /// <summary>
    /// A class that contains data and functionality of a process
    /// </summary>
    public class ProcessModel
    {
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
        /// A boolean flag that determines if a process is running
        /// </summary>
        public bool IsRunning => ProcessID != 0;


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
            ulong result = CoreDLL.RunProcess(ProcessPath, ProcessArgs);
            ProcessID = result;

            // If process ID returned as 0 it means process creation failed
            return ProcessID != 0 ? true : false;
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