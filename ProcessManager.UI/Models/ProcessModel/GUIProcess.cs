namespace ProcessManager.UI
{
    using System;


    /// <summary>
    /// A process running using the OS gui
    /// </summary>
    public class GUIProcess : ProcessBase
    {
        public override event Action<IProcessModel> ProcessClosedCallback;
        public override event Action<IProcessModel> ProcessInitializedCallback;


        /// <summary>
        /// A string contaning the path to the process
        /// </summary>
        public string ProcessPath { get; set; }

        /// <summary>
        /// A string contaning the process arguments 
        /// </summary>
        public string ProcessArgs { get; set; }


        public GUIProcess(string processPath, string processArgs = "", bool visibleOnStartup = true)
        {
            ProcessPath = processPath;
            ProcessArgs = processArgs;
            VisibleOnStartup = visibleOnStartup;

            // Set process type
            ProcessType = ProcessType.GUI;


            _processClosedCallback = (IntPtr process) =>
            {
                ProcessClosedCallback?.Invoke(this);
            };

            _processInitialziedCallback = (IntPtr process) =>
            {
                ProcessInitializedCallback?.Invoke(this);
            };

            CreateProcessObject(processPath, processArgs, null, null, ProcessType, visibleOnStartup, _processClosedCallback, _processInitialziedCallback, ref _processPointer);
        }


        ~GUIProcess()
        {
            DestroyProcessObject(ref _processPointer);
        }

    };
};
