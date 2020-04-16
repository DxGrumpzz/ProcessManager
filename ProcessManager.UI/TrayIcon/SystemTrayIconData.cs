namespace ProcessManager.UI
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Create a function pointer that takes a handle to some C# data and returns nothing
    /// </summary>
    /// <param name="Data"></param>
    public delegate void CallbackFunc(IntPtr Data);

    /// <summary>
    /// Data that will be sent to the TrayIcon
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SystemTrayIconData
    {
        /// <summary>
        /// The name of the project
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// The actuall project as a handle
        /// </summary>
        public IntPtr Data { get; set; }

        /// <summary>
        /// An event that will be called from C++ when user requests to close a project 
        /// </summary>
        public event CallbackFunc CloseProjectCallBack;

        /// <summary>
        /// An event that will be called from C++ when user requests to Run a project 
        /// </summary>
        public event CallbackFunc RunProjectCallBack;

        public SystemTrayIconData(Project p)
        {
            Data = GCHandle.ToIntPtr(GCHandle.Alloc(p));

            ProjectName = p.ProjectName;

            CloseProjectCallBack = null;
            RunProjectCallBack = null;
        }
    };
};
