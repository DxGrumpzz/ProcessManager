namespace ProcessManager.UI
{
    using System;


    /// <summary>
    ///
    /// </summary>
    public interface IProcessModel
    {
        public bool IsRunning { get; }


        public event Action<IProcessModel> ProcessClosedCallback;

        public bool RunProcess();
        public bool CloseProcess();

        
        public void HideProcessWindow();
        public void ShowProcessWindow();


    };
};