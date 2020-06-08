namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// The ViewModel for ProcessItemView
    /// </summary>
    public class ProcessItemViewModel : BaseViewModel
    {

        public static ProcessItemViewModel DesignInstance => new ProcessItemViewModel(null,
            new GUIProcess(Path.GetRandomFileName()));


        private const string DRAG_DROP_DATA_NAME = "ViewModelData";


        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _processRunning;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _processVisible;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _processLabelVisible;
        private bool _isSelected;
        private bool _dragAndDropEnabled;

        #endregion


        #region Public properties


        /// <summary>
        /// "Shared" drag and drop data that will be passed between "dragiee" and "dropiee" controls
        /// </summary>
        public Dictionary<string, object> DragDropData { get; }


        public ProjectItemViewModel ProjectItemVM { get; set; }


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
        /// The name of the process without path
        /// </summary>
        public string ProcessName
        {
            get
            {
                if (Process is ConsoleProcess consoleProcess)
                    return Path.GetFileName(consoleProcess.StartupDirectory);
                else if (Process is GUIProcess guiProcess)
                    return Path.GetFileName(guiProcess.ProcessPath);
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


        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }


        public bool DragAndDropEnabled
        {
            get => _dragAndDropEnabled;
            set
            {
                _dragAndDropEnabled = value;
                OnPropertyChanged();
            }
        }


        #endregion


        #region Commands

        public ICommand RunProcessCommand { get; }
        public ICommand CloseProcessCommand { get; }


        public ICommand ShowProcessCommand { get; }
        public ICommand HideProcessCommand { get; }

        public ICommand MouseEnterCommand { get; }
        public ICommand MouseLeaveCommand { get; }


        public ICommand EditProcessCommand { get; }



        public ICommand DragEnterCommand { get; }
        public ICommand DragLeaveCommand { get; }
        public ICommand DropCommand { get; }

        public ICommand DragCommand { get; }

        public ICommand MouseMovedCommand { get; set; }

        #endregion


        public ProcessItemViewModel(ProjectItemViewModel project, IProcessModel process)
        {
            ProjectItemVM = project;
            Process = process;


            DragDropData = new Dictionary<string, object>()
            {
                { DRAG_DROP_DATA_NAME, this },
            };


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


            RunProcessCommand = new AsyncRelayCommand(
                async () => await Task.Run(Process.RunProcess));

            CloseProcessCommand = new AsyncRelayCommand(
                async () => await Task.Run(Process.CloseProcess));

            ShowProcessCommand = new RelayCommand(() => Process.ShowProcessWindow());
            HideProcessCommand = new RelayCommand(() => Process.HideProcessWindow());

            EditProcessCommand = new RelayCommand(ExecuteEditProcessCommand);


            // Bind mouse enter/leave command if a process label has been specified in ProcessList.json file
            if (ProcessHasLabel == true)
            {
                MouseEnterCommand = new RelayCommand(() => ProcessLabelVisible = true);
                MouseLeaveCommand = new RelayCommand(() => ProcessLabelVisible = false);
            };





            // Bind drag and drop events
            DragEnterCommand = new RelayCommand<ProjectListItemViewModel>((project) =>
            {
                IsSelected = true;
            });

            DragLeaveCommand = new RelayCommand<ProjectListItemViewModel>((project) =>
            {
                IsSelected = false;
            });

            DropCommand = new RelayCommand<Dictionary<string, object>>((dragDropData) =>
            {
                // Validate that data actually exists
                if (dragDropData is null)
                {
                    DI.Logger.Log("Drag and drop error. Dropped data is null", LogLevel.Warning);
                    Debugger.Break();
                    return;
                };

                // Check if dropped data actually contains valid info
                if (dragDropData.TryGetValue(DRAG_DROP_DATA_NAME, out object dropData) == false)
                {
                    DI.Logger.Log("Drag and drop error. Unable to find valid data", LogLevel.Error);
                    Debugger.Break();
                    return;
                };

                // Check if dropped data is actually of the correct type
                if (!(dropData is ProcessItemViewModel dropDataAsVM))
                {
                    DI.Logger.Log("Drag and drop error. Dropped data contains invald value(s)", LogLevel.Warning);
                    Debugger.Break();
                    return;
                };

                if (dropDataAsVM == this)
                    return;

                // Do drop stuff
                Drop(dropDataAsVM);
            });

            // When the mouse moves over the drag drop button
            MouseMovedCommand = new RelayCommand<MouseMovedInfo>((mouseInfo) =>
            {
                // Check if user held his left mouse button
                if (mouseInfo.LeftButtonPressed == true)
                {
                    // Enable drag drop
                    DragAndDropEnabled = true;
                }
                else
                {
                    DragAndDropEnabled = false;
                };
            });

        }


        private void ExecuteEditProcessCommand()
        {
            switch (Process.ProcessType)
            {
                case ProcessType.Console:
                    DI.UI.ChangeView(View.EditConsoleProcessView, new EditConsoleProcessViewModel(ProjectItemVM, this));
                    break;

                case ProcessType.GUI:
                    DI.UI.ChangeView(View.EditGUIProcessView, new EditGUIProcessViewModel(ProjectItemVM, this));
                    break;

            };
        }


        private void Drop(ProcessItemViewModel droppedData)
        {
            var draggedIndex = ProjectItemVM.Project.ProcessList.IndexOf(droppedData.Process);
            var droppedIndex = ProjectItemVM.Project.ProcessList.IndexOf(Process);

            var temp = ProjectItemVM.Project.ProcessList[draggedIndex];

            ProjectItemVM.Project.ProcessList[draggedIndex] = Process;
            ProjectItemVM.Project.ProcessList[droppedIndex] = temp;

            ProjectItemVM.UpdateProcessList();


            var serializedProcessList = DI.Serializer.SerializeProcessList(ProjectItemVM.Project.ProcessList);

            File.WriteAllBytes(ProjectItemVM.Project.ProjectPathWithConfig, serializedProcessList);
        }

    };
};