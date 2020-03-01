namespace ProcessManager.UI
{
    using System.Diagnostics;
    using System.Windows.Input;

    /// <summary>
    /// The ViewModel for ProcessItemView
    /// </summary>
    public class ProcessItemViewModel : BaseViewModel
    {
        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _processRunning;

        #endregion


        #region Public properties

        /// <summary>
        /// The process associated with this viewmodel
        /// </summary>
        public ProcessModel Process { get; set; }

        /// <summary>
        /// The name of this process
        /// </summary>
        public string ProcessName => Process.ProcessName;

        /// <summary>
        /// A boolean flag that indicates if this process is currently running
        /// </summary>
        public bool ProcessRunning
        {
            get => _processRunning;
            set
            {
                _processRunning = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public ICommand RunProcessCommand { get; }
        public ICommand CloseProcessCommand { get; }


        public ProcessItemViewModel()
        {
            RunProcessCommand = new RelayCommand(ExecuteRunProcessCommand);
            CloseProcessCommand= new RelayCommand(ExecuteCloseProcessCommand);
        }


        private void ExecuteRunProcessCommand()
        {
            if (Process.RunProcess() == true)
                ProcessRunning = true;
        }

        private void ExecuteCloseProcessCommand()
        {
            if(Process.CloseProcess() == true)
                ProcessRunning = false;
        }

    };
};
