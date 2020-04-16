namespace ProcessManager.UI
{
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Input;

    /// <summary>
    /// The ViewModel for ProcessItemView
    /// </summary>
    public class ProcessItemViewModel : BaseViewModel
    {
        public static ProcessItemViewModel DesignInstance => new ProcessItemViewModel(
            new ProcessModel()
            {
                ProcessPath = Path.GetRandomFileName(),
            });


        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _processRunning;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _processVisible;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _processLabelVisible;

        #endregion


        #region Public properties

        /// <summary>
        /// The process associated with this viewmodel
        /// </summary>
        public ProcessModel Process { get; set; }

        /// <summary>
        /// The process path, Formatted
        /// </summary>
        public string ProcessPath => Path.GetFullPath(Process.ProcessPath);

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

        /// <summary>
        /// A boolean flag that indicates if the current process is shown
        /// </summary>
        public bool ProcessVisible
        {
            get => _processVisible;
            set
            {
                _processVisible = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// A boolean flag that indicates if the process label should be visible, or not
        /// </summary>
        public bool ProcessLabelVisible
        {
            get => _processLabelVisible;
            set
            {
                _processLabelVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A boolean flag that indicates if the associated process has a label
        /// </summary>
        public bool ProcessHasLabel => string.IsNullOrWhiteSpace(Process.ProcessLabel);

        #endregion


        #region Commands

        public ICommand RunProcessCommand { get; }
        public ICommand CloseProcessCommand { get; }


        public ICommand ShowProcessCommand { get; }
        public ICommand HideProcessCommand { get; }

        public ICommand MouseEnterCommand { get; }
        public ICommand MouseLeaveCommand { get; }

        #endregion



        public ProcessItemViewModel(ProcessModel process)
        {
            Process = process;

            _processRunning = process.IsRunning;
            _processVisible = process.ProcessVisibilityState > 0 ? true : false;


            // Bind the process closed event
            Process.ProcessClosedEvent += () => ProcessRunning = false;

            Process.ProcessInitializedEvent += () =>
            {
                if (Process.VisibleOnStartup == false)
                    ProcessVisible = false;
                else
                    ProcessVisible = true;


                ProcessRunning = true;
            };
            Process.ProcessVisibilityStateChanged += (ProcessVisibilityState visibilityState) =>
            {
                if (visibilityState == ProcessVisibilityState.Visible)
                {
                    ProcessVisible = true;
                }
                else if (visibilityState == ProcessVisibilityState.Hidden)
                {
                    ProcessVisible = false;
                };
            };


            RunProcessCommand = new RelayCommand(() => Process.RunProcess());
            CloseProcessCommand = new RelayCommand(() => Process.CloseProcessTree());

            ShowProcessCommand = new RelayCommand(() => Process.ShowProcess());
            HideProcessCommand = new RelayCommand(() => Process.HideProcess());


            // Bind mouse enter/leave command if a process label has been specified in ProcessList.json file
            if (ProcessHasLabel == false)
            {
                MouseEnterCommand = new RelayCommand(() => ProcessLabelVisible = true);

                MouseLeaveCommand = new RelayCommand(() => ProcessLabelVisible = false);
            };
        }
    };
};