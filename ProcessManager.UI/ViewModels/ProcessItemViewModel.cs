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
            new GUIProcess(Path.GetRandomFileName()));


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
        public IProcessModel Process { get; set; }

        /// <summary>
        /// The process path, Formatted
        /// </summary>
        public string ProcessPath 
        {
            get
            {
                if (Process is ConsoleProcess consoleProcess)
                    return Path.GetFullPath(consoleProcess.StartupDirectory);
                else if (Process is GUIProcess guiProcess)
                    return Path.GetFullPath(guiProcess.ProcessPath);
                else
                {
                    Debugger.Break();
                    return null;
                };

            }
        } 

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
        public bool ProcessHasLabel => !string.IsNullOrWhiteSpace(Process.ProcessLabel);

        #endregion


        #region Commands

        public ICommand RunProcessCommand { get; }
        public ICommand CloseProcessCommand { get; }


        public ICommand ShowProcessCommand { get; }
        public ICommand HideProcessCommand { get; }

        public ICommand MouseEnterCommand { get; }
        public ICommand MouseLeaveCommand { get; }

        #endregion



        public ProcessItemViewModel(IProcessModel process)
        {
            Process = process;

            ProcessRunning = process.IsRunning;

            ProcessVisible = process.VisibleOnStartup;


            // Bind the process closed event
            Process.ProcessClosedCallback += (IProcessModel process) => ProcessRunning = false;

             Process.ProcessInitializedCallback += (IProcessModel process) =>
             {
                 if (Process.VisibleOnStartup == false)
                     ProcessVisible = false;
                 else
                     ProcessVisible = true;


                 ProcessRunning = true;
             };

            Process.ProcessVisibilityChanged += (IProcessModel process, ProcessVisibilityState visibilityState) =>
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
            CloseProcessCommand = new RelayCommand(() => Process.CloseProcess());

            ShowProcessCommand = new RelayCommand(() => Process.ShowProcessWindow());
            HideProcessCommand = new RelayCommand(() => Process.HideProcessWindow());


            // Bind mouse enter/leave command if a process label has been specified in ProcessList.json file
            if (ProcessHasLabel == true)
            {
                MouseEnterCommand = new RelayCommand(() => ProcessLabelVisible = true);
                MouseLeaveCommand = new RelayCommand(() => ProcessLabelVisible = false);
            };
        }
    };
};