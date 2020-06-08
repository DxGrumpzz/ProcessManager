namespace ProcessManager.UI
{
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Input;

    /// <summary>
    /// 
    /// </summary>
    public class AddGUIProcessViewModel : BaseViewModel
    {
        public static AddGUIProcessViewModel DesignInstance => new AddGUIProcessViewModel(
            new ProjectItemViewModel(new Project()
            {
                ProjectPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetRandomFileName()}",
            }))
        {
            SelectedProcessPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetRandomFileName()}",
        };


        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _selectedProcessPath;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _addProcessEnabled;

        #endregion


        #region Public properties

        /// <summary>
        /// A ProjectViewModel which is used when the user wants to go back to the Project view and not lose any saved data
        /// </summary>
        public ProjectItemViewModel ProjectItemVM { get; }

        /// <summary>
        /// A path to the process
        /// </summary>
        public string SelectedProcessPath
        {
            get => _selectedProcessPath;
            set
            {
                _selectedProcessPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// An argument list for the process
        /// </summary>
        public string ProcessAgs { get; set; } = string.Empty;

        /// <summary>
        /// A boolean flag indiacting if this process will be visible when it starts
        /// </summary>
        public bool ProcessVisibleOnStartup { get; set; } = true;

        /// <summary>
        /// A label associated with this process
        /// </summary>
        public string ProcessLabel { get; set; } = string.Empty;

        public bool AddProcessEnabled
        {
            get => _addProcessEnabled;
            set
            {
                _addProcessEnabled = value;
                OnPropertyChanged();
            }
        }


        #endregion


        #region Commands

        public ICommand SelectProcessCommand { get; }
        
        public ICommand BackToMainPageCommand { get; }
        public ICommand SwitchToProjectViewCommand { get; }
        public ICommand SwitchToProcessSelectionViewCommand { get; }

        public ICommand AddProcessCommand { get; }

        public ICommand OpenProjectDirectoryCommand { get; }

        #endregion


        public AddGUIProcessViewModel(ProjectItemViewModel projectVM)
        {
            ProjectItemVM = projectVM;

            SelectProcessCommand = new RelayCommand(ExecuteSelectProcessCommand);


            BackToMainPageCommand = new RelayCommand(ExecuteBackToMainPageCommand);
            
            SwitchToProcessSelectionViewCommand = new RelayCommand(ExecuteSwitchToProcessSelectionViewCommand);

            SwitchToProjectViewCommand = new RelayCommand(() =>
            DI.UI.ChangeView(View.ProjectItemView, ProjectItemVM));


            AddProcessCommand = new RelayCommand(
                ExecuteAddProcessCommand,
                // Don't enable the button until the user chose a valid process path
                () => SelectedProcessPath?.Length >= 6 && 
                // And make sure the file actually exists
                File.Exists(SelectedProcessPath));

            OpenProjectDirectoryCommand = new RelayCommand(() =>
            DI.FolderDialog.OpenFolder(ProjectItemVM.Project.ProjectPath));
        }


        private void ExecuteAddProcessCommand()
        {
            if(string.IsNullOrWhiteSpace(SelectedProcessPath) ||
                SelectedProcessPath?.Length < 6)
            {
                DI.UserDialog.ShowDialog("An invalid process path was given");
                return;
            };

            var project = ProjectItemVM.Project;

            // Add the process  to the projets' process list
            project.ProcessList.Add(new GUIProcess(SelectedProcessPath, ProcessAgs, ProcessVisibleOnStartup)
            {
                ProcessLabel = ProcessLabel,
            });

            ProjectItemVM.UpdateProcessList();

            byte[] json = DI.Serializer.SerializeProcessList(project.ProcessList);

            // Write the json to the project's config file
            File.WriteAllBytes(project.ProjectPathWithConfig, json);

            // Return back to the Project's view
            ExecuteSwitchToProcessSelectionViewCommand();


        }

        private void ExecuteSelectProcessCommand()
        {
            // Use the open file dialog to select a process
            var fileDialog = DI.FileDialog;

            // If user has selected a process
            if (fileDialog.ShowOpenFileDialog() == true)
            {
                // Set selected path
                SelectedProcessPath = fileDialog.SelectedFilePath;
            };

        }


        private void ExecuteBackToMainPageCommand()
        {
            DI.UI.ChangeView(View.ProjectsListView, DI.ProjectsListVM);
        }

        private void ExecuteSwitchToProcessSelectionViewCommand()
        {
            DI.UI.ChangeView(View.AddProcessView, new AddProcessViewModel(ProjectItemVM));
        }

    };
};