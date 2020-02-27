namespace ProcessManager.UI
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// 
    /// </summary>
    public static class CoreDLL
    {
        private const string DLL = "ProcessManager.Core.dll";

        [DllImport(DLL)]
        public static extern void Initialize();

        [DllImport(DLL, CharSet = CharSet.Unicode)]
        public static extern ulong RunProcess(string processName, string processArgs);

        [DllImport(DLL)]
        public static extern void CloseProcess(ulong processID);


    };
};
