namespace ProcessManager.UI
{
    using System;
    using System.Runtime.InteropServices;


    /// <summary>
    /// An abstract class that only contains just enough data to make a base class for any process type
    /// </summary>
    public abstract class ProcessBase : IProcessModel
    {

        /// <summary>
        /// The name of the dll where the Core functions reside
        /// </summary>
        private const string DLL_NAME = "ProcessManager.Core.dll";


        /// <summary>
        /// Deleagte declaration for a invokeable C++ function
        /// </summary>
        /// <param name="process"></param>
        protected delegate void _ProcessClosedCallBack(IntPtr process);


        private object _process_run_close_monitor = new object();

        #region DLL calls

        /// <summary>
        /// Creates a heap allocated process and returns it's pointer
        /// </summary>
        /// <param name="processPath"></param>
        /// <param name="processArgs"></param>
        /// <param name="consoleScript"></param>
        /// <param name="startupDirectory"></param>
        /// <param name="runAsConsole"></param>
        /// <param name="visibleOnStartup"></param>
        /// <param name="processClosedCallback"></param>
        /// <param name="processInitialziedCallback"></param>
        /// <param name="process"></param>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        protected extern static void CreateProcessObject(string processPath, string processArgs,
                                                         string consoleScript, string startupDirectory,
                                                         bool runAsConsole,
                                                         bool visibleOnStartup,
                                                         _ProcessClosedCallBack processClosedCallback,
                                                         _ProcessClosedCallBack processInitialziedCallback,
                                                         ref IntPtr process);

        /// <summary>
        /// Because we create unmanaged memory in <see cref="CreateProcessObject(string, string, string, string, bool, bool, _ProcessClosedCallBack, _ProcessClosedCallBack, ref IntPtr)"/> 
        /// we have to free it using 'delete'
        /// </summary>
        /// <param name="process"> The pointer to the process pointer </param>
        [DllImport(DLL_NAME)]
        protected extern static void DestroyProcessObject(ref IntPtr process);



        /// <summary>
        /// Runs the process
        /// </summary>
        /// <param name="process"> Pointer to the process </param>
        /// <returns></returns>
        [DllImport(DLL_NAME)]
        protected extern static bool RunProcess(ref IntPtr process);

        /// <summary>
        /// Gracefully closes the process
        /// </summary>
        /// <param name="process"> Pointer to the process </param>
        /// <returns></returns>
        [DllImport(DLL_NAME)]
        protected extern static bool CloseProcess(ref IntPtr process);



        /// <summary>
        /// Check if the process is currently running
        /// </summary>
        /// <param name="process"> Pointer to the process </param>
        /// <returns></returns>
        [DllImport(DLL_NAME)]
        protected extern static bool ProcessRunning(IntPtr process);



        /// <summary>
        /// Show the process main window
        /// </summary>
        /// <param name="process"> Pointer to the process </param>
        /// <returns></returns>
        [DllImport(DLL_NAME)]
        protected extern static bool ShowProcessWindow(IntPtr process);

        /// <summary>
        /// Hides the process main window
        /// </summary>
        /// <param name="process"> Pointer to the process </param>
        /// <returns></returns>
        [DllImport(DLL_NAME)]
        protected extern static bool HideProcessWindow(IntPtr process);

        #endregion


        /// <summary>
        /// A pointer to unmanged memory contaning the process created in C++
        /// </summary>
        protected IntPtr _processPointer = IntPtr.Zero;

        public string ProcessLabel { get; set; }

        public bool IsRunning => ProcessRunning(_processPointer);

        public ProcessVisibilityState VisibilityState { get; private set; }

        public bool VisibleOnStartup { get; set; }

        public ProcessType ProcessType { get; set; }

        public bool IsInitialzing { get; private set; }

        /// <summary>
        /// A callback that will be called from C++ when this process closes
        /// </summary>
        protected _ProcessClosedCallBack _processClosedCallback;

        /// <summary>
        /// A callback that will be called from C++ when this process initializes
        /// </summary>
        protected _ProcessClosedCallBack _processInitialziedCallback;


        public abstract event Action<IProcessModel> ProcessClosedCallback;
        public abstract event Action<IProcessModel> ProcessInitializedCallback;

        public event Action<IProcessModel, ProcessVisibilityState> ProcessVisibilityChanged;



        public virtual bool RunProcess()
        {
            lock (_process_run_close_monitor)
            {
                // Don't run this process it it's alrady running
                if (IsRunning == true)
                    return false;

                if (IsInitialzing == true)
                    return false;

                try
                {
                    IsInitialzing = true;

                    // Run this process
                    return RunProcess(ref _processPointer);
                }
                finally
                {
                    IsInitialzing = false;
                };
            };
        }

        public virtual bool CloseProcess()
        {
            lock (_process_run_close_monitor)
            {
                // Don't close is process isn't running
                if (IsRunning == false)
                    return false;

                return CloseProcess(ref _processPointer);
            };
        }


        public virtual void HideProcessWindow()
        {
            // Hide this process mainwindow
            if (HideProcessWindow(_processPointer) == true)
            {
                // Notiify that this process has changed visibility
                VisibilityState = ProcessVisibilityState.Hidden;
                ProcessVisibilityChanged?.Invoke(this, VisibilityState);
            };
        }

        public virtual void ShowProcessWindow()
        {
            // Show this process mainwindow
            if (ShowProcessWindow(_processPointer) == true)
            {
                // Notiify that this process has changed visibility
                VisibilityState = ProcessVisibilityState.Visible;
                ProcessVisibilityChanged?.Invoke(this, VisibilityState);
            };
        }

    };
};