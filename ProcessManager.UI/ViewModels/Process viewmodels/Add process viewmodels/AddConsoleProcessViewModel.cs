namespace ProcessManager.UI
{
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Input;


    /// <summary>
    /// 
    /// </summary>
    public class AddConsoleProcessViewModel : BaseViewModel
    {
        public static AddConsoleProcessViewModel DesignInstance => new AddConsoleProcessViewModel(
        new ProjectItemViewModel(new Project()
        {
            ProjectPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
        }))
        {
            ConsoleDirectory = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
        };


        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _consoleDirectory;

        #endregion


        #region Public properties


        /// <summary>
        /// A ProjectViewModel which is used when the user wants to go back to the Project view and not lose any saved data
        /// </summary>
        public ProjectItemViewModel Project { get; }


        /// <summary>
        /// A path to a directory, which the console process will start on
        /// </summary>
        public string ConsoleDirectory
        {
            get => _consoleDirectory;
            set
            {
                _consoleDirectory = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A script to run in the console
        /// </summary>
        public string ConsoleScript { get; set; }

        /// <summary>
        /// A label associated with this process
        /// </summary>
        public string ProcessLabel { get; set; }

        /// <summary>
        /// A boolean flag indiacting if this process will be visible when it starts
        /// </summary>
        public bool ProcessVisibleOnStartup { get; set; } = true;

        #endregion


        #region Commands

        public ICommand SelectDirectoryCommand { get; }
        public ICommand SelectCurrentDirectoryCommand { get; }


        public ICommand BackToMainPageCommand { get; }
        public ICommand BackToProjectPageCommand { get; }

        public ICommand SwitchToProcessSelectionViewCommand { get; }
        public ICommand SwitchToProjectViewCommand { get; }

        public ICommand AddProcessCommand { get; }

        #endregion


        private AddConsoleProcessViewModel() { }
        public AddConsoleProcessViewModel(ProjectItemViewModel projectVM)
        {
            Project = projectVM;

            SelectDirectoryCommand = new RelayCommand(ExecuteSelectDirectoryCommand);
            SelectCurrentDirectoryCommand = new RelayCommand(() =>
                ConsoleDirectory = Project.Project.ProjectPath);

            BackToMainPageCommand = new RelayCommand(ExecuteBackToMainPageCommand);

            
            SwitchToProcessSelectionViewCommand = new RelayCommand(() =>
            // Go back to process type selection view
            DI.UI.ChangeView(View.AddProcessView, new AddProcessViewModel(Project)));


            BackToProjectPageCommand = new RelayCommand(() =>
            // Go back to the project item view
            DI.UI.ChangeView(View.ProjectItemView, Project));


            AddProcessCommand = new RelayCommand(
                ExecuteAddProcessCommand,
                // Don't execute command if the directory's length is  less than three charactes
                () => ConsoleDirectory?.Length >= 3 &&
                // And isn't a valid directory
                Directory.Exists(ConsoleDirectory));
        }


        private void ExecuteSelectDirectoryCommand()
        {
            var folderDialog = DI.FolderDialog;


            bool folderDialogResult;

            // Check if the Console directory actually exists
            if (string.IsNullOrWhiteSpace(ConsoleDirectory) == true &&
                // Check if directory exists
                Directory.Exists(ConsoleDirectory) == false)
            {
                // Show the folder dialog 
                folderDialogResult = folderDialog.ShowDialog();
            }
            else
                // Show the folder dialog from ConsoleDirectory
                folderDialogResult = folderDialog.ShowDialogFrom(ConsoleDirectory);


            // Check if the user has chose a valid directory
            if (folderDialogResult == false)
                return;

            // set the console directory to the selectd path
            ConsoleDirectory = folderDialog.SelectedPath;
        }


        private void ExecuteBackToMainPageCommand()
        {
            // Go back to to the projects list view
            DI.UI.ChangeView(View.ProjectsListView, DI.ProjectsListVM);
        }


        private void ExecuteAddProcessCommand()
        {
            // Validate directory path
            if (string.IsNullOrWhiteSpace(ConsoleDirectory) == true &&
                Directory.Exists(ConsoleDirectory) == false)
            {
                DI.UserDialog.ShowDialog("Invalid directory was selected");
                return;
            };

            // Validate console script
            if (string.IsNullOrWhiteSpace(ConsoleScript) == true)
            {
                DI.UserDialog.ShowDialog("Console script shouldn't be empty");
                return;
            };

            var project = Project.Project;

            // Add the new process to the project's process list
            project.ProcessList.Add(new ConsoleProcess(ConsoleScript, ConsoleDirectory, ProcessVisibleOnStartup)
            {
                ProcessLabel = ProcessLabel,
            });

            // Serialize the process list after adding the project
            byte[] json = DI.Serializer.SerializeProcessList(project.ProcessList);

            // Write the json to the project's config file
            File.WriteAllBytes(project.ProjectPathWithConfig, json);

            DI.UI.ChangeView(View.ProjectItemView, Project);
        }

    };
};