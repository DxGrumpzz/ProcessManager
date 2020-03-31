namespace ProcessManager.UI
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A static class that contains Interop functions 
    /// </summary>
    public static class CoreDLL
    {
        /// <summary>
        /// The name of the DLL
        /// </summary>
        private const string DLL = "ProcessManager.Core.dll";

        /// <summary>
        /// Initializes the ProcessManager functionality
        /// </summary>
        [DllImport(DLL)]
        public static extern void Initialize();

        /// <summary>
        /// Runs a single process
        /// </summary>
        /// <param name="processName"> The process path </param>
        /// <param name="processArgs"> The process' arguments </param>
        /// <returns></returns>
        [DllImport(DLL, CharSet = CharSet.Unicode)]
        public static extern ulong RunProcess(string processName, string processArgs, Action processClosedCallback, bool visibleOnStartup);

        /// <summary>
        /// Closes a single process
        /// </summary>
        /// <param name="processID"> The process' ID </param>
        [DllImport(DLL)]
        public static extern void CloseProcess(ulong processID);


        /// <summary>
        /// Hides a process' main wwindow, nothing sketcy
        /// </summary>
        /// <param name="processID"> The process' PID </param>
        /// <returns> Returns true if WinCall (ShowWindow) was successful </returns>
        [DllImport(DLL)]
        public static extern bool HideProcess(ulong processID);

        /// <summary>
        /// Shows a process
        /// </summary>
        /// <param name="processID"> The process' PID </param>
        /// <returns> Returns true if WinCall (ShowWindow) was successful </returns>
        [DllImport(DLL)]
        public static extern bool ShowProcess(ulong processID);

        /// <summary>
        /// Runs a single process as a console.
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="processArgs"></param>
        /// <param name="processClosedCallback"></param>
        /// <param name="visibleOnStartup"></param>
        /// <returns></returns>
        /// <remarks>
        /// Special process initiation is used to create processes of type *.bat, *.cmd, etc 
        /// </remarks>
        [DllImport(DLL, CharSet = CharSet.Unicode)]
        public static extern ulong RunConsoleProcess(string processName, string processArgs, Action processClosedCallback, bool visibleOnStartup);

        /// <summary>
        /// Closes a process and it's children
        /// </summary>
        /// <param name="processID"></param>
        [DllImport(DLL)]
        public static extern void CloseProcessTree(ulong processID);

        /// <summary>
        /// Checks if a process is running
        /// </summary>
        /// <param name="processID"> The process ID </param>
        /// <returns> Returns a boolean value indicating if the process is running </returns>
        [DllImport(DLL)]
        public static extern bool IsProcessRunning(ulong processID);

    };
};