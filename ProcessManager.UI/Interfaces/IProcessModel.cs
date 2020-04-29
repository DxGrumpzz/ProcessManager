namespace ProcessManager.UI
{
    using System;

    /// <summary>
    /// An interface describing a process 
    /// </summary>
    public interface IProcessModel
    {
        
        /// <summary>
        /// A boolean flag indicating if this process is currently running
        /// </summary>
        public bool IsRunning { get; }

        /// <summary>
        /// A boolean flag indicating if this process will be invisible when it starts
        /// </summary>
        public bool VisibleOnStartup { get; set; }

        /// <summary>
        /// The process' current visibility state
        /// </summary>
        public ProcessVisibilityState VisibilityState { get; }
        
        /// <summary>
        /// A label associated with this process
        /// </summary>
        public string ProcessLabel { get; set; }

        /// <summary>
        /// The type of process this is
        /// </summary>
        public ProcessType ProcessType { get; set; }



        /// <summary>
        /// An event that will be fired when the user changes process visibility
        /// </summary>
        public event Action<IProcessModel, ProcessVisibilityState> ProcessVisibilityChanged;

        /// <summary>
        /// An event that will be fired when the process closes
        /// </summary>
        public event Action<IProcessModel> ProcessClosedCallback;

        /// <summary>
        /// An event that will be fired when the process initialized
        /// </summary>
        public event Action<IProcessModel> ProcessInitializedCallback;


        /// <summary>
        /// Run this process
        /// </summary>
        /// <returns></returns>
        public bool RunProcess();

        /// <summary>
        /// Close this process
        /// </summary>
        /// <returns></returns>
        public bool CloseProcess();


        /// <summary>
        /// Hide this process' MainWindow
        /// </summary>
        public void HideProcessWindow();

        /// <summary>
        /// Show this process' MainWindow
        /// </summary>
        public void ShowProcessWindow();

    };
};