namespace ProcessManager.UI
{
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
        public static extern ulong RunProcess(string processName, string processArgs, bool visibleOnStartup);

        /// <summary>
        /// Closes a single process
        /// </summary>
        /// <param name="processID"> The process' ID </param>
        [DllImport(DLL)]
        public static extern void CloseProcess(ulong processID);


        [DllImport(DLL)]
        public static extern bool HideProcess(ulong processID);

        [DllImport(DLL)]
        public static extern bool ShowProcess(ulong processID);

    };
};
