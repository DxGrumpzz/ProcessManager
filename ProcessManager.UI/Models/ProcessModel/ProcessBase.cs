namespace ProcessManager.UI
{
    using System;
    using System.Runtime.InteropServices;


    /// <summary>
    /// 
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


        #region DLL calls

        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        protected extern static void CreateProcessObject(string processPath, string processArgs,
                                                         string consoleScript, string startupDirectory,
                                                         bool runAsConsole,
                                                         _ProcessClosedCallBack processClosedCallback, ref IntPtr process);

        [DllImport(DLL_NAME)]
        protected extern static void DestroyProcessObject(ref IntPtr process);


        [DllImport(DLL_NAME)]
        protected extern static bool RunProcess(ref IntPtr process);

        [DllImport(DLL_NAME)]
        protected extern static bool CloseProcess(ref IntPtr process);


        [DllImport(DLL_NAME)]
        protected extern static bool ProcessRunning(IntPtr process);


        [DllImport(DLL_NAME)]
        protected extern static bool ShowProcessWindow(IntPtr process);

        [DllImport(DLL_NAME)]
        protected extern static bool HideProcessWindow(IntPtr process);

        #endregion


        protected IntPtr _processPointer = IntPtr.Zero;

        public virtual bool IsRunning => ProcessRunning(_processPointer);

        protected _ProcessClosedCallBack _processClosedCallback;

        public abstract event Action<IProcessModel> ProcessClosedCallback;


        public virtual bool RunProcess()
        {
            if (IsRunning == true)
                return false;

            return RunProcess(ref _processPointer);
        }

        public virtual bool CloseProcess()
        {
            if (IsRunning == false)
                return false;

            return CloseProcess(ref _processPointer);
        }


        public virtual void HideProcessWindow()
        {
            ShowProcessWindow(_processPointer);
        }

        public virtual void ShowProcessWindow()
        {
            HideProcessWindow(_processPointer);
        }

    };
};